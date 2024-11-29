using BLL.Interfaces;
using BLL.DTOs;
using System.Text.RegularExpressions;

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

        public List<DuelDTO> GetDuelsByUserId(int id)
        {
            return duelRepository.GetDuelsByUserId(id);
        }

        public List<DuelDTO> GetDuelsForUser(int userId)
        {
            return duelRepository.GetDuelsForUser(userId);
        }

    }
}
