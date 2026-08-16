using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Subjects.UpdateSubject;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class UpdateSubjectTest
    {
        private readonly UpdateSubjectCommandHandler _handler;
        private readonly ISubjectRepository _subjectRepository;
        public readonly IRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubjectTest()
        {
            _subjectRepository = Substitute.For<ISubjectRepository>();
            _registrationRepository= Substitute.For<IRegistrationRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdateSubjectCommandHandler(_subjectRepository, _unitOfWork, _registrationRepository);
        }

        [Fact]
        public async Task Handle_NullRequest_ReturnsFailure()
        {
            // Act
            var result = await _handler.Handle(null!, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidNameOrCredits_ReturnsFailure()
        {
            // Arrange: nombre vacío -> Name.Create fallará
            var cmd = new UpdateSubjectCommand(
                id: Guid.NewGuid(),
                name: string.Empty,
                credits: 0,
                idTeacher: Guid.NewGuid(),
                estado: 1
            );

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenTeacherHasTooManySubjects_ReturnsCreditsTeacherComplete()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var cmd = new UpdateSubjectCommand(
                id: Guid.NewGuid(),
                name: "Algebra",
                credits: 3,
                idTeacher: teacherId,
                estado: 1 // activa -> chequea límite del profesor
            );

            // Simular que el profesor ya tiene 2 materias
            _subjectRepository.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>())
                .Returns(new List<Subject> { CreateSubjectInstance(), CreateSubjectInstance() });

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.CreditsTeacherComplete);
        }

        [Fact]
        public async Task Handle_WhenRepositoryUpdateFails_ReturnsUpdateError()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var cmd = new UpdateSubjectCommand(
                id: id,
                name: "Calculo",
                credits: 3,
                idTeacher: teacherId,
                estado: 1
            );

            _subjectRepository.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>())
                .Returns(new List<Subject>()); // no excede el límite

            _subjectRepository.Update(Arg.Any<Subject>()).Returns(Task.FromResult(Result.Failure(SubjectErrors.UpdateError)));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.UpdateError);
        }

        [Fact]
        public async Task Handle_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var cmd = new UpdateSubjectCommand(
                id: id,
                name: "Fisica", // Sin tilde para pasar la validación de Name.Create
                credits: 3,
                idTeacher: teacherId,
                estado: 1
            );

            _subjectRepository.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>())
                .Returns(new List<Subject>()); // No excede el límite del profesor

            _subjectRepository.Update(Arg.Any<Subject>())
                .Returns(Task.FromResult(Result.Success()));

            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(id);
            await _subjectRepository.Received(1).Update(Arg.Is<Subject>(s => s.Id == id));
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ReturnsCreateError()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var cmd = new UpdateSubjectCommand(
                id: id,
                name: "Historia",
                credits: 3,
                idTeacher: teacherId,
                estado: 1
            );

            _subjectRepository
                .When(x => x.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>()))
                .Do(x => throw new Exception("DB"));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.CreateError);
        }

        private static Subject CreateSubjectInstance()
        {
            var nameResult = Name.Create("Algebra");
            var creditsResult = Credits.Create(3); // Cambiado de 1 a 3

            if (nameResult.IsSuccess && creditsResult.IsSuccess)
            {
                var subjectResult = Subject.Create(nameResult.Value, creditsResult.Value, Guid.NewGuid());
                if (subjectResult.IsSuccess)
                {
                    return subjectResult.Value;
                }
            }

            // Fallback por reflexión en caso de fallo
            var subjFallback = (Subject)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Subject));
            SetPropertyValue(subjFallback, "Id", Guid.NewGuid());
            SetPropertyValue(subjFallback, "Status", StatusDetails.Active);

            if (nameResult.IsSuccess) SetPropertyValue(subjFallback, "Name", nameResult.Value);
            if (creditsResult.IsSuccess) SetPropertyValue(subjFallback, "Credits", creditsResult.Value);

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
