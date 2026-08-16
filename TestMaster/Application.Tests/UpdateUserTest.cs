using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.UpdateUser;
using Domain.Abstractions;
using Domain.User;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class UpdateUserTest
    {
        private readonly UpdateUserCommandHandler _handler;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UpdateUserCommandHandler(_userRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ReturnsFailure()
        {
            // Arrange: datos inválidos (dni 0, nombre vacío, email inválido, password inválido, teléfono inválido)
            var cmd = new UpdateUserCommand(
                id: Guid.NewGuid(),
                dni: 0,
                name: string.Empty,
                lastName: string.Empty,
                email: "bad-email",
                password: "short",
                phoneNumber: "123",
                roleId: Guid.Empty,
                status: 1
            );

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ReturnsUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cmd = new UpdateUserCommand(
                id: userId,
                dni: 12345678,
                name: "John",
                lastName: "Doe",
                email: "john.doe@example.com",
                password: "Password123", // cumple las reglas
                phoneNumber: "3111111111",
                roleId: Guid.NewGuid(),
                status: 1
            );

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(null));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.UserNotFound);
        }

        [Fact]
        public async Task Handle_WhenEmailChangeAndExists_ReturnsExistsEmail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existing = CreateMockUser(userId, email: "old@example.com");

            var cmd = new UpdateUserCommand(
                id: userId,
                dni: 12345678,
                name: "John",
                lastName: "Doe",
                email: "new@example.com", // distinto al existente -> dispara comprobación de unicidad
                password: "Password123",
                phoneNumber: "3111111111",
                roleId: existing.RolId,
                status: 1
            );

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existing);
            _userRepository.IsUserExists(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(true);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserErrors.ExistsEmail);
        }

        [Fact]
        public async Task Handle_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existing = CreateMockUser(userId, email: "same@example.com");

            var cmd = new UpdateUserCommand(
                id: userId,
                dni: 12345678,
                name: "JohnUpdated",
                lastName: "DoeUpdated",
                email: "same@example.com", // mismo email -> no chequeo de unicidad
                password: "pass12345678iK", // vacío -> mantiene la contraseña actual
                phoneNumber: "3111111111",
                roleId: existing.RolId,
                status: 1
            );

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existing);
            _userRepository.Update(Arg.Any<User>()).Returns(Task.FromResult(Result.Success()));
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(existing.Id);
            await _userRepository.Received(1).Update(existing);
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenRepoUpdateFails_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existing = CreateMockUser(userId, email: "same@example.com");

            var cmd = new UpdateUserCommand(
                id: userId,
                dni: 12345678,
                name: "JohnUpdated",
                lastName: "DoeUpdated",
                email: "same@example.com",
                password: "pass1234567WS",
                phoneNumber: "3111111111",
                roleId: existing.RolId,
                status: 1
            );

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existing);
            _userRepository.Update(Arg.Any<User>()).Returns(Task.FromResult(Result.Failure(UserErrors.UpdateError)));

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();

            // 1. UnitOfWork NO debe recibir llamadas (Correcto, lleva await porque es Async)
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

            // 2. CORRECCIÓN: Quita el 'await' de aquí si tu método de repositorio es sincrónico
            _userRepository.Received(1).Update(Arg.Any<User>());

        }

        private static User CreateMockUser(Guid id, string? email = null)
        {
            var dni = DNI.Create(12345678).Value;
            var name = Name.Create("Existing").Value;
            var lastName = LastName.Create("User").Value;
            var emailVo = Email.Create(email ?? "existing@example.com").Value;
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Pass123456789"); // hash válido
            var password = Password.CreateFromHash(passwordHash).Value;
            var phone = PhoneNumber.Create("3111111111").Value;
            var rolId = Guid.NewGuid();

            var userResult = User.Create(dni, name, lastName, emailVo, password, phone, rolId);
            var user = userResult.Value;
            typeof(User).GetProperty("Id")!.SetValue(user, id);
            return user;
        }
    }
}