using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.SearchSubject
{
    internal sealed class SearchByIdSubjectQueryHandler : ICommandHandler<SearchByIdSubjectQuery, SubjectDto>
    {
        private readonly ISubjectRepository _subjectRepository;

        public SearchByIdSubjectQueryHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<Result<SubjectDto>> Handle(SearchByIdSubjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(request.id, cancellationToken);
                if (subject is null)
                    return Result.Failure<SubjectDto>(SubjectErrors.NotFound);

                var dto = new SubjectDto(subject.Id, subject.Name.Value, subject.Credits.Value,  subject.UserId, subject.User?.Name?.Value + " " + subject.User?.LastName?.Value ?? "Sin Profesor", (int)subject.Status);
                
                return Result.Success(dto);
            }
            catch (Exception)
            {
                return Result.Failure<SubjectDto>(SubjectErrors.SearchError);
            }
        }
    }
}
