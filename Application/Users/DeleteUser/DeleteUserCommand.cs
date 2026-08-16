using Application.Abstractions.Messaging;
using System;

namespace Application.Users.DeleteUser
{
    public record DeleteUserCommand(Guid id) : ICommand;
}
