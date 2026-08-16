using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Roles
{
    public sealed class Role : Entity
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RolesDetails Description { get; private set; }
        private Role()
        {

        }

        private Role(Guid id, RolesDetails description) : base(id)
        {
            Description = description;
        }
        public static Result<Role> Create(RolesDetails description)
        {
            return new Role(Guid.NewGuid(), description);
        }


    }
}
