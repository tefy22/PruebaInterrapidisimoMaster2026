using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.User;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.LoginUser
{
    internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var emailResult = Email.Create(request.email);
            if (emailResult.IsFailure)
                return Result.Failure<string>(emailResult.Error);
            
            var user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (user is null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            if(!BCrypt.Net.BCrypt.Verify(request.password, user.Password!.Value))
                return Result.Failure<string>(UserErrors.PasswordInvalid);

            var token = await _jwtProvider.GenerateTokenAsync(user);

            return token;
        }
    }
}
