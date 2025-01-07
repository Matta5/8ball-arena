using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IDuelRepository
    {
        DuelDTO GetDuelById(int duelId);
        List<DuelParticipantDTO> GetParticipantsByDuelId(int duelId);
        int CreateDuel(int userId1, int userId2);
        void AssignWinner(int duelId, int winnerUserId);
        public List<DuelDTO> GetDuelsByUserId(int userId);
    }
}
