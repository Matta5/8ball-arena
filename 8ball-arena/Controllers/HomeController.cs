using _8ball_arena.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _8ball_arena.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("Id");
            ViewBag.Id = id.ToString();
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

