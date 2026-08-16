using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Registrations.UpdateRegistration;
using Domain.Abstractions;
using Domain.Registrations;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace TestMaster.Application.Tests
{
    public class DeleteRegistrationTest
    {
        private readonly DeleteRegistrationCommandHandler _handler;
        private readonly IRegistrationRepository _registrationRepositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;

        public DeleteRegistrationTest()
        {
            _registrationRepositoryMock = Substitute.For<IRegistrationRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();

            _handler = new DeleteRegistrationCommandHandler(
                _registrationRepositoryMock,
                _unitOfWorkMock
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryReturnsFailure()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteRegistrationCommand(id);

            _registrationRepositoryMock
                .Delete(id)
                .Returns(Task.FromResult(Result.Failure(Error.NullValue)));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.DeleteError);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRepositoryReturnsSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteRegistrationCommand(id);

            _registrationRepositoryMock
                .Delete(id)
                .Returns(Task.FromResult(Result.Success()));

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsync_WhenDeletionIsSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteRegistrationCommand(id);

            _registrationRepositoryMock
                .Delete(id)
                .Returns(Task.FromResult(Result.Success()));

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryDelete_WithCorrectId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteRegistrationCommand(id);

            _registrationRepositoryMock
                .Delete(Arg.Any<Guid>())
                .Returns(Task.FromResult(Result.Success()));

            _unitOfWorkMock
                .SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _registrationRepositoryMock.Received(1).Delete(id);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteRegistrationCommand(id);

            _registrationRepositoryMock
                .Delete(Arg.Any<Guid>())
                .Throws(new Exception("repository error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(RegistrationErrors.DeleteError);
        }
    }
}
