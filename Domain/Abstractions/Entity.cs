using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public abstract class Entity
    {
        private readonly List<IDomainEvents> _domainEvents = new();
        public Guid Id { get; init; }

        protected Entity()
        {
            
        }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public IReadOnlyList<IDomainEvents> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        protected void RaiseDomainEvent(IDomainEvents domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }   
    }
}
