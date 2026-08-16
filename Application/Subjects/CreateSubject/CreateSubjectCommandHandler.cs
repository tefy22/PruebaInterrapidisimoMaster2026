using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Subjects;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.CreateSubject
{
    internal sealed class CreateSubjectCommandHandler : ICommandHandler<CreateSubjectCommand, Guid>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubjectCommandHandler(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                var name = Name.Create(request.name);
                var credits = Credits.Create(request.credits);
                var idTeacher = request.idUser;

                if (name.IsFailure)
                    return Result.Failure<Guid>(name.Error);

                if(credits.IsFailure)
                    return Result.Failure<Guid>(credits.Error);

                var creditsTeacher = await _subjectRepository.GetSubjectForTeacher(idTeacher, cancellationToken);
                if (creditsTeacher.Count >= 2)
                    return Result.Failure<Guid>(SubjectErrors.CreditsTeacherComplete);

                var nameRpt = await _subjectRepository.GetByNameAsync(name.Value.Value, cancellationToken);
                if(nameRpt is not null)
                    return Result.Failure<Guid>(SubjectErrors.RepeatName);

                var subjectResult = Subject.Create(name.Value, credits.Value, idTeacher);
                if(subjectResult.IsFailure)
                    return Result.Failure<Guid>(subjectResult.Error);

                _subjectRepository.Add(subjectResult.Value);
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
