using BLL.Interfaces;
using BLL.Exceptions;
using BLL.Exceptions.Duel;
using System;

namespace BLL
{
    public class DuelService
    {
        private readonly IDuelRepository _duelRepository;

        public DuelService(IDuelRepository duelRepository)
        {
            _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
        }

        public DuelDTO GetDuelById(int duelId)
        {
            try
            {
                return _duelRepository.GetDuelById(duelId);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException($"Duel with ID {duelId} was not found.", ex);
            }
            catch (DuelRepositoryException ex)
            {
                throw new DuelServiceException("An error occurred in the repository while retrieving the duel by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new DuelServiceException("An unexpected error occurred while retrieving the duel by ID.", ex);
            }
        }

        public List<DuelDTO> GetDuelsByUserId(int userId)
        {
            try
            {
                return _duelRepository.GetDuelsByUserId(userId);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException($"No duels were found for user with ID {userId}.", ex);
            }
            catch (DuelRepositoryException ex)
            {
                throw new DuelServiceException("An error occurred in the repository while retrieving duels by user ID.", ex);
            }
            catch (Exception ex)
            {
                throw new DuelServiceException("An unexpected error occurred while retrieving duels by user ID.", ex);
            }
        }

        public int CreateDuel(int userId1, int userId2)
        {
            try
            {
                if (userId1 == userId2)
                {
                    throw new InvalidOperationException("A user cannot duel themselves.");
                }

                return _duelRepository.CreateDuel(userId1, userId2);
            }
            catch (InvalidOperationException ex)
            {
                throw new DuelServiceException(ex.Message, ex);
            }
            catch (DuelRepositoryException ex)
            {
                throw new DuelServiceException("An error occurred in the repository while creating the duel.", ex);
            }
            catch (Exception ex)
            {
                throw new DuelServiceException("An unexpected error occurred while creating the duel.", ex);
            }
        }

        public void AssignWinner(int duelId, int winnerId)
        {
            try
            {
                _duelRepository.AssignWinner(duelId, winnerId);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException($"Duel with ID {duelId} or user with ID {winnerId} was not found.", ex);
            }
            catch (DuelRepositoryException ex)
            {
                throw new DuelServiceException("An error occurred in the repository while assigning the winner.", ex);
            }
            catch (Exception ex)
            {
                throw new DuelServiceException("An unexpected error occurred while assigning the winner.", ex);
            }
        }
    }
}
