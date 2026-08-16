using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public record RegistrationDto(Guid id, Guid studentId, string studentName, int status, IReadOnlyList<RegistrationDetailDto> details);

}
