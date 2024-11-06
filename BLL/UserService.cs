using BLL.Interfaces;
using BLL.Models;
using System.Text.RegularExpressions;

namespace BLL
{
    public class UserService
    {
        private IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {

            this.userRepository = userRepository;
        }

        public List<UserDTO> GetAllUsers()
        {
            return userRepository.GetAllUsers();
        }

        public UserDTO GetUserByNameAndPassword(string username, string password)
        {
            return userRepository.GetUserByNameAndPassword(username, password);
        }

        public bool ValidateUserCredentials(string username, string password)
        {
            return userRepository.ValidateUserCredentials(username, password);
        }

        public bool CreateUser(UserDTO user)
        {
            string passwordValidationResult = IsValidPassword(user.password);
            if (passwordValidationResult != null)
            {
                return false;
            }
            userRepository.CreateUser(user);
            return true;
        }

        public string IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return "Password cannot be empty.";

            bool hasUpperCase = Regex.IsMatch(password, "[A-Z]");
            bool hasNumber = Regex.IsMatch(password, "[0-9]");

            if (!hasUpperCase)
                return "Password must include at least one capital letter.";
            if (!hasNumber)
                return "Password must include at least one number.";

            return null;
        }
    }
}
