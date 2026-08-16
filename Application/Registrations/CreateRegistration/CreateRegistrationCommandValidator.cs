using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.CreateRegistration
{
    public class CreateRegistrationCommandValidator : AbstractValidator<CreateRegistrationCommand>
    {
        public CreateRegistrationCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("El Id del estudiante es requerido.");
            RuleFor(x => x.SubjectId)
                .NotEmpty().WithMessage("El Id de la materia es requerido.");
        }
    }
}
