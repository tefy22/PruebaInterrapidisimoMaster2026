using Domain.Abstractions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public interface IUserRepository
    {
        Task<bool> IsUserExists(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetStudentsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetStudentsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetTeachersAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
        void Add(User user);
        Task<Result> Update(User user);
        Task<Result> Delete(Guid id);
    }
}
