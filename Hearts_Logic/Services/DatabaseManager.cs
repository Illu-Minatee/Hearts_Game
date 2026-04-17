using Npgsql;
using System.Collections.Generic;

namespace Hearts_Logic.Services
{
    public class DatabaseManager
    {
        private string _connectionString =
            "Host=localhost;Port=5432;Username=postgres;Password=1377;Database=hearts_db";

        public bool TestConnection()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void SaveScore(string playerName, int score)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO game_scores (player_name, total_score)
                    VALUES (@name, @score)
                    ON CONFLICT (player_name)
                    DO UPDATE SET total_score = EXCLUDED.total_score;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int LoadScore(string playerName)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT total_score FROM game_scores WHERE player_name = @name";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", playerName);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return int.Parse(result.ToString());
                    }
                }
            }

            return 0;
        }

        public void SaveAllScores(List<Hearts_Logic.Actors.Player> players)
        {
            foreach (var player in players)
            {
                SaveScore(player.Name, player.Score);
            }
        }

        public void LoadAllScores(List<Hearts_Logic.Actors.Player> players)
        {
            foreach (var player in players)
            {
                player.Score = LoadScore(player.Name);
            }
        }

        public void ResetAllScores(List<Hearts_Logic.Actors.Player> players)
        {
            foreach (var player in players)
            {
                SaveScore(player.Name, 0);
            }
        }
        public string GetSavedHumanPlayerName()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT player_name FROM game_scores " +
                               "WHERE player_name <> 'CPU West' " +
                               "AND player_name <> 'CPU North' " +
                               "AND player_name <> 'CPU East' " +
                               "LIMIT 1";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return "Player";
        }

        public void SaveCurrentPlayerName(string playerName)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO game_settings (setting_key, setting_value)
            VALUES ('current_player_name', @name)
            ON CONFLICT (setting_key)
            DO UPDATE SET setting_value = EXCLUDED.setting_value;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string LoadCurrentPlayerName()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT setting_value FROM game_settings WHERE setting_key = 'current_player_name'";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return "Player";
        }

    }
}