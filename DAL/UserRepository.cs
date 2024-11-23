using BLL.Interfaces;
using BLL.DTOs;
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

        public List<UserDTO> GetAllUsers()
        {
            List<UserDTO> users = new List<UserDTO>();
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                string query = @"SELECT id, username, email, password, wins, rating, games_played, profile_picture, date_joined 
                         FROM [Users] 
                         ORDER BY id DESC;";
                SqlCommand cmd = new SqlCommand(query, s);
                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UserDTO u = new UserDTO
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            Wins = reader.GetInt32(4),
                            Rating = reader.GetInt32(5),
                            GamesPlayed = reader.GetInt32(6),
                            ProfilePicture = !reader.IsDBNull(7) ? reader.GetString(7) : "/Images/Default.jpg",
                            DateJoined = reader.GetDateTime(8)
                        };
                        users.Add(u);
                    }
                }
            }
            return users;
        }

        public UserDTO GetUserById(int id)
        {
            UserDTO user = null;
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();
                string query = @"SELECT id, username, email, password, wins, rating, games_played, profile_picture, date_joined 
                         FROM [Users] 
                         WHERE id = @UserId";
                SqlCommand cmd = new SqlCommand(query, s);
                cmd.Parameters.AddWithValue("@UserId", id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserDTO
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            Wins = reader.GetInt32(4),
                            Rating = reader.GetInt32(5),
                            GamesPlayed = reader.GetInt32(6),
                            ProfilePicture = !reader.IsDBNull(7) ? reader.GetString(7) : "/Images/Default.jpg",
                            DateJoined = reader.GetDateTime(8)
                        };
                    }
                }
            }
            return user;
        }



        public UserDTO GetUserByNameAndPassword(string username, string password)
        {
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                string query = @"SELECT id, username, email, password, wins, rating, games_played, profile_picture, date_joined 
                         FROM [Users] 
                         WHERE username = @username AND password = @password";
                SqlCommand cmd = new SqlCommand(query, s);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserDTO
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            Wins = reader.GetInt32(4),
                            Rating = reader.GetInt32(5),
                            GamesPlayed = reader.GetInt32(6),
                            ProfilePicture = !reader.IsDBNull(7) ? reader.GetString(7) : "/Images/Default.jpg",
                            DateJoined = reader.GetDateTime(8)
                        };
                    }
                }
            }
            return null;
        }

        public bool ValidateUserCredentials(string username, string password, out int id)
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

        public void CreateUser(UserDTO user)
        {
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();
                string insertQuery = "INSERT INTO [Users] (username, email, password, profile_picture, date_joined) VALUES (@Username, @Email, @Password, @ProfilePicture, @DateJoined)";
                SqlCommand cmd = new SqlCommand(insertQuery, s);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@ProfilePicture", user.ProfilePicture ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateJoined", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
