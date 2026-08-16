using Domain.Abstractions;
using Domain.Registrations.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public sealed class Registration : Entity
    {
        public Guid StudentId { get; private set; }

        private readonly List<RegistrationDetail> _details = new();
        public IReadOnlyCollection<RegistrationDetail> Details => _details.AsReadOnly();

        public DateTime RegistrationDate { get; private set; }
        public StatusRegistrationDetails Status { get; private set; } = StatusRegistrationDetails.EnCurso;

        private Registration(Guid id, Guid studentId, StatusRegistrationDetails status) : base(id)
        {
            StudentId = studentId;
            RegistrationDate = DateTime.Now;
            Status = status;
        }

        public static Result<Registration> Create(Guid studentId, List<Guid> subjectIds, StatusRegistrationDetails status)
        {
            var registration = new Registration(Guid.NewGuid(), studentId, status);

            //Agregamos las materias a la lista interna
            foreach (var item in subjectIds)
            {
                registration._details.Add(new RegistrationDetail(Guid.NewGuid(), item));
            }

            //Generamos el evento de dominio para indicar que se ha creado un nuevo registro
            registration.RaiseDomainEvent(new RegistrationCreateDomainEvent(registration.Id));

            return registration;
        }
        
    }
}
