using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.SearchUser
{
    internal sealed class SearchAllUserQueryHandler : ICommandHandler<SearchAllUserQuery, IReadOnlyList<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public SearchAllUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IReadOnlyList<UserDto>>> Handle(SearchAllUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllAsync(cancellationToken);

                var dtos = users.Select(u => new UserDto(
                    Id: u.Id,
                    Name: u.Name.Value,
                    LastName: u.LastName.Value,
                    Email: u.Email.Value,
                    PhoneNumber: u.PhoneNumber.Value,
                    DNI: u.DNId.Value,
                    RoleId: u.RolId,
                    Status: (int)u.Status
                )).ToList();

                return Result.Success<IReadOnlyList<UserDto>>(dtos);
            }
            catch (Exception)
            {
                return Result.Failure<IReadOnlyList<UserDto>>(UserErrors.SearchError);
            }
        }
    }
}
