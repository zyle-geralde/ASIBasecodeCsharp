using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using ASI.Basecode.WebApp.Payload.OtpPayload;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;
        private readonly IPersonProfileService _personProfileService;
        private readonly IMapper _mapper;
        private readonly IAccessControlInterface _accessControlInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            IPersonProfileService profileService,
                            IAccessControlInterface accessControlInterface,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
            this._personProfileService = profileService;
            this._mapper = mapper;
            this._accessControlInterface = accessControlInterface;

        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            User user = null;

            /*User user = new() { Id = 0, UserId = "0", Name = "Name", Password = "Password" };
            
            await this._signInManager.SignInAsync(user);
            this._session.SetString("UserName", model.UserId);

            return RedirectToAction("Index", "Home");*/

            var loginResult = _userService.AuthenticateUser(model.UserId, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                // 認証OK
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserName", user.UserName);
                this._session.SetString("UserEmail", user.Email);
                PersonProfile userProfile = await _personProfileService.GetPersonProfile(model.UserId);

                this._session.SetString("UserRole", userProfile.Role);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 認証NG
                TempData["ErrorMessage"] = "Incorrect UserId or Password";
                return View();
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.LoginView = true;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            ViewBag.LoginView = true;
            try
            {
                var user = await _userService.AddUserFromRegister(model);

                return RedirectToAction("VerifyOtpPage", "Account", new { email = user.Email });
            }
            catch(InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                //TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View();
        }


        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }



        //Admin Register
        [HttpGet]
        [Route("Account/RegisterAdmin")]
        [AllowAnonymous]
        public IActionResult RegisterAdmin()
        {
            ViewBag.LoginView = true;
            return View("~/Views/Account/RegisterAdmin.cshtml");
        }

        [Authorize]
        public async Task<IActionResult> AdminDashboard()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [Route("Account/RegisterAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin(UserViewModel model)
        {
            ViewBag.LoginView = true;
            try
            {
                var user = await _userService.AddAdminFromRegister(model);


                return RedirectToAction("VerifyOtpPage", "Account", new { email = user.Email });
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                //TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
          
            return View();
        }


        //VerifyOTP
        [HttpGet]
        [Route("Account/VerifyOtpPage/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtpPage(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid verification request.";
                return RedirectToAction("Login", "Account");
            }
            try
            {
                OtpViewModel user_otp = await _userService.GetUserbyEmail(email);
                ViewBag.Email = email;
                return View("~/Views/Account/OTPView.cshtml", user_otp);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Email not found";
                return RedirectToAction("Login", "Account");
            }
           
        }


        [HttpPost]
        [Route("Account/VerifyOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(OtpViewModel model)
        {
            try
            {
                
                await _userService.VerifyOtp(model);
                TempData["SuccessMessage"] = "Your account has been verified successfully! You can now login!";

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex) {
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Account/OTPView.cshtml",model);
            }
        }

        [HttpPost]
        [Route("Account/ResendOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp(string email)

        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required to resend OTP.");
                return View("~/Views/Account/OTPView.cshtml", new OtpViewModel { Email = email });
            }

            try
            {
                OtpViewModel otpSent = await _userService.RegenerateOtpAsync(email);

                if (otpSent != null)
                {
                    TempData["SuccessMessage"] = "A new OTP has been sent to your email. Please check your inbox.";
                    return RedirectToAction("VerifyOtpPage", "Account", new { Email = email });
                }
                else
                {
                   
                    ModelState.AddModelError("", "Failed to send new OTP. Please try again.");
                    return View("~/Views/Account/OTPView.cshtml", new OtpViewModel { Email = email });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", "Email already verified");
                return View("~/Views/Account/OTPView.cshtml", new OtpViewModel { Email = email });
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError("", "An unexpected error occurred while trying to resend OTP. Please try again later.");
                return View("~/Views/Account/OTPView.cshtml", new OtpViewModel { Email = email });
            }

        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Account/ForgotPassword")]
        public async Task<IActionResult> SendOtpForForgotPassword([FromBody] ForgotPasswordOtpPayload emailObject)
        {
            if(emailObject == null)
            {
                return BadRequest(new { Message = "No data has been passed" });
            }
            try
            {
                string generatedOTP = await _userService.SendOTPForResetPassword(emailObject.Email);
                return Ok(new { Message = generatedOTP });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message});
            }
        }
    }
}
