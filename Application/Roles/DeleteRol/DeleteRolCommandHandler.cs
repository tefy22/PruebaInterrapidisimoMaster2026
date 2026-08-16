using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.DeleteRol
{
    internal sealed class DeleteRolCommandHandler : ICommandHandler<DeleteRolCommand>
    {
        private readonly IRoleRepository _rolRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteRolCommandHandler(IRoleRepository rolRepository, IUnitOfWork unitOfWork)
        {
            _rolRepository = rolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteRolCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existRol = await _rolRepository.GetByIdAsync(request.id, cancellationToken);

                if (existRol is null)
                    return Result.Failure(RolErrors.NotFound); 

                var deleteResult = await _rolRepository.Delete(request.id);

                if (deleteResult.IsFailure)
                    return Result.Failure(deleteResult.Error);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(deleteResult);
            }
            catch (Exception)
            {
                return Result.Failure(RolErrors.DeleteError);
            }
        }
    }
}
