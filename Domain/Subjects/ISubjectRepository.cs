using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Subjects
{
    public interface ISubjectRepository
    {
        Task<IReadOnlyList<Subject>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Subject>> GetByIdRegistrationAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);        
        Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Subject>> GetSubjectForTeacher(Guid idTeacher, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Subject>> GetOnlyActive(CancellationToken cancellationToken = default);
        void Add(Subject subject);
        Task<Result>Update(Subject subject);
        Task<Result> Delete(Guid id);

    }
}
