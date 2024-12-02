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
        private readonly DuelService duelService;

        public UserController(UserService userService, DuelService duelService)
        {
            this.userService = userService;
            this.duelService = duelService;
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


        public IActionResult Details(int id)
        {
            var user = userService.GetUserById(id);
            if (user == null)
                return NotFound();

            var duels = duelService.GetDuelsForUser(id);

            var activeDuels = duels
                .Where(d => d.Status == "Pending")
                .Select(d => new DuelViewModel
                {
                    Id = d.Id,
                    Status = d.Status,
                    DateCreated = d.DateCreated,
                    Participants = d.Participants.Select(p => new DuelParticipantViewModel
                    {
                        UserId = p.UserId,
                        Username = p.Username,
                        IsWinner = p.IsWinner
                    }).ToList()
                }).ToList();

            var completedDuels = duels
                .Where(d => d.Status == "Completed")
                .Select(d => new DuelViewModel
                {
                    Id = d.Id,
                    Status = d.Status,
                    DateCreated = d.DateCreated,
                    Participants = d.Participants.Select(p => new DuelParticipantViewModel
                    {
                        UserId = p.UserId,
                        Username = p.Username,
                        IsWinner = p.IsWinner
                    }).ToList()
                }).ToList();

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture ?? "/Images/Default.jpg",
                Wins = user.Wins,
                Rating = user.Rating,
                GamesPlayed = user.GamesPlayed,
                Duels = duels
            };

            return View(viewModel);
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

                    user.ProfilePicture = "/" + Path.Combine("Images", fileName);

				}

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
