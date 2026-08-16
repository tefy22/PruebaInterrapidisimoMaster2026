using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.DeleteUser
{
    internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUserRepository userRepository, IRegistrationRepository registrationRepository, ISubjectRepository subjectRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _registrationRepository = registrationRepository;
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.id, cancellationToken);
                if (user is null)
                    return Result.Failure(UserErrors.UserNotFound);

                // Validar que el usuario no tenga un registro de matrícula en curso
                var existingRegistration = await _registrationRepository.GetByStudentIdAsync(request.id, cancellationToken);
                if (existingRegistration is not null)
                    return Result.Failure(RegistrationErrors.AlreadyExists);

                // Validar si el usuario es profesor: comprobar materias asignadas
                var subjectsForTeacher = await _subjectRepository.GetSubjectForTeacher(request.id, cancellationToken);
                if (subjectsForTeacher is not null && subjectsForTeacher.Any())
                {
                    // Si alguna materia tiene inscripciones en curso, devolver error de registro
                    foreach (var subj in subjectsForTeacher)
                    {
                        var hasDetails = await _registrationRepository.HasRegistrationDetailsForSubjectAsync(subj.Id, cancellationToken);
                        if (hasDetails)
                            return Result.Failure(RegistrationErrors.AlreadyExists);
                    }

                    // Tiene materias asignadas (aunque sin inscripciones) -> no se permite eliminar el profesor
                    return Result.Failure(UserErrors.CannotDeleteTeacherWithSubjects);
                }

                var result = await _userRepository.Delete(request.id);
                if (result.IsFailure)
                    return Result.Failure(UserErrors.DeleteError);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(result);
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.DeleteError);
            }
        }
    }
}
