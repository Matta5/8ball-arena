using BLL.Interfaces;

namespace BLL
{
    public class DuelService
    {
        private IDuelRepository duelRepository;

        public DuelService(IDuelRepository duelRepository)
        {

            this.duelRepository = duelRepository;
        }

        public DuelDTO GetDuelById(int duelId)
        {
            return duelRepository.GetDuelById(duelId);
        }

        public List<DuelDTO> GetDuelsByUserId(int userId)
        {
            return duelRepository.GetDuelsByUserId(userId);
        }

        public int CreateDuel(int userId1, int userId2)
        {
            if (userId1 == userId2)
            {
                throw new InvalidOperationException("You can't duel yourself!");
            }

            return duelRepository.CreateDuel(userId1, userId2);
        }

        public void AssignWinner(int duelId, int winnerId)
        {
			duelRepository.AssignWinner(duelId, winnerId);
		}
    }
}
