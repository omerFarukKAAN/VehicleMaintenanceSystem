using vts.Models;
using vts.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace vts.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            if (await _accountRepository.IsUserExist(userModel.Email))
            {
                ModelState.AddModelError("", "This mail adress already exist");
                return View(userModel);
            }

            var result = await _accountRepository.CreateUserAsync(userModel);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ModelState.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Route("Login")]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel userModel, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            if (await _accountRepository.IsUserExist(userModel.Email))
            {
                var result = await _accountRepository.Login(userModel);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Çok fazla şifre denemesi yaptınız. Lütfen 1 dakika bekleyiniz.");

                    return View(userModel);
                }
            }

            ModelState.AddModelError("", "Kullanıcı adı veya Şifre bilgileri yanlış");
            return View(userModel);
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.Logout();

            return Redirect("/");
        }

        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Kullanıcı adı belirtmelisiniz");
                return View();
            }

            var result = await _accountRepository.ResetPassword(email);
            if (result == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View();
            }

            var resetPasswordLink = Url.Action("ResetPassword", "Account",
                        new { uid = result[1], token = result[0] },
                        protocol: Request.Scheme);

            // Burada şifre sıfırlama linki Email adresine gönderilebilir.

            return Ok(resetPasswordLink);
        }

        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string uid, string token)
        {
            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(token))
                return RedirectToAction("Index");

            var user = await _accountRepository.FindByIdAsync(uid);

            if (user == null)
            {
                return RedirectToAction("Register");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = uid,
                Token = token
            };

            return View(model);
        }


        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            var user = await _accountRepository.FindByIdAsync(userModel.UserId);

            if (user == null)
            {
                return RedirectToAction("Register");
            }

            var result = await _accountRepository.ResetPassword(user, userModel);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userModel);
        }
    }
}
