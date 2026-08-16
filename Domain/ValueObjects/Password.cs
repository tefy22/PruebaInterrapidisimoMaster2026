using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Domain.ValueObjects
{
    public partial record Password
    {
        /*         
         Mínimo 8 caracteres.
        Al menos una letra mayúscula.
        Al menos una letra minúscula.
        Al menos un número.
         */
        private const int MinLength = 8;
        private const int MaxLength = 20;
        private const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,20}$";
        public string Value { get; private set; }
        private Password(string value) => Value = value;

        [GeneratedRegex(Pattern)]
        private static partial Regex PasswordRegex();
        public static Result<Password> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Password>(ObjectsValueErrors.PasswordEmpty);

            return new Password(value);
        }

        public static Result<Password> Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Failure<Password>(ObjectsValueErrors.PasswordEmpty);

            if (value.Length < MinLength || value.Length > MaxLength)
                return Result.Failure<Password>(ObjectsValueErrors.PasswordLength);

            if (!PasswordRegex().IsMatch(value))
                return Result.Failure<Password>(ObjectsValueErrors.PasswordCharacter);

            return new Password(value);
        }

        public static Result<Password> CreateFromHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return Result.Failure<Password>(ObjectsValueErrors.PasswordEmpty);
            return new Password(hash);
        }
    }
}
