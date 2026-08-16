using Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace UniversityMaster.OptionsSetup
{
    public class JwtOptionSetup : IConfigureOptions<JwtOptions>
    {
        private const string sectionName = "Jwt";
        private readonly IConfiguration _configuration;

        public JwtOptionSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection(sectionName).Bind(options);
        }
    }
}
