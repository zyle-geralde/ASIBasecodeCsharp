using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl
{
    public interface IAccessControlInterface
    {
        Task<string> CheckAccess();
        Task<bool> CheckUserAccess();
        Task<bool> CheckAdminAccess();

        Task<IActionResult> CheckAdminAccesHelper();
    }
}
