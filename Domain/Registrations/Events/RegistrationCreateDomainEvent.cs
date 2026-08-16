using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations.Events
{
    public sealed record RegistrationCreateDomainEvent(Guid RegistrationId) : IDomainEvents;

}
