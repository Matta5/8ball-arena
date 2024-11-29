using BLL.DTOs;

namespace _8ball_arena.Models
{

    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public int Wins { get; set; }
        public int Rating { get; set; }
        public int GamesPlayed { get; set; }
        public DateTime DateJoined { get; set; }

        public List<DuelDTO> Duels { get; set; } = new List<DuelDTO>();
        public List<DuelDTO> CompletedDuels  => Duels.Where(d => d.Status == "Completed").ToList();
        public List<DuelDTO> ActiveDuels => Duels.Where(d => d.Status == "Pending").ToList();
    }

}
