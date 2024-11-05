using Microsoft.AspNetCore.Http;

namespace PBTPro.DAL.Services
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserResolverService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUser()
        {
            return _httpContext.HttpContext.User?.Identity?.Name ?? "unknown_user";
        }

    }
}
