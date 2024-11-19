using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BLL.Interfaces;

namespace _8ball_arena.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session != null && HttpContext.Session.GetInt32("Id") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            int id;
            if (_userRepository.ValidateUserCredentials(username, password, out id))
            {
                HttpContext.Session.SetInt32("Id", id);
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
