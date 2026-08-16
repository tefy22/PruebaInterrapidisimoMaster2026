using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public sealed class RegistrationDetail : Entity
    {
        public Guid SubjectId { get; private set; }
        public Rating Rating { get; private set; } = Rating.Create(0).Value;

        internal RegistrationDetail(Guid id, Guid subjectId) : base(id)
        {
            SubjectId = subjectId;
        }
    }
}
