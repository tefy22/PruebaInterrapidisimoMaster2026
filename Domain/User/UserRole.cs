using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public sealed class UserRole
    {
        public int RolId { get; private set; }
        public Guid UserId { get; private set; }

        private UserRole() { }

        public UserRole(int rolId, Guid userId)
        {
            RolId = rolId;
            UserId = userId;
        }

        public static UserRole Create(int rolId, Guid userId) => new UserRole(rolId, userId);

    }
}
