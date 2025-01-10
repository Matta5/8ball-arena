using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Exceptions.User;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace BLL;

public class UserService
{
    private IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public List<UserDTO> GetAllUsers()
    {
        try
        {
            return userRepository.GetAllUsers();
        }
        catch (UserRepositoryException ex)
        {
            throw new UserServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while retrieving all users.", ex);
        }
    }

    public UserDTO GetUserById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid user ID.", nameof(id));
        }

        try
        {
            var user = userRepository.GetUserById(id);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            return user;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while retrieving user by id.", ex);
        }
    }

    public UserDTO GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }

        try
        {
            var user = userRepository.GetUserByUsername(username);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            return user;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while retrieving user by username.", ex);
        }
    }

    public bool ValidateUserCredentials(string username, string password, out int id)
    {
        id = 0;

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        try
        {
            return userRepository.ValidateUserCredentials(username, password, out id);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while validating user credentials.", ex);
        }
    }

    public bool CreateUser(CreateUserDTO user)
    {
        if (user == null)
        {
            throw new UserServiceException("User cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(user.Username))
        {
            throw new UserServiceException("Username cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(user.Password))
        {
            throw new UserServiceException("Password cannot be null or empty.");
        }

        try
        {
            if (userRepository.CheckIfUsernameExists(user.Username))
            {
                throw new UserServiceException("Username already exists");
            }

            ValidatePassword(user.Password);

            user.DateJoined = DateTime.Now;
            userRepository.CreateUser(user);
            return true;
        }
        catch (UserServiceException ex)
        {
            throw ex;
         }
        catch (UserRepositoryException ex)
        {
            throw new UserServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while creating a new user.", ex);
        }
        
    }




    public void EditUser(int id, EditUserDTO user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(user.Username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(user.Username));
        }

        try
        {
            if (userRepository.GetUserByUsername(user.Username) != null)
            {
                throw new UserServiceException("Username already exists.");
            }

            var existingUser = userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found.");
            }

            userRepository.EditUser(id, user);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException("User not found.");
        }
        catch (UserServiceException ex)
        {
            throw;
        }
        catch (UserRepositoryException ex)
        {
            throw new UserServiceException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while editing user.", ex);
        }
    }

    public void DeleteUser(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid user ID.", nameof(id));
        }

        try
        {
            var existingUser = userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new NotFoundException("User not found.");
            }

            userRepository.DeleteUser(id);
        }
        catch (Exception ex)
        {
            throw new UserServiceException("An error occurred while deleting user.", ex);
        }
    }

    public void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new UserServiceException("Password cannot be empty.");
        }

        bool hasUpperCase = Regex.IsMatch(password, "[A-Z]");
        bool hasNumber = Regex.IsMatch(password, "[0-9]");

        if (!hasUpperCase)
        {
            throw new UserServiceException("Password must include at least one capital letter.");
        }

        if (!hasNumber)
        {
            throw new UserServiceException("Password must include at least one number.");
        }

        if (password.Length < 8)
        {
            throw new UserServiceException("Password must be at least 8 characters long.");
        }
    }

}
