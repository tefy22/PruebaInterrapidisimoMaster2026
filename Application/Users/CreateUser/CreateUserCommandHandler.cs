using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Roles;
using Domain.User;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace Application.Users.CreateUser
{
    internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return Result.Failure<Guid>(Error.NullValue);

                var emailResult = Email.Create(request.email);
                var passwordResult = Password.Validate(request.password);
                var dniResult = DNI.Create(request.dni);
                var nameResult = Name.Create(request.name);
                var lastNameResult = LastName.Create(request.lastName);
                var phoneNumberResult = PhoneNumber.Create(request.phoneNumber);

                if (dniResult.IsFailure)
                    return Result.Failure<Guid>(dniResult.Error);

                if (nameResult.IsFailure)
                    return Result.Failure<Guid>(nameResult.Error);

                if (lastNameResult.IsFailure)
                    return Result.Failure<Guid>(lastNameResult.Error);

                if (emailResult.IsFailure)
                    return Result.Failure<Guid>(emailResult.Error);

                if (passwordResult.IsFailure)
                    return Result.Failure<Guid>(passwordResult.Error);

                if (phoneNumberResult.IsFailure)
                    return Result.Failure<Guid>(phoneNumberResult.Error);

                var existEmail = await _userRepository.IsUserExists(emailResult.Value, cancellationToken);
                if (existEmail)
                    return Result.Failure<Guid>(UserErrors.ExistsEmail);

                var existDni = await _userRepository.GetUserByDNIAsync(dniResult.Value.Value, cancellationToken);
                if (existDni is not null)
                    return Result.Failure<Guid>(UserErrors.ExistsDNI);

                var passHash = BCrypt.Net.BCrypt.HashPassword(passwordResult.Value.Value);
                var passHashed = Password.CreateFromHash(passHash);                               

                var userResult = User.Create(
                    dniResult.Value,
                    nameResult.Value,
                    lastNameResult.Value,
                    emailResult.Value,
                    passHashed.Value,
                    phoneNumberResult.Value,
                    request.roleId
                );
                if (userResult.IsFailure)
                    return Result.Failure<Guid>(userResult.Error);

                _userRepository.Add(userResult.Value);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(userResult.Value.Id);
            }
            catch (Exception)
            {
                return Result.Failure<Guid>(UserErrors.CreateError);
            }
        }
    }

}
