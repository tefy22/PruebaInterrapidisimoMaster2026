using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.CreateSubject
{
    public record CreateSubjectCommand(string name, int credits, Guid idUser): ICommand<Guid>;
}
