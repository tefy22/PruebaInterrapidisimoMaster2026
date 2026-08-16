using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public sealed record Rating
    {
        public decimal Value { get; init; }

        private Rating(decimal value) => Value = value;

        public static Result<Rating> Create(decimal value)
        {
            if (value < 0 || value > 5)
            {
                return Result.Failure<Rating>(ObjectsValueErrors.RatingInvalid);
            }
            return new Rating(value);
        }
    }
}
