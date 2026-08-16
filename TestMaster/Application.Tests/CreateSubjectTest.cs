using Application.Subjects.CreateSubject;
using Domain.Abstractions;
using Domain.Subjects;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaster.Application.Tests
{
    public class CreateSubjectTest
    {
        private readonly CreateSubjectCommandHandler _handler;
        private readonly ISubjectRepository _subjectRepositoryMock;
        private readonly IUnitOfWork _unitOfWorkMock;
        private readonly CreateSubjectCommand _commandMock = new CreateSubjectCommand(
            name: "Mathematics",
            credits: 3,
            idUser: Guid.NewGuid()
        );

        public CreateSubjectTest()
        {
            _subjectRepositoryMock = Substitute.For<ISubjectRepository>();
            _unitOfWorkMock = Substitute.For<IUnitOfWork>();

            // Handler real que vamos a probar, inyectando los mocks
            _handler = new CreateSubjectCommandHandler(_subjectRepositoryMock, _unitOfWorkMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenCommandIsInValid()
        {
            // Arrange: comando inválido (campos vacíos / créditos 0)
            var invalidCommand = new CreateSubjectCommand(
                name: string.Empty,
                credits: 0, //solo admite 3, cambiar a varios numeros diferentes a 3 para probar
                idUser: Guid.Empty
            );
            // Act
            var result = await _handler.Handle(invalidCommand, CancellationToken.None);
            // Assert: debe ser fallo por validación
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCommandValid()
        {
            // Arrange: comando válido
            var validCommand = new CreateSubjectCommand(
                name: "Physics",
                credits: 3,
                idUser: Guid.NewGuid()
            );
            // Act
            var result = await _handler.Handle(validCommand, CancellationToken.None);
            // Assert: debe ser éxito
            result.IsSuccess.Should().BeTrue();
        }
    }
}
