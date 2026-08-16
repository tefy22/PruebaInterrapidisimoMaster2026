using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.CreateRol
{
    public class CreateRolCommandValidator :AbstractValidator<CreateRolCommand>
    {
        public CreateRolCommandValidator()
        {
            RuleFor(c => c.Description).NotEmpty().WithMessage("La descripción del rol no puede estar vacía.");
        }
    }
}
