using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.CreateUser;
using Domain.Abstractions;
using Domain.User;
using Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class CreateUserTest
    {
        private readonly CreateUserCommandHandler _handler;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly CreateUserCommand _commandMock = new CreateUserCommand(
            dni: 12345678,
            name: "John",
            lastName: "Doe",
            email: "j.doe@gmail.com",
            password: "Password123", 
            phoneNumber: "3111111111",
            roleId: Guid.NewGuid()
        );

        public CreateUserTest()
        {
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();

            // Handler real que vamos a probar, inyectando los mocks
            _handler = new CreateUserCommandHandler(_userRepositoryMock, _unitOfWorkMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenCommandIsInValid()
        {
            // Arrange: comando inválido (campos vacíos / dni 0)
            var invalidCommand = new CreateUserCommand(
                dni: 0,
                name: string.Empty,
                lastName: string.Empty,
                email: string.Empty,
                password: string.Empty,
                phoneNumber: string.Empty,
                roleId: Guid.Empty
            );

            // Act
            var result = await _handler.Handle(invalidCommand, CancellationToken.None);

            // Assert: debe ser fallo por validación
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCommandValid()
        {
            // Arrange
            var cmd = _commandMock;

            _userRepositoryMock
                .IsUserExists(Arg.Any<Email>(), Arg.Any<CancellationToken>())
                .Returns(false);

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
