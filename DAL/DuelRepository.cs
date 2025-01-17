﻿using BLL.Interfaces;
using BLL.Exceptions.Duel;
using BLL.DTOs;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq.Expressions;
using BLL.Exceptions.User;
using BLL.Exceptions;

namespace DAL
{
    public class DuelRepository : IDuelRepository
    {
        private readonly string connectionString;

        public DuelRepository(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public DuelDTO GetDuelById(int duelId)
        {
            try
            {
                DuelDTO duel = null;

                using (SqlConnection s = new SqlConnection(connectionString))
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
                                Participants = GetParticipantsByDuelId(reader.GetInt32(0))
                            };
                        }
                    }
                }

                if (duel == null)
                {
                    throw new NotFoundException($"Duel with ID {duelId} was not found.");
                }

                return duel;
            }
            catch (NotFoundException)
            {
                throw; // Propagate specific not found error
            }
            catch (Exception ex)
            {
                throw new DuelRepositoryException("An error occurred while retrieving the duel by ID.", ex);
            }
        }


        public List<DuelDTO> GetDuelsByUserId(int userId)
        {
            try
            {
                var duels = new List<DuelDTO>();

                using (SqlConnection s = new SqlConnection(connectionString))
                {
                    s.Open();

                    string query = @"
            SELECT 
                d2.Id AS DuelId,
                d2.Status AS DuelStatus,
                d2.DateCreated AS DuelDateCreated,
                dp.UserId AS ParticipantUserId,
                u.Username AS ParticipantUsername,
                dp.IsWinner AS ParticipantIsWinner
            FROM [DuelParticipants] dp
            INNER JOIN Duels d2 ON dp.DuelId = d2.Id
            INNER JOIN Users u ON u.Id = dp.UserId
            WHERE d2.Id IN (
                SELECT DuelId
                FROM [DuelParticipants]
                WHERE UserId = @UserId
            )
            ORDER BY d2.DateCreated DESC;";

                    SqlCommand cmd = new SqlCommand(query, s);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, DuelDTO> duelMap = new Dictionary<int, DuelDTO>();

                        while (reader.Read())
                        {
                            int duelId = reader.GetInt32(reader.GetOrdinal("DuelId"));

                            if (!duelMap.ContainsKey(duelId))
                            {
                                duelMap[duelId] = new DuelDTO
                                {
                                    Id = duelId,
                                    Status = reader.GetString(reader.GetOrdinal("DuelStatus")),
                                    DateCreated = reader.GetDateTime(reader.GetOrdinal("DuelDateCreated")),
                                    Participants = new List<DuelParticipantDTO>()
                                };
                            }

                            var participant = new DuelParticipantDTO
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("ParticipantUserId")),
                                Username = reader.GetString(reader.GetOrdinal("ParticipantUsername")),
                                IsWinner = reader.GetBoolean(reader.GetOrdinal("ParticipantIsWinner"))
                            };

                            duelMap[duelId].Participants.Add(participant);
                        }

                        duels = duelMap.Values.ToList();
                    }
                }

                if (duels.Count == 0)
                {
                    throw new NotFoundException($"No duels found for user ID {userId}.");
                }

                return duels;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DuelRepositoryException("An error occurred while retrieving duels by user ID.", ex);
            }
        }


        public List<DuelParticipantDTO> GetParticipantsByDuelId(int duelId)
        {
            try
            {
                List<DuelParticipantDTO> participants = new List<DuelParticipantDTO>();

                using (SqlConnection connection = new SqlConnection(connectionString))
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

                if (participants.Count == 0)
                {
                    throw new NotFoundException($"No participants found for duel ID {duelId}.");
                }

                return participants;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DuelRepositoryException("An error occurred while retrieving participants by duel ID.", ex);
            }
        }



        public int CreateDuel(int userId1, int userId2)
        {
            try
            {
                using (SqlConnection s = new SqlConnection(connectionString))
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

                            return duelId;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DuelRepositoryException("An error occurred while creating the duel.", ex);
            }
        }


        public void AssignWinner(int duelId, int winnerUserId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the duel exists
                            var participants = GetParticipantsByDuelId(duelId);
                            if (participants.All(p => p.UserId != winnerUserId))
                            {
                                throw new NotFoundException($"The user with ID {winnerUserId} is not a participant in duel ID {duelId}.");
                            }

                            // Update Participants
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
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DuelRepositoryException("An error occurred while assigning the winner.", ex);
            }
        }



    }
}
