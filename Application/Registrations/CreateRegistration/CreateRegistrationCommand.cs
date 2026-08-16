using Application.Abstractions.Messaging;
using Domain.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.CreateRegistration
{
    public record CreateRegistrationCommand(Guid StudentId, List<Guid> SubjectId, StatusRegistrationDetails Status) : ICommand<Guid>;
}
