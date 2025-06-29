using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl.AdminAccessControl
{
    public class AdminAccessService :IAdminAccessInterface
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminAccessService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> CheckAccess()
        {
            string userRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
            {
                return false;
            }
            return true;
        }
    }
}
