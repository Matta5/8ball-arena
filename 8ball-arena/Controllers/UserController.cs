using BLL;
using Microsoft.AspNetCore.Mvc;
using BLL.DTOs;
using _8ball_arena.Models;
using System.Linq;

namespace _8ball_arena.Controllers
{
    public class UserController : BaseController
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
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                ProfilePicture = u.ProfilePicture,
                Wins = u.Wins,
                Rating = u.Rating,
                GamesPlayed = u.GamesPlayed,
                DateJoined = u.DateJoined
            }).ToList();

            return View(userViewModels);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            var sessionId = HttpContext.Session.GetInt32("Id");
            UserDTO userDTO = userService.GetUserById(id);
            UserViewModel userViewModel = new UserViewModel
            {
                Id = userDTO.Id,
                Username = userDTO.Username,
                Email = userDTO.Email,
                ProfilePicture = userDTO.ProfilePicture,
                Wins = userDTO.Wins,
                Rating = userDTO.Rating,
                GamesPlayed = userDTO.GamesPlayed,
                DateJoined = userDTO.DateJoined
            };

            ViewBag.SessionId = sessionId;
            return View(userViewModel);
        }



        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserDTO user, IFormFile profilePicture)
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
                        ViewBag.FileError = "File must be a .jpg or .png image";
                        return View(user);
                    }

                    // Save the profile picture to wwwroot/Images and get the file path
                    var fileName = Path.GetFileName(profilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        profilePicture.CopyTo(fileStream);
                    }

                    // Save the relative file path to the database
                    user.ProfilePicture = Path.Combine("Images", fileName);
                }

                // User creation logic
                if (!userService.CreateUser(user))
                {
                    ViewBag.PasswordError = "Password must include a capital letter and a number";
                    return View(user);
                }

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
