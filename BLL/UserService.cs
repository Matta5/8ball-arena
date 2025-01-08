using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;
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
		catch (Exception ex)
		{
			throw new UserServiceException("An error occurred while retrieving all users.", ex);
		}
	}

	public UserDTO GetUserById(int id)
	{
			var user = userRepository.GetUserById(id);
			if (user == null)
			{
				throw new NotFoundException("User not found.");
			}
			return user;
	}

	public UserDTO GetUserByUsername(string username)
	{
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
		try
		{
			return userRepository.ValidateUserCredentials(username, password, out id);
		}
		catch (Exception ex)
		{
			throw new UserServiceException("An error occurred while validating user credentials.", ex);
		}
	}

	public bool CreateUser(CreateUserDTO user, out string errorMessage)
	{
		errorMessage = null;
		try
		{
			string passwordValidationResult = IsValidPassword(user.Password);
			if (passwordValidationResult != null)
			{
				errorMessage = passwordValidationResult;
				return false;
			}
			user.DateJoined = DateTime.Now;
			userRepository.CreateUser(user);
			return true;
		}
		catch (Exception ex)
		{
			throw new UserServiceException("An error occurred while creating a new user.", ex);
		}
	}


	public void EditUser(int id, EditUserDTO user)
	{
		try
		{
			var existingUser = userRepository.GetUserById(id);
			if (existingUser == null)
			{
				throw new NotFoundException("User not found.");
			}
			userRepository.EditUser(id, user);
		}
		catch (Exception ex)
		{
			throw new UserServiceException("An error occurred while editing user.", ex);
		}
	}

	public void DeleteUser(int id)
	{
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
