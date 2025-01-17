using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface ICommentRepository
    {
        List<CommentDTO> GetCommentsByDuelId(int duelId);
        void AddComment(CommentDTO comment);
    }
}
