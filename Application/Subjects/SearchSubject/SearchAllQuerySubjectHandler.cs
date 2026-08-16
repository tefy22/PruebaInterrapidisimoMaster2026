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
    internal sealed class SearchAllQuerySubjectHandler : ICommandHandler<SearchAllSubjectQuery, IReadOnlyList<SubjectDto>>
    {
        private readonly ISubjectRepository _subjectRepository;

        public SearchAllQuerySubjectHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<Result<IReadOnlyList<SubjectDto>>> Handle(SearchAllSubjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subjects = await _subjectRepository.GetAllAsync(cancellationToken);

                var dtos = subjects.Select(s => new SubjectDto(
                    id: s.Id,
                    name: s.Name.Value,
                    credits: s.Credits.Value,
                    idUser: s.UserId,
                    userName: s.User?.Name?.Value + " " + s.User?.LastName?.Value ?? "Sin Profesor",
                    estado: (int)s.Status
                )).ToList();

                return Result.Success<IReadOnlyList<SubjectDto>>(dtos);
            }
            catch (Exception)
            {
                return Result.Failure<IReadOnlyList<SubjectDto>>(SubjectErrors.SearchError);
            }
        }
    }
}
