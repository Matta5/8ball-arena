namespace BLL.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DuelId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
