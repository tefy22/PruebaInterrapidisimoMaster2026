using Domain.Abstractions;
using Domain.ValueObjects;
using Domain.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Subjects
{
    public sealed class Subject : Entity
    {
        public Name Name { get; private set; }
        public Credits Credits { get; private set; }
        public Guid UserId { get; private set; }
        public StatusDetails Status { get; private set; } = StatusDetails.Active;
        public Domain.User.User User { get; private set; }

        private Subject()
        {
            
        }

        private Subject(Guid id, Name name, Credits credits, Guid userId, StatusDetails status) : base(id)
        {
            Name = name;
            Credits = credits;
            UserId = userId;
            Status = status;
        }
        public static Result<Subject> Create(Name name, Credits credits, Guid theacherId)
        {
            return new Subject(Guid.NewGuid(), name, credits, theacherId, StatusDetails.Active);
        }

        public static Result<Subject> Update(Guid id, Name name, Credits credits, Guid theacherId, StatusDetails status)
        {
            return new Subject(id, name, credits, theacherId, status);
        }
    }
}
