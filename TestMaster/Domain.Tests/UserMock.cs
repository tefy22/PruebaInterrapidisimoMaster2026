using Domain.Registrations;
using Domain.Roles;
using Domain.Subjects;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMaster.Domain.Tests
{
    internal class UserMock
    {
        public static readonly DNI dni = DNI.Create(1020202020).Value;
        public static readonly Name nombre = Name.Create("Stefania").Value;
        public static readonly LastName apellido = LastName.Create("Afanador").Value;
        public static readonly Email email = Email.Create("s.afanador@gmail.com").Value;
        public static readonly Password password =  Password.Create("Stefania123456").Value;
        public static readonly PhoneNumber phoneNumber = PhoneNumber.Create("3132656396").Value;
        public static readonly Guid idRol = new Guid("e7f3c1a0-8b9d-4c2e-9f3b-1a2b3c4d5e6f");

    }

    internal class SubjectMock
    {
        public static readonly Name nombre = Name.Create("Matematicas").Value;
        public static readonly Credits credits = Credits.Create(3).Value;
        public static readonly Guid idUser = new Guid("e7f3c1a0-8b9d-4c2e-9f3b-1a2b3c4d5e6f");
        public static readonly StatusDetails status = StatusDetails.Active;
    }

    internal class  RegistrationMock
    {
        public static readonly Guid IdStudent = new Guid("11111111-1111-1111-1111-111111111111");

        public static readonly IReadOnlyList<Guid> SubjectIds = new List<Guid>
        {
            new Guid("22222222-2222-2222-2222-222222222222"),
            new Guid("33333333-3333-3333-3333-333333333333")
        };

        public static readonly StatusRegistrationDetails Status = StatusRegistrationDetails.EnCurso;

    }

    internal class RoleMock
    {
        public static readonly RolesDetails Description = RolesDetails.Teacher;
    }
}
