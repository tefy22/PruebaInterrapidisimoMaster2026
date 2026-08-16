using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public class Result
    {
        public bool IsSuccess { get;} 
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected internal Result(bool isSucess, Error error) {

            if (isSucess && error != Error.None)
                throw new InvalidOperationException();

            if (!isSucess && error == Error.None)
                throw new InvalidOperationException();

            IsSuccess = isSucess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
        public static Result<TValue> Create<TValue>(TValue value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    [NotNull]
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("El resultado del valor de error no es admisible");
    
    public static implicit operator Result<TValue>(TValue value) => Create(value);

    protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    
}
