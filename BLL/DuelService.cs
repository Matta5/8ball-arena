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

        public void CreateDuel(int userId1, int userId2)
        {
            if (userId1 == userId2)
            {
                throw new InvalidOperationException("You can't duel yourself!");
            }

            duelRepository.CreateDuel(userId1, userId2);
        }
    }
}
