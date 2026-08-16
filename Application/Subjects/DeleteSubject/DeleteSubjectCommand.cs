using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.DeleteSubject
{
    public record DeleteSubjectCommand(Guid id) : ICommand;
}
