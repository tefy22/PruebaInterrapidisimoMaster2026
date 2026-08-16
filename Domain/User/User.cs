using Domain.Abstractions;
using Domain.Roles;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public sealed class User : Entity
    {
        public DNI DNId { get; private set; }
        public Name Name { get; private set; }
        public LastName LastName { get; private set; }
        public Email Email { get; private set; }
        public Password Password { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public DateTime? CreatedAt { get; private set; }
        public Guid RolId { get; private set; }
        public StatusDetails Status { get; private set; } = StatusDetails.Active;

        private User()
        {

        }
        public User(DNI dNId, Name name, LastName lastName, Email email, Password password, PhoneNumber phoneNumber, StatusDetails status, Guid rolId)
        {
            DNId = dNId;
            Name = name;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            CreatedAt = DateTime.Now;
            Status = status;
            RolId = rolId;
        }

        public static Result<User> Create(DNI dNId, Name name, LastName lastName, Email email, Password password, PhoneNumber phoneNumber, Guid rolId)
        {
            return new User(dNId, name, lastName, email, password, phoneNumber, StatusDetails.Active, rolId);
        }

        public Result Update(DNI dNId, Name name, LastName lastName, Email email, Password password, PhoneNumber phoneNumber, StatusDetails status, Guid rolId)
        {
            if (dNId is null || name is null || lastName is null || email is null || password is null || phoneNumber is null)
                return Result.Failure(Error.NullValue);

            DNId = dNId;
            Name = name;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            Status = status;
            RolId = rolId;

            return Result.Success();
        }

    }
}
