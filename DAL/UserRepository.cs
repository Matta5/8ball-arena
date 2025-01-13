using BLL.Interfaces;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Exceptions.User;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public UserDTO GetUserById(int id)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    string userQuery = @"SELECT id, username, email, password, rating, profile_picture, date_joined 
                                 FROM [Users] 
                                 WHERE id = @UserId";
                    SqlCommand userCmd = new SqlCommand(userQuery, s);
                    userCmd.Parameters.AddWithValue("@UserId", id);

                    using (SqlDataReader userReader = userCmd.ExecuteReader())
                    {
                        if (!userReader.HasRows) 
                        {
                            throw new NotFoundException("User not found.");
                        }

                        userReader.Read();
                        return MapUserDTO(userReader);
                    }
                }
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }
            catch (SqlException sqlEx)
            {
                throw new UserRepositoryException("Database error occurred while fetching user by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while fetching the user by ID.", ex);
            }
        }


        public UserDTO GetUserByUsername(string username)
        {
            try
            {
                UserDTO user = null;
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    string query = @"SELECT id, username, email, password, rating, profile_picture, date_joined 
                             FROM [Users] 
                             WHERE username = @username";
                    SqlCommand cmd = new SqlCommand(query, s);
                    cmd.Parameters.AddWithValue("@username", username);
                    s.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (!reader.HasRows)
                        {
                            throw new NotFoundException("User not found.");
                        }
                        return MapUserDTO(reader);
                    }
                }
            }
            catch (UserRepositoryException ex)
            {
                throw ex;
            }
            catch (SqlException sqlEx)
            {
                throw new UserRepositoryException("Database error occurred while fetching user by username.", sqlEx);
            }
        }

        public bool CheckIfUsernameExists(string username)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM [Users] WHERE username = @username";
                    SqlCommand cmd = new SqlCommand(query, s);
                    cmd.Parameters.AddWithValue("@username", username);

                    s.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while checking if username exists.", ex);
            }
        }


        public List<UserDTO> GetAllUsers()
        {
            List<UserDTO> users = new List<UserDTO>();
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    string query = @"SELECT id, username, email, password, rating, profile_picture, date_joined 
                             FROM [Users] 
                             ORDER BY id DESC;";
                    SqlCommand cmd = new SqlCommand(query, s);
                    s.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(MapUserDTO(reader));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while fetching all users.", ex);
            }
            return users;
        }

        public void CreateUser(CreateUserDTO createEditUserDTO)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    string insertQuery = @"INSERT INTO [Users] 
                                   (username, email, password, profile_picture, date_joined) 
                                   VALUES (@Username, @Email, @Password, @ProfilePicture, @DateJoined)";
                    SqlCommand cmd = new SqlCommand(insertQuery, s);
                    cmd.Parameters.AddWithValue("@Username", createEditUserDTO.Username);
                    cmd.Parameters.AddWithValue("@Email", createEditUserDTO.Email);
                    cmd.Parameters.AddWithValue("@Password", createEditUserDTO.Password);
                    cmd.Parameters.AddWithValue("@ProfilePicture", createEditUserDTO.ProfilePicture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateJoined", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new UserRepositoryException("Database error occurred while creating user.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while creating user.", ex);
            }
        }

        public void EditUser(int id, EditUserDTO createEditUserDTO)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    string updateQuery = @"UPDATE [Users] 
                                   SET username = @Username, 
                                       email = @Email, 
                                       profile_picture = @ProfilePicture 
                                   WHERE id = @id";

                    SqlCommand cmd = new SqlCommand(updateQuery, s);
                    cmd.Parameters.AddWithValue("@Username", createEditUserDTO.Username);
                    cmd.Parameters.AddWithValue("@Email", createEditUserDTO.Email);
                    cmd.Parameters.AddWithValue("@ProfilePicture", createEditUserDTO.ProfilePicture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new NotFoundException("User not found.");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new UserRepositoryException("Database error occurred while updating user.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while updating user.", ex);
            }
        }

        
        public bool ValidateUserCredentials(string username, string password, out int id)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    string query = "SELECT id FROM [Users] WHERE username = @username AND password = @password";
                    SqlCommand cmd = new SqlCommand(query, s);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    s.Open();
                    var result = cmd.ExecuteScalar();
                    id = -1;
                    if (result != null)
                    {
                        id = (int)result;
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while validating user credentials.", ex);
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();
                    string deleteQuery = @"DELETE FROM [Users] WHERE id = @UserId";
                    SqlCommand cmd = new SqlCommand(deleteQuery, s);
                    cmd.Parameters.AddWithValue("@UserId", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new NotFoundException("User not found.");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new UserRepositoryException("Database error occurred while deleting user.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("An error occurred while deleting user.", ex);
            }
        }

        private UserDTO MapUserDTO(SqlDataReader reader)
        {
            return new UserDTO
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3),
                Rating = reader.GetInt32(4),
                ProfilePicture = !reader.IsDBNull(5) ? reader.GetString(5) : "/Images/Default.jpg",
                DateJoined = reader.GetDateTime(6)
            };
        }


    }
}
