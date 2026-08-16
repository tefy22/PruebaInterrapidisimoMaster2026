using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Subjects
{
    public sealed record Credits
    {
        private const int MinCredits = 3;
        public int Value { get; init; }
        private Credits(int value) => Value = value;

        public static Result<Credits> Create(int value)
        {
            if (value != MinCredits)
                return Result.Failure<Credits>(ObjectsValueErrors.InvalidCredits);

            return new Credits(value);
        }
    }
}
