using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.UpdateSubject
{
    public record UpdateSubjectCommand(Guid id, string name, int credits, Guid idTeacher, int estado) : ICommand<Guid>; 
}
