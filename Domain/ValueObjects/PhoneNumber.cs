using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public partial record PhoneNumber
    {
        private const int MaxLength = 10;   
        private const string Pattern = @"^3\d{9}$";
        public string Value { get; private set; }
        private PhoneNumber(string value) => Value = value;

        [GeneratedRegex(Pattern)]
        private static partial Regex PhoneNumberRegex();

        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<PhoneNumber>(ObjectsValueErrors.PhoneNumbersEmpty);

            if (value.Length != MaxLength)
                return Result.Failure<PhoneNumber>(ObjectsValueErrors.PhoneNumbersLength);

            if (!PhoneNumberRegex().IsMatch(value))
                return Result.Failure<PhoneNumber>(ObjectsValueErrors.PhoneNumbersStart);

            return new PhoneNumber(value);
        }

    }
}
