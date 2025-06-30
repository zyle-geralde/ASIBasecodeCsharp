
using ASI.Basecode.Services.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl
{
    public class AccessControlService :Controller, IAccessControlInterface
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionManager _sessionManager;
        public AccessControlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            this._sessionManager = new SessionManager(httpContextAccessor.HttpContext.Session);
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

        public async Task<bool> CheckAdminAccess()
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
