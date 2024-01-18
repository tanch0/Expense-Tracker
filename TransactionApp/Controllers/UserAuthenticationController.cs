using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionApp.Models.DTO;
using TransactionApp.Repositories.Abstract;

namespace TransactionApp.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _authService;
        public UserAuthenticationController(IUserAuthenticationService authService)
        {
            this._authService = authService;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _authService.LoginAsync(model);
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Display", "Dashboard");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            model.Role = "user";
            var result = await this._authService.RegisterAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Registration));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this._authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin()
        {
            RegistrationModel model = new RegistrationModel
            {
                Username="admin",
                Email="admin@gmail.com",
                FirstName="John",
                LastName="Doe",
                Password="admin123@"
            };
            model.Role = "admin";
            var result = await this._authService.RegisterAsync(model);
           return Ok(result);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _authService.ChangePasswordAsync(model, User.Identity.Name);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(ChangePassword));
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity.Name;
            var userProfile = await _authService.GetProfileAsync(username);

            if (userProfile == null)
            {
                // Handle user not found
                return NotFound();
            }

            return View(userProfile);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var username = User.Identity.Name;
            var result = await _authService.UpdateProfileAsync(model, username);

            if (result.StatusCode == 1)
            {
                TempData["success"] = "Profile Updated successfully";
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                // Handle errors, e.g., display error messages
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }

    }
}
