using IdentityPractice.Tools;
using IdentityPractice.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace IdentityPractice.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IViewRenderService _viewRenderService;
        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender,
            IViewRenderService viewRenderService, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _viewRenderService = viewRenderService;
            _userManager = userManager;
        }


        public IActionResult Register()
        {
            ViewBag.IsSent = false;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsSent = false;
                return View(model);
            }
                

            var result = await _userManager.CreateAsync(new IdentityUser()
            {
                UserName = model.UserName,
                PhoneNumber = model.Phone,
                Email = model.Email
            }, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View();
                }
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ConfirmEmail", "Account", new { userId = user.Id, token = token }
            , Request.Scheme);
            string body = await _viewRenderService.RenderToStringAsync("_RegisterEmail", callBackUrl);
            await _emailSender.SendEmailAysnc(new EmailModel(user.Email, "تایید حساب", body));
            ViewBag.IsSent = true;
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            ViewBag.IsConfirmed = result.Succeeded ? true : false;
            return View();
        }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid) return View(model);

            var user = _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربری با این مشخصات یافت نشد!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password,
                model.RememberMe, false);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWithTowFactor");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کاربری شما قفل شده است!");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "تلاش برای ورود ناموفق است!");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult ForgotPassword()
        {
            ViewBag.IsSent = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.IsSent = false;
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.IsSent = false;
                ModelState.AddModelError(string.Empty, "تلاش برای ارسال ایمیل ناموفق می باشد!");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string? callBackUrl = Url.ActionLink("ResetPassword", "Account", new { email = user.Email, token = token }
            , Request.Scheme);
            string body = await _viewRenderService.RenderToStringAsync("_ResetPasswordEmail", callBackUrl);
            await _emailSender.SendEmailAysnc(new EmailModel(user.Email, "بازیابی رمز عبور", body));
            ViewBag.IsSent = true;
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token)) return BadRequest();

            var model = new ResetPasswordVM()
            {
                Email = email,
                Token = token
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "بازیابی رمز عبور نام موفق");
                return View(model);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return RedirectToAction("Login");
        }


        #region RemoteValidation
        
        [HttpPost]   
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string userName)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.UserName == userName);

            if (!any)
                return Json(true); 

            return Json("نام کاربری مورد نظر از قبل ثبت شده است!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.Email == email);

            if (!any)
                return Json(true);

            return Json("ایمیل مورد نظر از قبل ثبت شده است!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsPhoneValid(string phone)
        {

            bool validationResult =  InputsValidation.IsValidIranianPhoneNumber(phone);

            if (validationResult) 
                return Json(true);

            return Json("شماره مورد نظر معتبر نمی باشد");
        }

        #endregion


    }
}
