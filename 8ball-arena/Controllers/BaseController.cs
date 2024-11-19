using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _8ball_arena.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var id = HttpContext.Session.GetInt32("Id");
            ViewBag.Id = id?.ToString();
            base.OnActionExecuting(context);
        }
    }
}
