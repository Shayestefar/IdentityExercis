using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Simple.Models;
using Simple.Models.Entities.Identity;

using System.Diagnostics;
using System.Threading.Tasks;

namespace Simple.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(
            ILogger<HomeController> logger,

            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,

            IUserStore<User> userStore,
            IRoleStore<Role> roleStore

        //IUserValidator<User> userValidator,
        //IRoleValidator<User> roleValidator
        )
        {
            Logger = logger;
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
            UserStore = userStore;
            RoleStore = roleStore;
            //UserValidator = userValidator;
            //RoleValidator = roleValidator;
        }

        public ILogger<HomeController> Logger { get; }
        public UserManager<User> UserManager { get; }
        public RoleManager<Role> RoleManager { get; }
        public SignInManager<User> SignInManager { get; }
        public IUserStore<User> UserStore { get; }
        public IRoleStore<Role> RoleStore { get; }
        public IUserValidator<User> UserValidator { get; }
        public IRoleValidator<User> RoleValidator { get; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet(nameof(LoginUser))]
        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginUserAsync(string email, string password)
        {
            User user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "لطفا از صحت اطلاعات وارد شده اطمینان حاصل کنید");
                return View();
            }

            var checkPassSignIn = await SignInManager.CheckPasswordSignInAsync(user, password, true);
            if (checkPassSignIn.Succeeded)
            {
                var result = await SignInManager.PasswordSignInAsync(email, password, true, false);
                if (result.Succeeded) RedirectToAction(nameof(Index));
                else
                {
                    ModelState.AddModelError(string.Empty, "اطلاعات وارد شده صحیح نمی باشد");
                    return View();
                }
            }

            return View();
        }


        [HttpGet(nameof(ResetPassword))]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(
            string email,
            string code,
            string password
        )
        {
            User user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "کاربر نا معتبر");
                return View();
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPassword));

            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
