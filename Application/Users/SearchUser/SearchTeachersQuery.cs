using Application.Abstractions.Messaging;
using Domain.User;
using System.Collections.Generic;

namespace Application.Users.SearchUser
{
    public record SearchTeachersQuery : ICommand<IReadOnlyList<UserDto>>;
}