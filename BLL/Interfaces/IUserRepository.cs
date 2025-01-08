using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IUserRepository
    {
        public List<UserDTO> GetAllUsers();
        public UserDTO GetUserById(int id);
        public UserDTO GetUserByUsername(string username);
        public bool ValidateUserCredentials(string username, string password, out int id);
        public void CreateUser(CreateUserDTO user);
        public void EditUser(int id, EditUserDTO user);
        public void DeleteUser(int id);
        public bool CheckIfUsernameExists(string username);
    }
}