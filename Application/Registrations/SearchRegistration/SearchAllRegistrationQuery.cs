using Application.Abstractions.Messaging;
using Domain.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.SearchRegistration
{
    public record SearchAllRegistrationQuery : ICommand<IReadOnlyList<RegistrationDto>>;
   
}
