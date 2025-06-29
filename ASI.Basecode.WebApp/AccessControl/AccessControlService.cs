
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl
{
    public class AccessControlService : IAccessControlInterface
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccessControlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> CheckAdminAccess()
        {
            string userRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckUserAccess()
        {
            string userRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "User")
            {
                return false;
            }
            return true;
        }
    }
}
