using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public record UserDto(
        Guid Id,
        int DNI,
        string Name,
        string LastName,
        string Email,
        string PhoneNumber,
        Guid RoleId,
        int Status
    );
}
