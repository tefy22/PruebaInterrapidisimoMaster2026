using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Registration?> GetByStatus(CancellationToken cancellationToken = default);
        Task<Registration?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Registration>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SharedSubjectDto>> GetSharedSubjectsWithPeerNamesAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<bool> HasRegistrationDetailsForSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);

        void Add(Registration registration);
        Task<Result> Delete(Guid id);
    }
}
