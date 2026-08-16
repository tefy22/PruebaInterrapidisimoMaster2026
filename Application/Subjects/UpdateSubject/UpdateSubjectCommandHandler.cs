using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.UpdateSubject
{
    internal sealed class UpdateSubjectCommandHandler : ICommandHandler<UpdateSubjectCommand, Guid>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSubjectCommandHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, IRegistrationRepository registrationRepository)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
            _registrationRepository = registrationRepository;
        }

        public async Task<Result<Guid>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                var name = Name.Create(request.name);
                var credits = Credits.Create(request.credits);
                var idTeacher = request.idTeacher;
                var statusResult = request.estado;

                if (name.IsFailure)
                    return Result.Failure<Guid>(name.Error);

                if (credits.IsFailure)
                    return Result.Failure<Guid>(credits.Error);

                if (statusResult < 0)
                    return Result.Failure<Guid>(Error.NullValue);

                if (request.estado == 1)
                {
                    var creditsTeacher = await _subjectRepository.GetSubjectForTeacher(idTeacher, cancellationToken);
                    if (creditsTeacher.Count >= 2)
                        return Result.Failure<Guid>(SubjectErrors.CreditsTeacherComplete);
                }
                else
                {
                    var hasRegistrations = await _registrationRepository.HasRegistrationDetailsForSubjectAsync(request.id, cancellationToken);
                    if (hasRegistrations)
                        return Result.Failure<Guid>(SubjectErrors.CannotDeleteSubjectWithRegistrations);
                }

                var subjectResult = Subject.Update(request.id, name.Value, credits.Value, idTeacher, (StatusDetails)statusResult);
                if (subjectResult.IsFailure)
                    return Result.Failure<Guid>(subjectResult.Error);

                var updateResult = await _subjectRepository.Update(subjectResult.Value);
                if (updateResult.IsFailure)
                    return Result.Failure<Guid>(updateResult.Error);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(subjectResult.Value.Id);
            }
            catch (Exception)
            {
                return Result.Failure<Guid>(SubjectErrors.CreateError);
            }
        }
       
    }
}
