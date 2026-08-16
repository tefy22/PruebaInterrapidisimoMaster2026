using Application.Abstractions.Messaging;
using Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.SearchSubject
{
    public record SearchActiveSubjectQuery() : ICommand<IReadOnlyList<SubjectDto>>;
}
