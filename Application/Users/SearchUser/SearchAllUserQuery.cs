using Application.Abstractions.Messaging;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.SearchUser
{
    public record SearchAllUserQuery : ICommand<IReadOnlyList<UserDto>>;
}
