using Application.Abstractions.Messaging;
using Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Application.Subjects.SearchSubject
{
    public record SearchByIdSubjectQuery(Guid id):ICommand<SubjectDto>;
}
