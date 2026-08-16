using Domain.Abstractions;
using Domain.Roles;
using Domain.User;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Result> Delete(Guid id)
        {
            try
            {
                var existing = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == id);
                if (existing is null)
                    return Result.Failure(UserErrors.UserNotFound);

                _dbContext.Set<User>().Remove(existing);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.DeleteError);
            }
        }
        

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>().ToListAsync(cancellationToken);        
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetStudentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                .Where(u =>
                    u.Status == StatusDetails.Active &&
                    _dbContext.Set<Role>().Any(r => r.Id == u.RolId && r.Description == RolesDetails.Student))
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetStudentsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                 .Where(u =>
                     u.Id == id &&
                     u.Status == StatusDetails.Active &&
                     _dbContext.Set<Role>().Any(r => r.Id == u.RolId && r.Description == RolesDetails.Student))
                 .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetTeachersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<User>()
                .Where(u =>
                    u.Status == StatusDetails.Active &&
                    _dbContext.Set<Role>().Any(r => r.Id == u.RolId && r.Description == RolesDetails.Teacher))
                .ToListAsync(cancellationToken);
        }

        public Task<bool> IsUserExists(Email email, CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<User>().AnyAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<Result> Update(User user)
        {
            try
            {
                var existing = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existing is null)
                    return Result.Failure(UserErrors.UserNotFound);

                _dbContext.Entry(existing).CurrentValues.SetValues(user);

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(UserErrors.UpdateError);
            }
        }
    }
}