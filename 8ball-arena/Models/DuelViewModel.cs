using BLL.DTOs;

namespace _8ball_arena.Models
{
    public class DuelViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public List<DuelParticipantViewModel> Participants { get; set; } = new List<DuelParticipantViewModel>();
        public List<CommentViewModel>? Comments { get; set; }
        public string? NewCommentText { get; set; }
    }
}
