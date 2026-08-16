using Application.Abstractions.Messaging;
using Domain.Registrations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.SearchRegistration
{
    public sealed record GetSharedSubjectsQuery(Guid StudentId) : ICommand<IReadOnlyList<SharedSubjectDto>>;

}
