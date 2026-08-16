using Application.Abstractions.Messaging;
using Domain.User;
using System;

namespace Application.Users.SearchUser
{
    public record SearchStudentByIdQuery(Guid Id) : ICommand<UserDto>;
}