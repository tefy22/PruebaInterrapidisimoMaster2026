using Domain.Subjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaster.Domain.Tests
{
    public class SubjectTest
    {
        [Fact]
        public void CreateSubject_Should_WhenValidParameters()
        {
            // Arrange
            //se creó en MockSubject

            //Act 
            var result = Subject.Create(SubjectMock.nombre, SubjectMock.credits, SubjectMock.idUser);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(SubjectMock.nombre);
            result.Value.Credits.Should().Be(SubjectMock.credits);
            result.Value.UserId.Should().Be(SubjectMock.idUser);
        }
        [Fact]
        public void UpdateSubject_Should_WhenValidParameters()
        {
            // Arrange
            var subjectResult = Subject.Create(SubjectMock.nombre, SubjectMock.credits, SubjectMock.idUser);
            var subject = subjectResult.Value;
            // Act
            var updateResult = Subject.Update(
                subject.Id,
                SubjectMock.nombre,
                SubjectMock.credits,
                SubjectMock.idUser,
                SubjectMock.status
            );
            // Assert
            updateResult.IsSuccess.Should().BeTrue();
            subject.Name.Should().Be(SubjectMock.nombre);
            subject.Credits.Should().Be(SubjectMock.credits);
            subject.UserId.Should().Be(SubjectMock.idUser);
            subject.Status.Should().Be(SubjectMock.status);
        }
    }
}
