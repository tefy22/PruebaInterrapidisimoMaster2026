using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.User;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.SearchUser
{
    internal sealed class SearchStudentByIdQueryHandler : ICommandHandler<SearchStudentByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public SearchStudentByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(SearchStudentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetStudentsByIdAsync(request.Id, cancellationToken);

                if (user is null)
                    return Result.Failure<UserDto>(UserErrors.StudentNotFound);

                var dto = new UserDto(
                    Id: user.Id,
                    DNI: user.DNId.Value,
                    Name: user.Name.Value,
                    LastName: user.LastName.Value,
                    Email: user.Email.Value,
                    PhoneNumber: user.PhoneNumber.Value,
                    RoleId: user.RolId,
                    Status: (int)user.Status
                );

                return Result.Success(dto);
            }
            catch (Exception)
            {
                return Result.Failure<UserDto>(UserErrors.SearchError);
            }
        }
    }
}