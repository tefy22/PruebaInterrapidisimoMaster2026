using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UpdateUser
{
    public record UpdateUserCommand(
        Guid id,
        int dni,
        string name,
        string lastName,
        string email,
        string password,
        string phoneNumber,
        Guid roleId,
        int status
    ) : ICommand<Guid>;
}
