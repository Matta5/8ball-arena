using BLL;
using BLL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using BLL.DTOs;
using _8ball_arena.Models;

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
			try
			{
				List<UserDTO> userDTOs = userService.GetAllUsers();
				List<UserViewModel> userViewModels = userDTOs.Select(u => new UserViewModel
				{
					Id = u.Id,
					Username = u.Username,
					Email = u.Email,
					ProfilePicture = u.ProfilePicture,
					Rating = u.Rating,
					DateJoined = u.DateJoined
				}).ToList();

				return View(userViewModels);
			}
			catch (Exception ex)
			{
				ViewData["Error"] = "An error occurred while retrieving users.";
				return View("Error");
			}
		}

		public IActionResult Details(int id)
		{
			try
			{
				ViewBag.SessionId = HttpContext.Session.GetInt32("Id");

				var user = userService.GetUserById(id);
				if (user == null)
					return NotFound();

				var duels = duelService.GetDuelsByUserId(id);

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
					Rating = user.Rating,
					Duels = duels,
					DateJoined = user.DateJoined
				};

				return View(viewModel);
			}
			catch (NotFoundException ex)
			{
				return NotFound();
			}
			catch (Exception ex)
			{
				ViewData["Error"] = "An error occurred while retrieving user details.";
				return View("Error");
			}
		}

		// GET: UserController/Create
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateUserDTO user, IFormFile profilePicture)
		{
			try
			{
				if (profilePicture != null)
				{
					var extension = Path.GetExtension(profilePicture.FileName).ToLower();
					if (extension != ".jpg" && extension != ".png")
					{
						ViewBag.FileError = "File must be a .jpg or .png image";
						return View(user);
					}

					var fileName = Path.GetFileName(profilePicture.FileName);
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						profilePicture.CopyTo(fileStream);
					}

					user.ProfilePicture = "/" + Path.Combine("Images", fileName);
				}

				string errorMessage;
				if (!userService.CreateUser(user, out errorMessage))
				{
					ViewBag.PasswordError = errorMessage;
					return View(user);
				}

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewData["Error"] = "An error occurred while creating the user.";
				return View("Error");
			}
		}


		public IActionResult Edit(int Id)
		{
			try
			{
				UserDTO u = userService.GetUserById(Id);

				UserViewModel songViewModel = new UserViewModel
				{
					Id = u.Id,
					Username = u.Username,
					Email = u.Email,
					ProfilePicture = u.ProfilePicture,
				};

				return View(songViewModel);
			}
			catch (NotFoundException ex)
			{
				return NotFound();
			}
			catch (UserServiceException ex)
			{
				ViewData["Error"] = ex.Message;
				return View();
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, UserViewModel userViewModel, IFormFile? profilePicture)
		{
			if (!ModelState.IsValid)
			{
				return View(userViewModel);
			}

			try
			{
				var editUserDTO = new EditUserDTO
				{
					Username = userViewModel.Username,
					Email = userViewModel.Email,
					ProfilePicture = userViewModel.ProfilePicture
				};

				if (profilePicture != null && profilePicture.Length > 0)
				{
					var extension = Path.GetExtension(profilePicture.FileName).ToLower();
					if (extension != ".jpg" && extension != ".png")
					{
						ViewBag.FileError = "File must be a .jpg or .png image";
						return View(userViewModel);
					}

					var fileName = Path.GetFileName(profilePicture.FileName);
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						profilePicture.CopyTo(fileStream);
					}

					editUserDTO.ProfilePicture = "/" + Path.Combine("Images", fileName);
				}

				userService.EditUser(id, editUserDTO);
				return RedirectToAction(nameof(Details), new { id = id });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				return View(userViewModel);
			}
		}

		// GET: UserController/Delete/5
		public ActionResult Delete(int id)
		{
			try
			{
				UserDTO user = userService.GetUserById(id);

				DeleteUserViewModel deleteUserViewModel = new DeleteUserViewModel
				{
					Id = user.Id,
					Username = user.Username
				};

				return View(deleteUserViewModel);
			}
			catch (NotFoundException ex)
			{
				return NotFound();
			}
			catch (UserServiceException ex)
			{
				ViewData["Error"] = ex.Message;
				return View("Error");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(DeleteUserViewModel model)
		{
			try
			{
				UserDTO user = userService.GetUserById(model.Id);

				if (user.Password == model.Password)
				{
					userService.DeleteUser(model.Id);
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.PasswordError = "Incorrect password.";
					model.Username = user.Username;
					return View(model);
				}
			}
			catch (NotFoundException ex)
			{
				return NotFound();
			}
			catch (UserServiceException ex)
			{
				ViewData["Error"] = ex.Message;
				return View(model);
			}
		}
	}
}
