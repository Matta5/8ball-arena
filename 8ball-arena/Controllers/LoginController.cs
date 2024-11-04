using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BLL.Interfaces;

namespace _8ball_arena.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
         
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (_userRepository.ValidateUserCredentials(username, password))
            {
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
