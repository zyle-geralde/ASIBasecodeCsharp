using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.AccessControl.AdminAccessControl
{
    public interface IAdminAccessInterface
    {
        Task<bool> CheckAccess();
    }
}
