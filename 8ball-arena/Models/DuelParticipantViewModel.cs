namespace _8ball_arena.Models
{
    public class DuelParticipantViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool? IsWinner { get; set; }
        public string ProfilePicture { get; set; }
    }

}
