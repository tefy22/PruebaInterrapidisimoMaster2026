using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public record RegistrationDetailDto(Guid id, Guid subjectId, string subjectName, int credits, decimal rating, 
        Guid teacherId, string teacherName);


}
