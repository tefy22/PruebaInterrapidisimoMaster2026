using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public partial record Email
    {
        private const string Pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public string Value { get; private set; }
        private Email(string value) => Value = value;

        [GeneratedRegex(Pattern)]
        private static partial Regex EmailRegex();

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Email>(ObjectsValueErrors.EmailEmpty);

            if (!EmailRegex().IsMatch(value))
                return Result.Failure<Email>(ObjectsValueErrors.EmailInvalid);

            return new Email(value);
        }

    }
}
