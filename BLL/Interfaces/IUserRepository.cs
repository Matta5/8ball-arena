using BLL.Models;

namespace BLL.Interfaces
{
    public interface IUserRepository
    {
        public List<UserDTO> GetAllUsers();
        public UserDTO GetUserByNameAndPassword(string username, string password);
        public bool ValidateUserCredentials(string username, string password);

        public void CreateUser(UserDTO user);


    }
}
