using Application.Abstractions.Messaging;
using Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.SearchRol
{
    public record SearchAllRolQuery() : ICommand<IReadOnlyList<RolDto>>;
}
