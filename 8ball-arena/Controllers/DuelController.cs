using BLL;
using Microsoft.AspNetCore.Mvc;
using _8ball_arena.Models;
using BLL.DTOs;
using BLL.Exceptions;

namespace _8ball_arena.Controllers
{
    public class DuelController : BaseController
    {
        private readonly DuelService duelService;
        private readonly UserService userService;

        public DuelController(DuelService duelService, UserService userService)
        {
            this.duelService = duelService;
            this.userService = userService;
        }


        // GET: DuelController/Details/5
        public ActionResult Details(int id)
        {
            var sessionId = HttpContext.Session.GetInt32("Id");
            DuelDTO duelDTO = duelService.GetDuelById(id);

            if (duelDTO == null)
            {
                return NotFound();
            }

            duelDTO.Participants = duelDTO.Participants ?? new List<DuelParticipantDTO>();

            DuelViewModel duelViewModel = new DuelViewModel
            {
                Id = duelDTO.Id,
                Status = duelDTO.Status,
                DateCreated = duelDTO.DateCreated,
                ParticipantUserId = duelDTO.ParticipantUserId,
                Participants = duelDTO.Participants.Select(p => new DuelParticipantViewModel
                {
                    UserId = p.UserId,
                    Username = p.Username,
                    IsWinner = p.IsWinner
                }).ToList()
            };

            ViewBag.SessionId = sessionId;
            return View(duelViewModel);
        }


        // GET: DuelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DuelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateDuelViewModel duelViewModel)
        {
            if (ModelState.IsValid)
            {
                var sessionId = HttpContext.Session.GetInt32("Id");
                if (sessionId == null)
                {
                    ModelState.AddModelError(string.Empty, "Please log in before creating a duel.");
                    return View(duelViewModel);  
                }

                try
                {
                    UserDTO userDTO = userService.GetUserByUsername(duelViewModel.Username);
                    int participantUser = userDTO.Id;

                    int duelId = duelService.CreateDuel(sessionId.Value, participantUser);
                    return RedirectToAction("Details", "Duel", new { id = duelId });
                }
                catch (PasswordValidationException ex)
                {
                    ModelState.AddModelError("Password", ex.Message);
                    return View(duelViewModel);
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(duelViewModel);
                }
                catch (UserServiceException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(duelViewModel);
                }
            }

            return View(duelViewModel);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignWinner(int id, int winnerId)
        {
			duelService.AssignWinner(id, winnerId);
			return RedirectToAction("Details", "Duel", new { id });
		}
	}
}
