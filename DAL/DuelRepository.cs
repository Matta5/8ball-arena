using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BLL.DTOs;

namespace DAL.Repositories
{
    public class DuelRepository
    {
        private readonly string _connectionString;

        public DuelRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DuelDTO GetDuelById(int duelId)
        {
            DuelDTO duel = null;

            using (SqlConnection s = new SqlConnection(_connectionString))
            {
                string query = @"SELECT d.Id, d.Status, d.DateCreated
                         FROM [Duels] d
                         WHERE d.Id = @DuelId;";
                SqlCommand cmd = new SqlCommand(query, s);
                cmd.Parameters.AddWithValue("@DuelId", duelId);

                s.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        duel = new DuelDTO
                        {
                            Id = reader.GetInt32(0),
                            Status = reader.GetString(1),
                            DateCreated = reader.GetDateTime(2),
                            Participants = GetParticipantsByDuelId(reader.GetInt32(0)) // Fetch participants
                        };
                    }
                }
            }

            return duel;
        }


        public List<DuelDTO> GetDuelsByUserId(int userId)
        {
            List<DuelDTO> duels = new List<DuelDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT d.Id AS DuelId, d.DateCreated, d.Status, 
                   dp.UserId AS ParticipantUserId
            FROM DuelParticipants dp
            JOIN Duels d ON dp.DuelId = d.Id
            WHERE dp.UserId = @UserId
            ORDER BY d.DateCreated DESC;";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DuelDTO duel = new DuelDTO
                        {
                            Id = reader.GetInt32(0),
                            DateCreated = reader.GetDateTime(1),
                            Status = reader.GetString(2),
                            ParticipantUserId = reader.GetInt32(3)
                        };
                        duels.Add(duel);
                    }
                }
            }

            return duels;
        }



        public List<DuelParticipantDTO> GetParticipantsByDuelId(int duelId)
        {
            List<DuelParticipantDTO> participants = new List<DuelParticipantDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT dp.UserId, u.Username, dp.IsWinner
            FROM DuelParticipants dp
            JOIN Users u ON dp.UserId = u.Id
            WHERE dp.DuelId = @DuelId;";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DuelId", duelId);

                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        participants.Add(new DuelParticipantDTO
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            IsWinner = reader.GetBoolean(2)
                        });
                    }
                }
            }

            return participants;
        }


        public void CreateDuel(int userId1, int userId2)
        {
            using (SqlConnection s = new SqlConnection(_connectionString))
            {
                s.Open();

                using (SqlTransaction transaction = s.BeginTransaction())
                {
                    try
                    {
                        // Insert the Duel
                        string insertDuelQuery = @"INSERT INTO [Duels] (Status, DateCreated)
                                           OUTPUT INSERTED.Id
                                           VALUES ('Pending', @DateCreated);";

                        SqlCommand duelCmd = new SqlCommand(insertDuelQuery, s, transaction);
                        duelCmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);

                        int duelId = (int)duelCmd.ExecuteScalar();

                        // Insert Participants
                        string insertParticipantQuery = @"INSERT INTO [DuelParticipants] (DuelId, UserId, IsWinner)
                                                  VALUES (@DuelId, @UserId, @IsWinner);";

                        SqlCommand participantCmd = new SqlCommand(insertParticipantQuery, s, transaction);
                        participantCmd.Parameters.AddWithValue("@DuelId", duelId);
                        participantCmd.Parameters.AddWithValue("@IsWinner", false);

                        participantCmd.Parameters.AddWithValue("@UserId", userId1);
                        participantCmd.ExecuteNonQuery();
                        participantCmd.Parameters.Clear();

                        participantCmd.Parameters.AddWithValue("@DuelId", duelId);
                        participantCmd.Parameters.AddWithValue("@UserId", userId2);
                        participantCmd.Parameters.AddWithValue("@IsWinner", false);
                        participantCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AssignWinner(int duelId, int winnerUserId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateParticipantsQuery = @"
                    UPDATE DuelParticipants
                    SET IsWinner = CASE 
                        WHEN UserId = @WinnerUserId THEN 1
                        ELSE 0
                    END
                    WHERE DuelId = @DuelId;";

                        SqlCommand participantsCmd = new SqlCommand(updateParticipantsQuery, connection, transaction);
                        participantsCmd.Parameters.AddWithValue("@WinnerUserId", winnerUserId);
                        participantsCmd.Parameters.AddWithValue("@DuelId", duelId);
                        participantsCmd.ExecuteNonQuery();

                        string updateDuelQuery = @"
                    UPDATE Duels
                    SET Status = 'Completed'
                    WHERE Id = @DuelId;";

                        SqlCommand duelCmd = new SqlCommand(updateDuelQuery, connection, transaction);
                        duelCmd.Parameters.AddWithValue("@DuelId", duelId);
                        duelCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


    }
}
