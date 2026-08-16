using Domain.User;
using Domain.ValueObjects;
using Xunit;
using System;
using FluentAssertions;

namespace TestMaster.Domain.Tests
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_Should_WhenValidParameters()
        {
            // Arrange
            //se creó en MockUser

            //Act 
            var result = User.Create(UserMock.dni, UserMock.nombre, UserMock.apellido, UserMock.email, UserMock.password, UserMock.phoneNumber, UserMock.idRol);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(UserMock.nombre);
            result.Value.LastName.Should().Be(UserMock.apellido);
            result.Value.Email.Should().Be(UserMock.email);
            result.Value.Password.Should().Be(UserMock.password);
            result.Value.PhoneNumber.Should().Be(UserMock.phoneNumber);
            result.Value.RolId.Should().Be(UserMock.idRol);
        }

        [Fact]
        public void UpdateUser_Should_WhenValidParameters()
        {
            // Arrange
            var userResult = User.Create(UserMock.dni, UserMock.nombre, UserMock.apellido, UserMock.email, UserMock.password, UserMock.phoneNumber, UserMock.idRol);
            var user = userResult.Value;

            // Act
            var updateResult = user.Update(
                UserMock.dni,
                UserMock.nombre,
                UserMock.apellido,
                UserMock.email,
                UserMock.password,
                UserMock.phoneNumber,
                user.Status,
                UserMock.idRol
            );

            // Assert
            updateResult.IsSuccess.Should().BeTrue();
            user.Name.Should().Be(UserMock.nombre);
            user.LastName.Should().Be(UserMock.apellido);
            user.Email.Should().Be(UserMock.email);
            user.Password.Should().Be(UserMock.password);
            user.PhoneNumber.Should().Be(UserMock.phoneNumber);
            user.RolId.Should().Be(UserMock.idRol);
        }
    }
}