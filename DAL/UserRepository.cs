using BLL.Interfaces;
using BLL.Models;
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

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User]", s);
                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User u = new User();
                        u.id = reader.GetInt32(0);
                        u.username = reader.GetString(1);
                        u.email = reader.GetString(2);
                        u.password = reader.GetString(3);
                        u.wins = reader.GetInt32(4);
                        u.rating = reader.GetInt32(5);
                        u.gamesPlayed = reader.GetInt32(6);
                        // Check if profile_picture is NULL
                        if (!reader.IsDBNull(7))
                        {
                            u.profile_picture = reader.GetString(7);
                        }
                        else
                        {
                            // Assign a default value if profile_picture is NULL
                            u.profile_picture = "Images/Default.jpg";
                        }
                        users.Add(u);
                    }
                }
                return users;
            }
        }

        public User GetUserById(int id)
        {
            User user = null;

            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();

                string selectQuery = "SELECT * FROM [User] WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(selectQuery, s);
                cmd.Parameters.AddWithValue("@UserId", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            id = reader.GetInt32(0),
                            username = reader.GetString(1),
                            email = reader.GetString(2),
                            password = reader.GetString(3)
                        };
                    }
                }
            }

            return user;
        }

        public void CreateUser(User user)
        {
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                s.Open();

                string insertQuery = "INSERT INTO [User] (username, email, password, profile_picture) VALUES (@Username, @Email, @Password, @ProfilePicture)";
                SqlCommand cmd = new SqlCommand(insertQuery, s);
                cmd.Parameters.AddWithValue("@Username", user.username);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Password", user.password);
                cmd.Parameters.AddWithValue("@ProfilePicture", user.profile_picture ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public User GetUserByNameAndPassword(string username, string password)
        {
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE username = @username AND password = @password", s);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User u = new User();
                        u.id = reader.GetInt32(0);
                        u.username = reader.GetString(1);
                        u.email = reader.GetString(2);
                        u.password = reader.GetString(3);
                        u.wins = reader.GetInt32(4);
                        u.rating = reader.GetInt32(5);
                        u.gamesPlayed = reader.GetInt32(6);
                        return u;
                    }
                }
            }
            return null;
        }

        public bool ValidateUserCredentials(string username, string password)
        {
            using (SqlConnection s = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM [User] WHERE username = @username AND password = @password", s);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                s.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 1;
            }
        }
    }
}
