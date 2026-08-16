
using Application.Abstractions.Messaging;
using Domain.User;
using System.Collections.Generic;

namespace Application.Users.SearchUser
{
    public record SearchStudentsQuery : ICommand<IReadOnlyList<UserDto>>;
}