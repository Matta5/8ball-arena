using BLL.Interfaces;
using BLL.DTOs;
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

        public UserDTO GetUserById(int id)
        {
            return userRepository.GetUserById(id);
        }

        public UserDTO GetUserByNameAndPassword(string username, string password)
        {
            return userRepository.GetUserByNameAndPassword(username, password);
        }

        public bool ValidateUserCredentials(string username, string password, out int id)
        {
            return userRepository.ValidateUserCredentials(username, password, out id);
        }

        public bool CreateUser(CreateUserDTO user)
        {
            string passwordValidationResult = IsValidPassword(user.Password);
            if (passwordValidationResult != null)
            {
                return false;
            }
            user.DateJoined = DateTime.Now;
            userRepository.CreateUser(user);
            return true;
        }

        public void EditUser(int id, EditUserDTO user)
        {
            userRepository.EditUser(id, user);
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
