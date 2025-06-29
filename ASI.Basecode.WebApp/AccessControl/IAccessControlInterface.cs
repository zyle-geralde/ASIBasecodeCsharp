using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl
{
    public interface IAccessControlInterface
    {
        Task<bool> CheckUserAccess();
        Task<bool> CheckAdminAccess();

    }
}
