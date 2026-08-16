using System;
using System.Collections.Generic;
using System.Runtime.Serialization; // Requerido para crear objetos sin constructor
using System.Threading;
using System.Threading.Tasks;
using Application.Users.DeleteUser;
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
    public class DeleteUserTest
    {
        private readonly IUserRepository _userRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _registrationRepository = Substitute.For<IRegistrationRepository>();
            _subjectRepository = Substitute.For<ISubjectRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _handler = new DeleteUserCommandHandler(
                _userRepository,
                _registrationRepository,
                _subjectRepository,
                _unitOfWork);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ReturnsUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.UserNotFound);
        }

        [Fact]
        public async Task Handle_WhenStudentHasExistingRegistration_ReturnsAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(userId);
            var user = CreateMockUser(userId);

            // SOLUCIÓN: Instanciamos un objeto puro de dominio omitiendo el constructor privado
            var dummyRegistration = (Registration)FormatterServices.GetUninitializedObject(typeof(Registration));

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _registrationRepository.GetByStudentIdAsync(userId, Arg.Any<CancellationToken>()).Returns(dummyRegistration);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.AlreadyExists);
        }

        [Fact]
        public async Task Handle_WhenTeacherHasSubjectsWithRegistrations_ReturnsAlreadyExists()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(teacherId);
            var user = CreateMockUser(teacherId);
            var subject = CreateMockSubject(Guid.NewGuid());

            _userRepository.GetByIdAsync(teacherId, Arg.Any<CancellationToken>()).Returns(user);
            _registrationRepository.GetByStudentIdAsync(teacherId, Arg.Any<CancellationToken>()).Returns((Registration?)null);
            _subjectRepository.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>()).Returns(new List<Subject> { subject });

            _registrationRepository.HasRegistrationDetailsForSubjectAsync(subject.Id, Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.AlreadyExists);
        }

        [Fact]
        public async Task Handle_WhenTeacherHasSubjectsWithoutRegistrations_ReturnsCannotDeleteTeacherWithSubjects()
        {
            // Arrange
            var teacherId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(teacherId);
            var user = CreateMockUser(teacherId);
            var subject = CreateMockSubject(Guid.NewGuid());

            _userRepository.GetByIdAsync(teacherId, Arg.Any<CancellationToken>()).Returns(user);
            _registrationRepository.GetByStudentIdAsync(teacherId, Arg.Any<CancellationToken>()).Returns((Registration?)null);
            _subjectRepository.GetSubjectForTeacher(teacherId, Arg.Any<CancellationToken>()).Returns(new List<Subject> { subject });
            _registrationRepository.HasRegistrationDetailsForSubjectAsync(subject.Id, Arg.Any<CancellationToken>()).Returns(false);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.CannotDeleteTeacherWithSubjects);
        }

        [Fact]
        public async Task Handle_WhenValidStudent_DeletesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(userId);
            var user = CreateMockUser(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _registrationRepository.GetByStudentIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Registration?)null);
            _subjectRepository.GetSubjectForTeacher(userId, Arg.Any<CancellationToken>()).Returns(new List<Subject>());
            _userRepository.Delete(userId).Returns(Task.FromResult(Result.Success()));
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _userRepository.Received(1).Delete(userId);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenRepoDeleteFails_ReturnsDeleteError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new DeleteUserCommand(userId);
            var user = CreateMockUser(userId);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _registrationRepository.GetByStudentIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Registration?)null);
            _subjectRepository.GetSubjectForTeacher(userId, Arg.Any<CancellationToken>()).Returns(new List<Subject>());
            _userRepository.Delete(userId).Returns(Task.FromResult(Result.Failure(UserErrors.DeleteError)));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.DeleteError);
        }

        // Helpers

        private static User CreateMockUser(Guid id)
        {
            var dni = DNI.Create(12345678).Value;
            var name = Name.Create("Test").Value;
            var lastName = LastName.Create("User").Value;
            var email = Email.Create("test.user@example.com").Value;
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Pass123");
            var password = Password.CreateFromHash(passwordHash).Value;
            var phone = PhoneNumber.Create("3111111111").Value;
            var rolId = Guid.NewGuid();

            var userResult = User.Create(dni, name, lastName, email, password, phone, rolId);
            var user = userResult.Value;
            typeof(User).GetProperty("Id")!.SetValue(user, id);
            return user;
        }

        private static Subject CreateMockSubject(Guid teacherId)
        {
            var subj = (Subject)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Subject));

            // Asignamos mediante reflexión los valores mínimos necesarios para que el Handler funcione
            typeof(Subject).GetProperty("Id")?.SetValue(subj, Guid.NewGuid());

            // Asegúrate de que el nombre de la propiedad "TeacherId" coincida con tu entidad (puede ser TeacherId o ProfesorId)
            typeof(Subject).GetProperty("TeacherId")?.SetValue(subj, teacherId);

            return subj;
        }
    }
}