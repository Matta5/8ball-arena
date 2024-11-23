using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IUserRepository
    {
        public List<UserDTO> GetAllUsers();
        public UserDTO GetUserById(int id);
        public UserDTO GetUserByNameAndPassword(string username, string password);
        public bool ValidateUserCredentials(string username, string password, out int id);

        public void CreateUser(UserDTO user);
    }
}