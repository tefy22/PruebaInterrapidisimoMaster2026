using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Subjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subjects.SearchSubject
{
    internal sealed class SearchActiveSubjectQueryHandler : ICommandHandler<SearchActiveSubjectQuery, IReadOnlyList<SubjectDto>>
    {
        private readonly ISubjectRepository _subjectRepository;

        public SearchActiveSubjectQueryHandler(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<Result<IReadOnlyList<SubjectDto>>> Handle(SearchActiveSubjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var subjects = await _subjectRepository.GetOnlyActive(cancellationToken);
                //var dtos = subjects.Select(s => new SubjectDto(
                //        id: s.Id,
                //        name: s.Name.Value,
                //        credits: s.Credits.Value,
                //        idTeacher: s.TeacherId,
                //        estado: (int)s.Status
                //    )).ToList();
                var dtos = subjects.Select(s => new SubjectDto(
                    id: s.Id,
                    name: s.Name.Value,
                    credits: s.Credits.Value,
                    idUser: s.UserId,
                    userName: s.User?.Name?.Value + " " + s.User?.LastName?.Value ?? "Sin Profesor", // Extraemos el nombre de la relación cargada
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
