using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization; add this to allow anonymous

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IAccessControlInterface _accessControlInterface;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IAccessControlInterface accessControlInterface,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._accessControlInterface = accessControlInterface;
        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        //[AllowAnonymous] //This is to bypass authentication. Ex. if you want to access this route without loging in
        [Authorize]
        public async Task<IActionResult> Index()
        {
            bool checkUserAccess = await _accessControlInterface.CheckUserAccess();
            if (!checkUserAccess) return RedirectToAction("AdminDashboard", "Account");
            return View();
        }
    }
}
