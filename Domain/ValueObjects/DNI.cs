using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public partial record DNI
    {
        public int Value { get; private set; }
        private const string Pattern = @"^\d{3,10}$";
        [GeneratedRegex(Pattern)]
        private static partial Regex DNIRegex();
        
        private DNI(int value) => Value = value;

        public static Result<DNI> Create(int value)
        {
            if (value <= 0)
                return Result.Failure<DNI>(ObjectsValueErrors.DNIEmpty);

            if (!DNIRegex().IsMatch(value.ToString()))
                return Result.Failure<DNI>(ObjectsValueErrors.DNIInvalid);

            return new DNI(value);   
        }
    }
}
