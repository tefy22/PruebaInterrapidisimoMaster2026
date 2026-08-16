using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CreateUser
{
    public record CreateUserRequest(int dni, string name, string lastName, string email, string password, string phoneNumber, Guid roleId);
}
