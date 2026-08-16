using Domain.Abstractions;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal sealed class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Role>().ToListAsync();
        }
        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (!Enum.TryParse<RolesDetails>(name, out var roleDetail))
                return null;

            return await _dbContext.Set<Role>().FirstOrDefaultAsync(x => x.Description == roleDetail, cancellationToken);
        }
        public async Task<Result> Update(Role rol)
        {
            try
            {
                var existing = await _dbContext.Set<Role>().FirstOrDefaultAsync(r => r.Id == rol.Id);
                if (existing is null)
                    return Result.Failure(RolErrors.NotFound);

                // Actualiza los valores de la entidad rastreada
                _dbContext.Entry(existing).CurrentValues.SetValues(rol);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(RolErrors.UpdateError);
            }
        }

        public async Task<Result> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return Result.Failure(RolErrors.Empty);

            try
            {
                // Validar existencia usando el método asincrónico del repositorio
                var rolExist = GetByIdAsync(id, CancellationToken.None).GetAwaiter().GetResult();
                if (rolExist is null)
                    return Result.Failure(RolErrors.NotFound);

                _dbContext.Set<Role>().Remove(rolExist);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(RolErrors.DeleteError);
            }
        }
    }
}
