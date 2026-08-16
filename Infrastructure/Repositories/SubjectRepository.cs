using Domain.Abstractions;
using Domain.Subjects;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal sealed class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext dbContext) : base(dbContext) { }
        public async Task<Result> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Failure(SubjectErrors.Empty);

            try
            {
                var subjectExist = await _dbContext.Set<Subject>().FirstOrDefaultAsync(s => s.Id == id);
                if (subjectExist is null)
                    return Result.Failure(SubjectErrors.NotFound);

                _dbContext.Set<Subject>().Remove(subjectExist);
                await _dbContext.SaveChangesAsync();
                return Result.Success();

            }
            catch (Exception)
            {
                return Result.Failure(SubjectErrors.DeleteError);
            }
        }

        public async Task<IReadOnlyList<Subject>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Subject>()
                                   .Include(s => s.User)
                                   .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Subject>> GetByIdRegistrationAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids as IList<Guid> ?? ids.ToList();

            if (!idList.Any())
                return Array.Empty<Subject>();

            var subjects = await _dbContext.Set<Subject>().Where(s => ids.Contains(s.Id) && (int)s.Status == 1).ToListAsync(cancellationToken);
            return subjects;
        }

        public async Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var result = await GetAllAsync(cancellationToken);
            return result.FirstOrDefault(s => s.Name.Value.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IReadOnlyList<Subject>> GetOnlyActive(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Subject>().Where(s => s.Status == StatusDetails.Active).ToListAsync();
        }

        public async Task<IReadOnlyList<Subject>> GetSubjectForTeacher(Guid idTeacher, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Subject>()
                                   .Include(s => s.User)
                                   .Where(s => s.UserId == idTeacher && s.Status == StatusDetails.Active)
                                   .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasAnySubjectTeacherAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Subject>().AnyAsync(s => s.UserId == userId, cancellationToken);
        }

        public async Task<Result> Update(Subject subject)
        {
            try
            {
                var exist = await _dbContext.Set<Subject>().FirstOrDefaultAsync(s => s.Id == subject.Id);

                if (exist is null)
                    return Result.Failure(SubjectErrors.NotFound);

                _dbContext.Entry(exist).CurrentValues.SetValues(subject);
                await _dbContext.SaveChangesAsync();
                return Result.Success();

            }
            catch (Exception)
            {
                return Result.Failure(SubjectErrors.UpdateError);
            }
        }
    }
}
