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
    internal sealed class SearchAllRolQueryHandler : ICommandHandler<SearchAllRolQuery, IReadOnlyList<RolDto>>
    {
        private readonly IRoleRepository _rolRepository;

        public SearchAllRolQueryHandler(IRoleRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public async Task<Result<IReadOnlyList<RolDto>>> Handle(SearchAllRolQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _rolRepository.GetAllAsync(cancellationToken);
                var dtos = roles.Select(r => new RolDto(r.Id, r.Description.ToString()))
                                .ToList();
                return Result.Success<IReadOnlyList<RolDto>>(dtos);
            }
            catch (Exception)
            {
                return Result.Failure<IReadOnlyList<RolDto>>(RolErrors.SearchError);
            }
        }
    }
}
