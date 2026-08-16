using Application.Abstractions.Messaging;
using Domain.Abstractions;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.CreateUser
{
    internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(r=> r.email).NotEmpty().WithMessage("El email no puede ser nulo");
            RuleFor(r => r.password).NotEmpty().WithMessage("El password no puede ser nulo");
            RuleFor(r => r.roleId).NotEmpty().WithMessage("El rol no puede ser nulo");
        }
    }
}
