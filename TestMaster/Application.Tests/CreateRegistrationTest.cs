using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Registrations.CreateRegistration;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.User;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class CreateRegistrationTest
    {
        private readonly CreateRegistrationCommandHandler _handler;
        private readonly IRegistrationRepository _registrationRepositoryMock;
        private readonly ISubjectRepository _subjectRepositoryMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;

        public CreateRegistrationTest()
        {
            _registrationRepositoryMock = Substitute.For<IRegistrationRepository>();
            _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();

            _handler = new CreateRegistrationCommandHandler(
                _registrationRepositoryMock,
                _subjectRepositoryMock,
                _userRepositoryMock,
                _unitOfWorkMock
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCommandIsNull()
        {
            // Act
            var result = await _handler.Handle(null!, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.NullValue);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSubjectIdListIsEmpty()
        {
            // Arrange
            var command = new CreateRegistrationCommand(
                StudentId: Guid.NewGuid(),
                SubjectId: new List<Guid>(),
                Status: StatusRegistrationDetails.EnCurso
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.EmptySubjects);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenStudentAlreadyHasRegistration()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var existingRegistration = CreateMockRegistration(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(existingRegistration);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.AlreadyExists);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenStudentNotFound()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.StudentNotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSubjectsNotFound()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var student = CreateMockUser(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(student);

            // Retorna solo 1 materia cuando se solicitan 2
            var subjects = new List<Subject> { CreateSubjectWithTeacher(Guid.NewGuid()) };
            _subjectRepositoryMock
                .GetByIdRegistrationAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(subjects);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(SubjectErrors.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenMoreThanThreeSubjects()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var student = CreateMockUser(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(student);

            var subjects = new List<Subject>
            {
                CreateSubjectWithTeacher(Guid.NewGuid()),
                CreateSubjectWithTeacher(Guid.NewGuid()),
                CreateSubjectWithTeacher(Guid.NewGuid()),
                CreateSubjectWithTeacher(Guid.NewGuid())
            };
            _subjectRepositoryMock
                .GetByIdRegistrationAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(subjects);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.MaxSubjects);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSameTeacherInMultipleSubjects()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var teacherId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var student = CreateMockUser(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(student);

            // Ambas materias tienen el mismo profesor
            var subjects = new List<Subject>
            {
                CreateSubjectWithTeacher(teacherId),
                CreateSubjectWithTeacher(teacherId)
            };
            _subjectRepositoryMock
                .GetByIdRegistrationAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(subjects);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.SameTeacherInSelection);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var student = CreateMockUser(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(student);

            // Tres materias con diferentes profesores
            var subjects = new List<Subject>
            {
                CreateSubjectWithTeacher(Guid.NewGuid()),
                CreateSubjectWithTeacher(Guid.NewGuid()),
                CreateSubjectWithTeacher(Guid.NewGuid())
            };
            _subjectRepositoryMock
                .GetByIdRegistrationAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(subjects);

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsync_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var subjectIds = new List<Guid> { Guid.NewGuid() };
            var command = new CreateRegistrationCommand(
                StudentId: studentId,
                SubjectId: subjectIds,
                Status: StatusRegistrationDetails.EnCurso
            );

            var student = CreateMockUser(studentId);
            _registrationRepositoryMock
                .GetByStudentIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns((Registration?)null);

            _userRepositoryMock
                .GetStudentsByIdAsync(studentId, Arg.Any<CancellationToken>())
                .Returns(student);

            var subjects = new List<Subject>
            {
                CreateSubjectWithTeacher(Guid.NewGuid())
            };
            _subjectRepositoryMock
                .GetByIdRegistrationAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
                .Returns(subjects);

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        // Helpers de instanciación
        private static User CreateMockUser(Guid id)
        {
            var user = (User)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(User));
            SetPropertyValue(user, "Id", id);
            return user;
        }

        private static Registration CreateMockRegistration(Guid studentId)
        {
            var reg = (Registration)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Registration));
            SetPropertyValue(reg, "Id", Guid.NewGuid());
            SetPropertyValue(reg, "StudentId", studentId);
            return reg;
        }

        private static Subject CreateSubjectWithTeacher(Guid teacherId)
        {
            var nameResult = Name.Create("Matematicas");
            var creditsResult = Credits.Create(3);

            if (nameResult.IsSuccess && creditsResult.IsSuccess)
            {
                var subjectResult = Subject.Create(nameResult.Value, creditsResult.Value, teacherId);
                if (subjectResult.IsSuccess)
                {
                    return subjectResult.Value;
                }
            }

            var subject = (Subject)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Subject));
            SetPropertyValue(subject, "Id", Guid.NewGuid());
            SetPropertyValue(subject, "UserId", teacherId);
            SetPropertyValue(subject, "Status", StatusDetails.Active);
            return subject;
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