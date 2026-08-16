using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.SearchRegistration
{
    internal sealed class GetSharedSubjectsQueryHandler : ICommandHandler<GetSharedSubjectsQuery, IReadOnlyList<SharedSubjectDto>>
    {
        private readonly IRegistrationRepository _registrationRepository;

        public GetSharedSubjectsQueryHandler(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<Result<IReadOnlyList<SharedSubjectDto>>> Handle(GetSharedSubjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<IReadOnlyList<SharedSubjectDto>>(Error.NullValue);

                if (request.StudentId == Guid.Empty)
                    return Result.Failure<IReadOnlyList<SharedSubjectDto>>(RegistrationErrors.Empty);

                var sharedSubjects = await _registrationRepository.GetSharedSubjectsWithPeerNamesAsync(request.StudentId, cancellationToken);

                if (sharedSubjects is null || !sharedSubjects.Any())
                    return Result.Success<IReadOnlyList<SharedSubjectDto>>(Array.Empty<SharedSubjectDto>());

                return Result.Success<IReadOnlyList<SharedSubjectDto>>(sharedSubjects);
            }
            catch (Exception)
            {
                return Result.Failure<IReadOnlyList<SharedSubjectDto>>(RegistrationErrors.SearchError);
            }
        }
    }
}
