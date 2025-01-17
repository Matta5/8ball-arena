using BLL.DTOs;
using BLL.Interfaces;
using BLL.Exceptions;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class CommentRepository : ICommentRepository
    {
        private readonly string connectionString;

        public CommentRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public List<CommentDTO> GetCommentsByDuelId(int duelId)
        {
            var comments = new List<CommentDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, UserId, DuelId, Text, Date FROM Comments WHERE DuelId = @DuelId";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DuelId", duelId);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new CommentDTO
                        {
                            Id = reader.GetInt32(0),
                            UserId = reader.GetInt32(1),
                            DuelId = reader.GetInt32(2),
                            Text = reader.GetString(3),
                            DateTime = reader.GetDateTime(4)
                        });
                    }
                }
            }

            return comments;
        }

        public void AddComment(CommentDTO comment)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Comments (UserId, DuelId, Text, Date) VALUES (@UserId, @DuelId, @Text, @Date)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", comment.UserId);
                cmd.Parameters.AddWithValue("@DuelId", comment.DuelId);
                cmd.Parameters.AddWithValue("@Text", comment.Text);
                cmd.Parameters.AddWithValue("@Date", comment.DateTime);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
