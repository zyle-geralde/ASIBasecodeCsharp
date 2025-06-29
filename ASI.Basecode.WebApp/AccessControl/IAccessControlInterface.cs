using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl
{
    public interface IAccessControlInterface
    {
        Task<bool> CheckAdminAccess();
        Task<bool> CheckUserAccess();
    }
}
