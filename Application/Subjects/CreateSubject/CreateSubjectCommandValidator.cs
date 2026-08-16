using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.CreateSubject
{
    public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
    {
        public CreateSubjectCommandValidator()
        {
            RuleFor(c => c.name).NotEmpty().WithMessage("El nombre de la materia no puede ser vacio");
            RuleFor(c => c.credits).NotEmpty().WithMessage("Los creditos no pueden ser vacios");
            RuleFor(c => c.idUser).NotEmpty().WithMessage("El profesor no puede ser vacio");
        }
    }
}
