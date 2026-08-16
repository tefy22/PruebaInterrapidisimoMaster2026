using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Authentication
{
    public interface IJwtProvider
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
