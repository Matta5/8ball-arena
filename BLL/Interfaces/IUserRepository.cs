using BLL.Models;

namespace BLL.Interfaces
{
    public interface IUserRepository
    {
        public List<User> GetAllUsers();
        public User GetUserByNameAndPassword(string username, string password);
        public bool ValidateUserCredentials(string username, string password);


    }
}
