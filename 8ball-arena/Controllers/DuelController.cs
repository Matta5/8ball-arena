using BLL;
using Microsoft.AspNetCore.Mvc;
using _8ball_arena.Models;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Exceptions.Duel;
using System;

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
            try
            {
                var sessionId = HttpContext.Session.GetInt32("Id");
                DuelDTO duelDTO = duelService.GetDuelById(id);

                if (duelDTO == null)
                {
                    TempData["Error"] = "The requested duel was not found.";
                    return RedirectToAction("Index", "Home");
                }

                duelDTO.Participants = duelDTO.Participants ?? new List<DuelParticipantDTO>();

                DuelViewModel duelViewModel = new DuelViewModel
                {
                    Id = duelDTO.Id,
                    Status = duelDTO.Status,
                    DateCreated = duelDTO.DateCreated,
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
            catch (DuelServiceException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while retrieving duel details.";
                return RedirectToAction("Index", "Home");
            }
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
                    TempData["Error"] = "You must be logged in to create a duel.";
                    return View(duelViewModel);
                }

                try
                {
                    UserDTO userDTO = userService.GetUserByUsername(duelViewModel.Username);
                    int participantUser = userDTO.Id;

                    int duelId = duelService.CreateDuel(sessionId.Value, participantUser);
                    return RedirectToAction("Details", "Duel", new { id = duelId });
                }
                catch (NotFoundException)
                {
                    TempData["Error"] = "This username was not found.";
                    return View(duelViewModel);
                }
                catch (UserServiceException ex)
                {
                    TempData["Error"] = ex.Message;
                    return View(duelViewModel);
                }
                catch (DuelServiceException ex)
                {
                    TempData["Error"] = ex.Message;
                    return View(duelViewModel);
                }
                catch (Exception)
                {
                    TempData["Error"] = "An unexpected error occurred while creating the duel.";
                    return View(duelViewModel);
                }
            }

            TempData["Error"] = "The provided duel data is invalid.";
            return View(duelViewModel);
        }

        // POST: DuelController/AssignWinner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignWinner(int id, int winnerId)
        {
            try
            {
                duelService.AssignWinner(id, winnerId);
                TempData["Success"] = "Winner has been assigned successfully!";
                return RedirectToAction("Details", "Duel", new { id });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (DuelServiceException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Duel", new { id });
            }
            catch (Exception)
            {
                TempData["Error"] = "An unexpected error occurred while assigning the winner.";
                return RedirectToAction("Details", "Duel", new { id });
            }
        }
    }
}
