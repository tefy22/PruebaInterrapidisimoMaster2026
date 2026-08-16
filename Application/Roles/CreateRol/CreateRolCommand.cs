using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.CreateRol
{
    public record CreateRolCommand(string Description) : ICommand<Guid>;
}
