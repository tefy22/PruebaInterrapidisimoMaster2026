using Domain.Roles;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaster.Domain.Tests
{
    public class RoleTest
    {
        [Fact]
        public void CreateRole_ShouldReturnSuccess()
        {
            // Arrange
            
            // Act
            var result = Role.Create(RoleMock.Description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            result.Value.Description.Should().Be(RoleMock.Description);
        }
    }
}
