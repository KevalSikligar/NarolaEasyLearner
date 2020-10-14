using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EasyLearner.Service.GlobalConstant;
using EasyLearnerAdmin.Data;
using EasyLearner.Service.Enums;

namespace EasyLearnerAdmin.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool isActive { get; set; }
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var activeUser = _context.Users.Where(u => (u.Email.Equals(Input.Email) || u.UserName == Input.Email) && u.IsActive == true).Any();
                    
                                    

                    if (activeUser)
                    {
                        var userResult =await  _userManager.FindByNameAsync(Input.Email);
                        if (userResult!=null)
                        {
                            var roleList=await _userManager.GetRolesAsync(userResult);
                            if (roleList.FirstOrDefault().Equals(UserRoles.Admin) || roleList.FirstOrDefault().Equals(UserRoles.Staff))
                            {
                                _logger.LogInformation("User logged in.");
                                //   return LocalRedirect(returnUrl);
                                return LocalRedirect("~/Home/Index");

                            }
                            else {
                                await _signInManager.SignOutAsync();
                                TempData["WrongUserNamePasswordAlert"] = LoginValidationMessageList.WrongUserNamePassword;
                            }
                        }
                       
                    }
                    else
                    {
                        await _signInManager.SignOutAsync();
                        TempData["InActiveValidationAlert"] = LoginValidationMessageList.InactivateUser;
                    }
                }
                else
                {
                    TempData["WrongUserNamePasswordAlert"] = LoginValidationMessageList.WrongUserNamePassword;
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
