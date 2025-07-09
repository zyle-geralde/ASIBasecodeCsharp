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
using System.Collections.Generic;
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
        private static readonly Dictionary<string, UserViewModel> _pendingRegistrations = new();

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
                TempData["ErrorMessage"] = "Incorrect Email or Password";
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

        /*  [HttpPost]
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
          }*/
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            ViewBag.LoginView = true;
            try
            {
                // Validate username and email don't exist in verified users
                if (await _userService.IsEmailVerifiedAndExists(model.Email))
                {
                    throw new InvalidDataException(Resources.Messages.Errors.UserExists);
                }

                if (await _userService.IsUsernameVerifiedAndExists(model.UserName))
                {
                    throw new InvalidDataException("Username already exists.");
                }

                // Generate OTP without creating user in database
                string otpCode = await _userService.SendOtpCodeEmail(model.Email);
                model.OtpCode = otpCode;
                model.OtpExpirationDate = DateTime.Now.AddMinutes(2);

                model.Role = "User";
                // Store model in memory with email as key
                _pendingRegistrations[model.Email] = model;

                return RedirectToAction("VerifyOtpPage", "Account", new { email = model.Email });
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        // Helper method to validate OTP
        private bool IsOtpValid(string email, string otpCode)
        {
            if (!_pendingRegistrations.TryGetValue(email, out UserViewModel userModel))
            {
                return false;
            }

            return userModel.OtpCode == otpCode &&
                   userModel.OtpExpirationDate.HasValue &&
                   userModel.OtpExpirationDate.Value > DateTime.Now;
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
                // Validate username and email don't exist in verified users
                if (await _userService.IsEmailVerifiedAndExists(model.Email))
                {
                    throw new InvalidDataException(Resources.Messages.Errors.UserExists);
                }

                if (await _userService.IsUsernameVerifiedAndExists(model.UserName))
                {
                    throw new InvalidDataException("Username already exists.");
                }

                // Generate OTP without creating user in database
                string otpCode = await _userService.SendOtpCodeEmail(model.Email);
                model.OtpCode = otpCode;
                model.OtpExpirationDate = DateTime.Now.AddMinutes(5);

                model.Role = "Admin";
                // Store model in memory with email as key
                _pendingRegistrations[model.Email] = model;

                return RedirectToAction("VerifyOtpPage", "Account", new { email = model.Email });
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View();
        }


        //VerifyOTP
        [HttpGet]
        [Route("Account/VerifyOtpPage/{email}")]
        [AllowAnonymous]
        public IActionResult VerifyOtpPage(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid verification request.";
                return RedirectToAction("Login", "Account");
            }

            if (!_pendingRegistrations.TryGetValue(email, out UserViewModel model))
            {
                TempData["ErrorMessage"] = "Registration session expired. Please register again.";
                return RedirectToAction("Register", "Account");
            }

            OtpViewModel otpModel = new OtpViewModel
            {
                Email = email,
                OtpCode = string.Empty // Don't send the OTP to the view for security reasons
            };

            return View("~/Views/Account/OTPView.cshtml", otpModel);
        }


        [HttpPost]
        [Route("Account/VerifyOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(OtpViewModel model)
        {
            try
            {
                // Check if the OTP is valid without modifying the database
                bool isOtpValid = IsOtpValid(model.Email, model.OtpCode);

                if (!isOtpValid)
                {
                    throw new Exception("Invalid or expired OTP code");
                }

                // Get the stored registration data
                if (!_pendingRegistrations.TryGetValue(model.Email, out UserViewModel userModel))
                {
                    throw new Exception("Registration session expired. Please register again.");
                }

                // Get role based on registration path (could be stored in the UserViewModel during registration)
                //string role = userModel.Email.EndsWith("admin") ? "Admin" : "User";
                string role = userModel.Role;

                // Create verified user directly
                await _userService.CreateVerifiedUser(userModel, role);

                // Remove from pending registrations
                _pendingRegistrations.Remove(model.Email);

                TempData["SuccessMessage"] = "Your account has been verified successfully! You can now login!";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Account/OTPView.cshtml", model);
            }
        }

        [HttpPost]
        [Route("Account/ResendOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp(string email)
        {
            try
            {
                if (!_pendingRegistrations.TryGetValue(email, out UserViewModel userModel))
                {
                    TempData["ErrorMessage"] = "Registration session expired. Please register again.";
                    return RedirectToAction("Register", "Account");
                }

                // Generate new OTP and send email
                string otpCode = await _userService.SendOtpCodeEmail(email);
                userModel.OtpCode = otpCode;
                userModel.OtpExpirationDate = DateTime.Now.AddMinutes(5);

                // Update in pending registrations
                _pendingRegistrations[email] = userModel;

                TempData["SuccessMessage"] = "A new verification code has been sent to your email.";
                return RedirectToAction("VerifyOtpPage", "Account", new { email = email });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("VerifyOtpPage", "Account", new { email = email });
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

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/NewPassword")]
        public async Task<IActionResult> SendOtpForForgotPassword([FromBody] UserViewModel userObject)
        {
            if (userObject == null)
            {
                return BadRequest(new { Message = "No data has been passed" });
            }
            try
            {
                await _userService.UpdatePassword(userObject);
                return Ok(new { Message = "Successfully Updated Password" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
