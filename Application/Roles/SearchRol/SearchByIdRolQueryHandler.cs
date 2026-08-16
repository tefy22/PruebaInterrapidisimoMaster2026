using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Roles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.SearchRol
{
    internal sealed class SearchByIdRolQueryHandler : ICommandHandler<SearchByIdRolQuery, RolDto>
    {
        private readonly IRoleRepository _rolRepository;

        public SearchByIdRolQueryHandler(IRoleRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public async Task<Result<RolDto>> Handle(SearchByIdRolQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rol = await _rolRepository.GetByIdAsync(request.id, cancellationToken);

                if (rol is null)
                    return Result.Failure<RolDto>(RolErrors.NotFound);

                var dto = new RolDto(rol.Id, rol.Description.ToString());
                return Result.Success(dto);

            }
            catch (Exception)
            {
                return Result.Failure<RolDto>(RolErrors.SearchError);
            }
        }
    }
}
