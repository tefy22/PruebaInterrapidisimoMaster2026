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

namespace Application.Registrations.SearchRegistration
{
    internal sealed class SearchAllRegistrationQueryHandler : ICommandHandler<SearchAllRegistrationQuery, IReadOnlyList<RegistrationDto>>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUserRepository _userRepository;

        public SearchAllRegistrationQueryHandler(IRegistrationRepository registrationRepository, ISubjectRepository subjectRepository, IUserRepository userRepository)
        {
            _registrationRepository = registrationRepository;
            _subjectRepository = subjectRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IReadOnlyList<RegistrationDto>>> Handle(SearchAllRegistrationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var registrations = await _registrationRepository.GetAllAsync(cancellationToken);

                if (registrations is null || !registrations.Any())
                    return Result.Success<IReadOnlyList<RegistrationDto>>(Array.Empty<RegistrationDto>());

                // Recolectar ids relevantes
                var allStudentIds = registrations.Select(r => r.StudentId).Distinct().ToList();
                var allSubjectIds = registrations.SelectMany(r => r.Details.Select(d => d.SubjectId)).Distinct().ToList();

                // Obtener materias en bloque (método disponible)
                var subjects = allSubjectIds.Any()
                    ? (await _subjectRepository.GetByIdRegistrationAsync(allSubjectIds, cancellationToken)).ToDictionary(s => s.Id)
                    : new Dictionary<Guid, Subject>();

                // Obtener profesores necesarios (no hay método de GetByIds, obtenemos todos y filtramos)
                var teacherIds = subjects.Values.Select(s => s.UserId).Distinct().ToList();
                var teachers = (await _userRepository.GetTeachersAsync(cancellationToken))
                                .Where(t => teacherIds.Contains(t.Id))
                                .ToDictionary(t => t.Id);

                // Obtener estudiantes (no hay método GetByIds, usamos GetAllAsync y filtramos)
                var students = (await _userRepository.GetStudentsAsync(cancellationToken))
                                .Where(s => allStudentIds.Contains(s.Id))
                                .ToDictionary(s => s.Id);

                var resultDtos = new List<RegistrationDto>();

                foreach (var reg in registrations)
                {
                    // Student name
                    var studentName = students.TryGetValue(reg.StudentId, out var st)
                        ? $"{st.Name.Value} {st.LastName.Value}"
                        : string.Empty;

                    var detailDtos = reg.Details.Select(d =>
                    {
                        subjects.TryGetValue(d.SubjectId, out var subj);

                        var subjectName = subj?.Name.Value ?? string.Empty;
                        var credits = subj?.Credits.Value ?? 0;
                        var teacherId = subj?.UserId ?? Guid.Empty;

                        var teacherName = Guid.Empty.Equals(teacherId) ? string.Empty :
                            (teachers.TryGetValue(teacherId, out var tch) ? $"{tch.Name.Value} {tch.LastName.Value}" : string.Empty);

                        var rating = d.Rating?.Value ?? 0;

                        return new RegistrationDetailDto(
                            id: d.Id,
                            subjectId: d.SubjectId,
                            subjectName: subjectName,
                            credits: credits,
                            rating: rating,
                            teacherId: teacherId,
                            teacherName: teacherName
                        );
                    }).ToList();

                    var regDto = new RegistrationDto(
                        id: reg.Id,
                        studentId: reg.StudentId,
                        studentName: studentName,
                        status: (int)reg.Status,
                        details: detailDtos
                    );

                    resultDtos.Add(regDto);
                }

                return Result.Success<IReadOnlyList<RegistrationDto>>(resultDtos);
            }
            catch (Exception)
            {
                return Result.Failure<IReadOnlyList<RegistrationDto>>(RegistrationErrors.SearchError);
            }
        }
    }
}
