using Domain.Registrations;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaster.Domain.Tests
{
    public class RegistrationTest
    {
        [Fact]
        public void CreateRegistration_Should_WhenValidParameters()
        {
            // Arrange
            //se creó en MockRegistration
            //Act 
            var result = Registration.Create(RegistrationMock.IdStudent, RegistrationMock.SubjectIds.ToList(), RegistrationMock.Status);
            
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.StudentId.Should().Be(RegistrationMock.IdStudent);
            result.Value.Details.Select(d => d.SubjectId).Should().BeEquivalentTo(RegistrationMock.SubjectIds);
            result.Value.Status.Should().Be(RegistrationMock.Status);
        }
    }
}
