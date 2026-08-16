using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public partial record Name
    {
        public string Value { get; private set; }
        private Name(string value) => Value = value;
        public static Result<Name> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Result.Failure<Name>(ObjectsValueErrors.NameEmpty);

            return new Name(value);
        }
    }

}
