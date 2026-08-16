using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{    
    public partial record LastName
    {
        public string Value { get; private set; }
        private LastName(string value) => Value = value;
        public static Result<LastName> Create(string value)
        {
            if(string.IsNullOrEmpty(value))
                return Result.Failure<LastName>(ObjectsValueErrors.LastNameEmpty);

            return new LastName(value);
        }
    }
}
