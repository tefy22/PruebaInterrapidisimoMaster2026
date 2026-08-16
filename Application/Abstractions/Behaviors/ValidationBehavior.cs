using Application.Abstractions.Messaging;
using Application.Exceptions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseCommand
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationErrors = _validators.Select(v => v.Validate(context))
                .Where(vr => vr.Errors.Any())
                .SelectMany(vr => vr.Errors)
                .Select(vf => new ValidationError(
                        vf.PropertyName,
                        vf.ErrorMessage
                )).ToList();

            if (validationErrors.Any())
                throw new Exceptions.ValidationException(validationErrors);

            return await next();
        }
    }
}
