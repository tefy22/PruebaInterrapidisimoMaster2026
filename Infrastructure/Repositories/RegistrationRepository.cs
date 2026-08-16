using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal sealed class RegistrationRepository : Repository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
        public async Task<Result> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Failure(RegistrationErrors.Empty);

            try
            {
                var regExist = await GetByIdAsync(id, CancellationToken.None);
                if (regExist is null)
                    return Result.Failure(RegistrationErrors.NotFound);

                _dbContext.Set<Registration>().Remove(regExist);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(RegistrationErrors.DeleteError);
            }
        }

        public async Task<Registration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Registration>()
                                   .Include(r => r.Details)
                                   .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Registration>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Registration>()
                                   .Include(r => r.Details)
                                   .ToListAsync(cancellationToken);
        }

        public async Task<Registration?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Registration>()
                                   .Include(r => r.Details)
                                   .FirstOrDefaultAsync(r => r.StudentId == studentId, cancellationToken);
        }

        public Task<Registration?> GetByStatus(CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<Registration>().FirstOrDefaultAsync(r => r.Status == StatusRegistrationDetails.EnCurso, cancellationToken);
        }

        public async Task<IReadOnlyList<SharedSubjectDto>> GetSharedSubjectsWithPeerNamesAsync(Guid studentId, CancellationToken cancellationToken)
        {
            if (studentId == Guid.Empty)
                return Array.Empty<SharedSubjectDto>();

            // Ajusta el nombre del procedimiento si es distinto en la BD
            const string storedProcName = "GetAvailableSubjectsForStudent";

            var param = new SqlParameter("@idStudent", SqlDbType.UniqueIdentifier) { Value = studentId };

            // Reemplaza la línea problemática por la siguiente, usando ExecuteSqlQuery en lugar de SqlQuery
            var query = _dbContext.Set<SharedSubjectDto>().FromSqlRaw($"EXEC {storedProcName} @idStudent", param);

            // ToListAsync acepta CancellationToken
            var list = await query.ToListAsync(cancellationToken);

            return list;
        }

        public async Task<bool> HasRegistrationDetailsForSubjectAsync(Guid subjectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Registration>()
                .SelectMany(r => r.Details)
                .AnyAsync(d => d.SubjectId == subjectId, cancellationToken);
        }
    }
}
