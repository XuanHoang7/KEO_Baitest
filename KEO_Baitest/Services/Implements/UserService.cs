using KEO_Baitest.Services.Interfaces;
using System.Security.Claims;

namespace KiemTraThuViec1.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUser()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null;
            }

            return identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
            //var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;

            //return new User
            //{
            //    Id = userId,
            //    Name = userName,
            //    Email = userEmail
            //};
        }
    }
}
