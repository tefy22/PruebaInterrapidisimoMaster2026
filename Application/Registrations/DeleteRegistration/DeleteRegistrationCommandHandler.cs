using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Registrations;
using Domain.Subjects;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Registrations.UpdateRegistration
{
    internal sealed class DeleteRegistrationCommandHandler : ICommandHandler<DeleteRegistrationCommand>
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteRegistrationCommandHandler(IRegistrationRepository registrationRepository, IUnitOfWork unitOfWork)
        {
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _registrationRepository.Delete(request.id);

                if (result.IsFailure)
                    return Result.Failure(RegistrationErrors.DeleteError);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure<Guid>(RegistrationErrors.DeleteError);
            }
        }
    }
}
