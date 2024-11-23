namespace BLL.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Wins { get; set; } = 0;
        public int Rating { get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;
        public string ProfilePicture { get; set; }
        public DateTime DateJoined { get; set; }


        public List<DuelDTO> ActiveDuels { get; set; } = new List<DuelDTO>();
        public List<DuelDTO> CompletedDuels { get; set; } = new List<DuelDTO>();
    }
}
