using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.SearchUser
{
    internal sealed class SearchAllTeachersQueryHandler : ICommandHandler<SearchTeachersQuery, IReadOnlyList<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public SearchAllTeachersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IReadOnlyList<UserDto>>> Handle(SearchTeachersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetTeachersAsync(cancellationToken);

                if (users is null || !users.Any())
                    return Result.Success<IReadOnlyList<UserDto>>(Array.Empty<UserDto>());

                var dtos = users.Select(u => new UserDto(
                    Id: u.Id,
                    DNI: u.DNId.Value,
                    Name: u.Name.Value,
                    LastName: u.LastName.Value,
                    Email: u.Email.Value,
                    PhoneNumber: u.PhoneNumber.Value,
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