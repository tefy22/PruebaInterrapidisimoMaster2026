using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.User;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.CreateRegistration
{
    internal sealed class CreateRegistrationCommandHandler : ICommandHandler<CreateRegistrationCommand, Guid>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRegistrationCommandHandler(IRegistrationRepository registrationRepository, ISubjectRepository subjectRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _registrationRepository = registrationRepository;
            _subjectRepository = subjectRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                if (request.SubjectId is null || !request.SubjectId.Any())
                    return Result.Failure<Guid>(RegistrationErrors.EmptySubjects);

                var existingRegistration = await _registrationRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);
                if (existingRegistration is not null) 
                    return Result.Failure<Guid>(RegistrationErrors.AlreadyExists);

                // Verificar estudiante existe
                var student = await _userRepository.GetStudentsByIdAsync(request.StudentId, cancellationToken);
                if (student is null)
                    return Result.Failure<Guid>(UserErrors.StudentNotFound);                

                // Obtener las materias seleccionadas de un solo golpe (Evita hilos concurrentes en EF)
                var subjects = (await _subjectRepository.GetByIdRegistrationAsync(request.SubjectId, cancellationToken)).ToList();
                
                if (subjects.Count != request.SubjectId.Count)
                    return Result.Failure<Guid>(SubjectErrors.NotFound);

                // Regla 1: máximo 3 materias
                if (request.SubjectId.Count > 3)
                    return Result.Failure<Guid>(RegistrationErrors.MaxSubjects);

                // Regla 2: no puede tener clases con el mismo profesor
                var selectedTeacherIds = new HashSet<Guid>();
                foreach (var s in subjects)
                {
                    if (!selectedTeacherIds.Add(s.UserId))
                    {
                        // La misma cátedra/profesor aparece en las materias seleccionadas -> rechazo
                        return Result.Failure<Guid>(RegistrationErrors.SameTeacherInSelection);
                    }
                }                

                // Crear registro de dominio y persistir
                var registration = Registration.Create(request.StudentId, request.SubjectId, request.Status);
                if(registration.IsFailure)
                    return Result.Failure<Guid>(registration.Error);

                _registrationRepository.Add(registration.Value);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(registration.Value.Id);

            }
            catch (Exception)
            {
                return Result.Failure<Guid>(RegistrationErrors.CreateError);
            }
        }
    }
}
