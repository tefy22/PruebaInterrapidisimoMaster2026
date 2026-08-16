using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.CreateRol
{
    internal sealed class CreateRolCommandHanlder : ICommandHandler<CreateRolCommand, Guid>
    {
        private readonly IRoleRepository _rolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRolCommandHanlder(IRoleRepository rolRepository, IUnitOfWork unitOfWork)
        {
            _rolRepository = rolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateRolCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                // Validar y convertir la descripción a enum RolesDetails
                if (!Enum.TryParse<RolesDetails>(request.Description, ignoreCase: true, out var descriptionEnum))
                    return Result.Failure<Guid>(RolErrors.InvalidDescription);                

                // Verificar existencia por nombre (evitar duplicados)
                var exists = await _rolRepository.GetByNameAsync(descriptionEnum.ToString(), cancellationToken);
                if (exists is not null)
                    return Result.Failure<Guid>(RolErrors.Exists);
                
                var rolResult = Role.Create(descriptionEnum);
                if (!rolResult.IsSuccess)
                    return Result.Failure<Guid>(rolResult.Error);
                

                var rol = rolResult.Value;

                _rolRepository.Add(rolResult.Value);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(rol.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<Guid>(RolErrors.CreateError);
            }
        }
    }
}
