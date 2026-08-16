using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.LoginUser
{
    public record LoginCommand(string email, string password) : ICommand<string>;
}
