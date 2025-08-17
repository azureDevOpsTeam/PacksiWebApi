using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InfrastructureLayer.Extensions
{
    [InjectAsScoped]
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public int? UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userId, out var id) ? id : null;
            }
        }

        public string UserIpAddress =>
            _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public string UserFirstName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

        public string UserLastName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;

        public string UserFullName => $"{UserFirstName} {UserLastName}".Trim();

        public string UserDisplayName => !string.IsNullOrWhiteSpace(UserFullName) ? UserFullName : UserName;

        public string UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    }
}