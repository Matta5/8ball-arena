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
                        users.Add(u);
                    }
                }
                return users;
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
