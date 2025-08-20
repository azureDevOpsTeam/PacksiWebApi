using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class IdentityService(IConfiguration iConfiguration,
                                 ILogger<IdentityService> logger,
                                 IHttpContextAccessor httpContextAccessor,
                                 IRepository<Role> roleRepository,
                                 IRepository<UserRole> userRoleRepository) : IIdentityService
    {
        private readonly IRepository<Role> _roleRepository = roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository = userRoleRepository;
        private readonly IConfiguration _iConfiguration = iConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<IdentityService> _logger = logger;

        public ServiceResult AuthenticateOneTimePassword(SignInDto signInViewModel, UserAccount userAccount)
        {
            if (signInViewModel.SecurityCode != userAccount.SecurityCode || DateTime.Now > userAccount.ExpireSecurityCode)
            {
                return new ServiceResult
                {
                    RequestStatus = RequestStatus.NotFound,
                    Data = signInViewModel,
                    Message = IdentityMessages.IncorrectSecurityCode
                };
            }

            AuthorizeResultDto result = new()
            {
                UserFullName = signInViewModel.UserName
            };

            return new ServiceResult
            {
                RequestStatus = RequestStatus.Successful,
                Data = result,
                Message = CommonMessages.Successful
            };
        }

        public async Task<ServiceResult> AuthenticateUserInformationAsync(SignInDto signInViewModel, UserAccount userAccount)
        {
            var passwordIsValid = HashGenerator.VerifyPassword(signInViewModel.Password, userAccount.Password, userAccount.SecurityStamp);
            if (!passwordIsValid)
                return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = IdentityMessages.IncorrectPassword };

            var roles = await _userRoleRepository.Query()
                .Where(ur => ur.UserAccountId == userAccount.Id)
                .Join(_roleRepository.Query(), ur => ur.RoleId, r => r.Id, (ur, r) => r.RoleName)
                .ToListAsync();

            var (accessToken, tokenId) = TokenGenerator(userAccount.UserName, userAccount.Id, roles);

            AuthorizeResultDto result = new()
            {
                AccessTokens = accessToken,
                TokenId = tokenId,
                UserFullName = userAccount.UserName
            };

            return new ServiceResult
            {
                RequestStatus = RequestStatus.Successful,
                Data = result,
                Message = CommonMessages.Successful
            };
        }

        public (string jwtToken, string tokenId) TokenGenerator(string userName, int userId, List<string> roles)
        {
            if (_iConfiguration == null) throw new ArgumentNullException(nameof(_iConfiguration));
            if (_httpContextAccessor == null) throw new ArgumentNullException(nameof(_httpContextAccessor));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:Key"]);

            if (tokenKey.Length < 32) throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_iConfiguration["JWT:JwtTokenExpirationTimeInMinutes"])),
                Issuer = _iConfiguration["JWT:Issuer"],
                Audience = _iConfiguration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return (jwtToken, token.Id);
        }

        public async Task<(string jwtToken, string tokenId)> TokenRequestGeneratorAsync(string userName, int userAccountId)
        {
            if (_iConfiguration == null) throw new ArgumentNullException(nameof(_iConfiguration));
            if (_httpContextAccessor == null) throw new ArgumentNullException(nameof(_httpContextAccessor));

            var roles = await _userRoleRepository.Query()
                .Where(ur => ur.UserAccountId == userAccountId)
                .Join(_roleRepository.Query(), ur => ur.RoleId, r => r.Id, (ur, r) => r.RoleName)
                .ToListAsync();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_iConfiguration["JWT:Key"]);

            if (tokenKey.Length < 32) throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.NameIdentifier, userAccountId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_iConfiguration["JWT:JwtTokenExpirationTimeInMinutes"])),
                Issuer = _iConfiguration["JWT:Issuer"],
                Audience = _iConfiguration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return (jwtToken, token.Id);
        }
    }
}