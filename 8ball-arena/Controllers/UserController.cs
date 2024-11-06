using BLL;
using Microsoft.AspNetCore.Mvc;
using DAL;
using BLL.Models;
using _8ball_arena.Models;

namespace _8ball_arena.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        // GET: UserController
        public ActionResult Index()
        {
            List<UserDTO> userDTOs = userService.GetAllUsers();
            List<UserViewModel> userViewModels = userDTOs.Select(u => new UserViewModel
            {
                Id = u.id,
                Username = u.username,
                Email = u.email,
                ProfilePicture = u.profile_picture,
                Wins = u.wins,
                Rating = u.rating,
                GamesPlayed = u.gamesPlayed
            }).ToList();

            return View(userViewModels);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDTO user, IFormFile profilePicture)
        {
            try
            {
                // Check if a file has been uploaded
                if (profilePicture != null)
                {
                    // Check if the file is an image
                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".png")
                    {
                        ModelState.AddModelError("", "Invalid file type. Only JPG and PNG file types are allowed.");
                        return View(user);
                    }

                    // Save the profile picture to wwwroot/Images and get the file path
                    var fileName = Path.GetFileName(profilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    // Save the relative file path to the database
                    user.profile_picture = Path.Combine("Images", fileName);
                }

                // User creation logic
                userService.CreateUser(user);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
