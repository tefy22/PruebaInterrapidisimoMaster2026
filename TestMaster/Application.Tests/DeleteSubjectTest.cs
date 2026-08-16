using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Subjects.DeleteSubject;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class DeleteSubjectTest
    {
        private readonly DeleteSubjectCommandHandler _handler;
        private readonly ISubjectRepository _subjectRepositoryMock;
        private readonly IRegistrationRepository _registration_repositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly DeleteSubjectCommand _commandMock = new DeleteSubjectCommand(
            id: Guid.NewGuid()
        );

        public DeleteSubjectTest()
        {
            _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
            _registration_repositoryMock = Substitute.For<IRegistrationRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();

            // Handler real que vamos a probar, inyectando los mocks
            _handler = new DeleteSubjectCommandHandler(_subjectRepositoryMock, _registration_repositoryMock, _unitOfWorkMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenSubjectNotFound()
        {           
            // Arrange
            var subjectId = Guid.NewGuid();
            var cmd = new DeleteSubjectCommand(subjectId);

            _subjectRepositoryMock.GetByIdAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Subject?>(null));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSubjectHasRegistrations()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var cmd = new DeleteSubjectCommand(subjectId);
            var subject = CreateMockSubject(subjectId);

            _subjectRepositoryMock.GetByIdAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Subject?>(subject));
            _registration_repositoryMock.HasRegistrationDetailsForSubjectAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.CannotDeleteSubjectWithRegistrations);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAndReturnSuccess_WhenSubjectValidAndNoRegistrations()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var cmd = new DeleteSubjectCommand(subjectId);
            var subject = CreateMockSubject(subjectId);

            _subjectRepositoryMock.GetByIdAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Subject?>(subject));
            _registration_repositoryMock.HasRegistrationDetailsForSubjectAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            _subjectRepositoryMock.Delete(subjectId).Returns(Task.FromResult(Result.Success()));
            _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _subjectRepositoryMock.Received(1).Delete(subjectId);
            await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryDeleteFails()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var cmd = new DeleteSubjectCommand(subjectId);
            var subject = CreateMockSubject(subjectId);

            _subjectRepositoryMock.GetByIdAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Subject?>(subject));
            _registration_repositoryMock.HasRegistrationDetailsForSubjectAsync(subjectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(false));

            _subjectRepositoryMock.Delete(subjectId).Returns(Task.FromResult(Result.Failure(SubjectErrors.DeleteError)));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.DeleteError);
            await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            // Arrange
            var subjectId = Guid.NewGuid();
            var cmd = new DeleteSubjectCommand(subjectId);

            _subjectRepositoryMock
                .When(x => x.GetByIdAsync(subjectId, Arg.Any<CancellationToken>()))
                .Do(call => throw new Exception("Database error"));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.DeleteError);
        }

        private static Subject CreateMockSubject(Guid id)
        {
            // 1. Usar texto simple sin tildes para no violar las reglas de validación del Value Object
            var nameResult = Name.Create("Matematicas");
            var creditsResult = Credits.Create(6);

            // 2. Si las fábricas tienen éxito, instanciar normalmente
            if (nameResult.IsSuccess && creditsResult.IsSuccess)
            {
                var subjResult = Subject.Create(nameResult.Value, creditsResult.Value, Guid.NewGuid());
                if (subjResult.IsSuccess)
                {
                    var subj = subjResult.Value;
                    SetPropertyValue(subj, "Id", id);
                    return subj;
                }
            }

            // 3. Fallback seguro por reflexión (sin llamar a .Value en objetos Result fallidos)
            var subjFallback = (Subject)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Subject));
            SetPropertyValue(subjFallback, "Id", id);
            SetPropertyValue(subjFallback, "UserId", Guid.NewGuid());
            SetPropertyValue(subjFallback, "Status", StatusDetails.Active);

            if (nameResult.IsSuccess)
                SetPropertyValue(subjFallback, "Name", nameResult.Value);

            if (creditsResult.IsSuccess)
                SetPropertyValue(subjFallback, "Credits", creditsResult.Value);

            return subjFallback;
        }

        private static void SetPropertyValue(object target, string propertyName, object value)
        {
            var prop = target.GetType().GetProperty(propertyName);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(target, value);
            }
        }
    }
}
