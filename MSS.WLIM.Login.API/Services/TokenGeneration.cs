using Microsoft.IdentityModel.Tokens;
using MSS.WLIM.DataServices.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MSS.WLIM.Login.API.Services
{
    public class TokenGeneration
    {
        private IConfiguration _config;
        private ILogger<TokenGeneration> _logger;
        private IUserLoginRepository _userLoginRepository;
        public TokenGeneration(IConfiguration config, ILogger<TokenGeneration> logger, IUserLoginRepository userLoginRepository)
        {
            _config = config;
            _logger = logger;
            _userLoginRepository = userLoginRepository;
        }
        public class AuthResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

        public async Task<AuthResponse> Validate(string emailId, string password)
        {
            AuthResponse authResponse = null;
            bool isValidUser = await _userLoginRepository.Validate(emailId, password);

            if (isValidUser)
            {
                string role = await _userLoginRepository.GetUserRole(emailId);
                string userName = await _userLoginRepository.GetUserName(emailId);
                string token = GenerateToken(emailId, role, userName);
                authResponse = new AuthResponse
                {
                    Token = token,
                    Role = role
                };
            }

            return authResponse;
        }


        private string GenerateToken(string emailId, string role, string userName)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, emailId),
                    new Claim(ClaimTypes.Role, role), // Add the user's role to the claims
                    new Claim("UserName", userName) // Custom claim for employee name
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
