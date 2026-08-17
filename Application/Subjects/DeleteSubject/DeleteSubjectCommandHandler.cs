using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.DeleteSubject
{
    internal sealed class DeleteSubjectCommandHandler : ICommandHandler<DeleteSubjectCommand>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubjectCommandHandler(ISubjectRepository subjectRepository, IRegistrationRepository registrationRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(request.id, cancellationToken);
                if (subject is null)
                    return Result.Failure(SubjectErrors.NotFound);

                var hasRegistrations = await _registrationRepository.HasRegistrationDetailsForSubjectAsync(request.id, cancellationToken);
                if (hasRegistrations)
                    return Result.Failure(SubjectErrors.CannotDeleteSubjectWithRegistrations);

                var result = await _subjectRepository.Delete(request.id);
                if (result.IsFailure)
                    return Result.Failure(SubjectErrors.DeleteError);

                await _unitOfWork.SaveChangesAsync();
                return Result.Success(result);
            }
            catch (Exception)
            {
                return Result.Failure(SubjectErrors.DeleteError);
            }
        }
    }
}
