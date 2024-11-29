using BLL;
using Microsoft.AspNetCore.Mvc;
using _8ball_arena.Models;
using BLL.DTOs;

namespace _8ball_arena.Controllers
{
    public class DuelController : BaseController
    {
        private readonly DuelService duelService;

        public DuelController(DuelService duelService)
        {
            this.duelService = duelService;
        }


        // GET: DuelController/Details/5
        public ActionResult Details(int id)
        {
            var sessionId = HttpContext.Session.GetInt32("Id");
            DuelDTO duelDTO = duelService.GetDuelById(id);

            if (duelDTO == null)
            {
                // Handle case where the duel is not found
                return NotFound();
            }

            // Ensure Participants is not null
            duelDTO.Participants = duelDTO.Participants ?? new List<DuelParticipantDTO>();

            // Map DuelDTO to DuelViewModel
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
    }
}
