using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.User;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UpdateUser
{
    internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                // Validar ValueObjects
                var dniResult = DNI.Create(request.dni);
                var nameResult = Name.Create(request.name);
                var lastNameResult = LastName.Create(request.lastName);
                var emailResult = Email.Create(request.email);
                var phoneResult = PhoneNumber.Create(request.phoneNumber);
                var passwordResult = Password.Validate(request.password);

                if (dniResult.IsFailure) return Result.Failure<Guid>(dniResult.Error);
                if (nameResult.IsFailure) return Result.Failure<Guid>(nameResult.Error);
                if (lastNameResult.IsFailure) return Result.Failure<Guid>(lastNameResult.Error);
                if (emailResult.IsFailure) return Result.Failure<Guid>(emailResult.Error);
                if (phoneResult.IsFailure) return Result.Failure<Guid>(phoneResult.Error);
                if (passwordResult.IsFailure) return Result.Failure<Guid>(passwordResult.Error);

                // Obtener usuario existente
                var existing = await _userRepository.GetByIdAsync(request.id, cancellationToken);
                if (existing is null)
                    return Result.Failure<Guid>(UserErrors.UserNotFound);

                // Si cambia el email, comprobar unicidad
                if (!string.Equals(existing.Email.Value, emailResult.Value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = await _userRepository.IsUserExists(emailResult.Value, cancellationToken);
                    if (exists)
                        return Result.Failure<Guid>(UserErrors.ExistsEmail);
                }

                var passwordValue = existing.Password;
                if (!string.IsNullOrWhiteSpace(request.password))
                {
                    var passHash = BCrypt.Net.BCrypt.HashPassword(request.password);
                    var passFromHash = Password.CreateFromHash(passHash);
                    if (passFromHash.IsFailure)
                        return Result.Failure<Guid>(passFromHash.Error);

                    passwordValue = passFromHash.Value;
                }

                var updateResult = existing.Update(
                    dNId: dniResult.Value,
                    name: nameResult.Value,
                    lastName: lastNameResult.Value,
                    email: emailResult.Value,
                    password: passwordValue,
                    phoneNumber: phoneResult.Value,
                    status: (StatusDetails)request.status,
                    rolId: request.roleId
                );

                if (updateResult.IsFailure)
                    return Result.Failure<Guid>(updateResult.Error);

                var repoResult = await _userRepository.Update(existing);
                if (repoResult.IsFailure)
                    return Result.Failure<Guid>(repoResult.Error);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(existing.Id);
            }
            catch (Exception)
            {
                return Result.Failure<Guid>(UserErrors.UpdateError);
            }
        }
    }
}
