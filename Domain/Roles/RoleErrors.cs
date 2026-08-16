using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Roles
{
    public static class RoleErrors
    {
        public static Error NotFound = new Error("RoleErrors.NotFound", "El rol digitado no existe");
    }
}
