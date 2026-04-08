using Npgsql;
using Hearts_Logic.Actors;

namespace Hearts_Logic.Services
{
    public class StatsService
    {
        private DatabaseManager db = new DatabaseManager();

        public void UpdatePlayerStats(Player player, bool isWinner)
        {
            using var conn = db.GetConnection();
            conn.Open();

            string checkQuery = "SELECT COUNT(*) FROM player_stats WHERE player_name = @name";
            using var checkCmd = new NpgsqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("name", player.Name);

            long count = (long)checkCmd.ExecuteScalar();

            if (count == 0)
            {
                string insertQuery = @"
                    INSERT INTO player_stats (player_name, wins, losses, total_score)
                    VALUES (@name, @wins, @losses, @score)";

                using var insertCmd = new NpgsqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("name", player.Name);
                insertCmd.Parameters.AddWithValue("wins", isWinner ? 1 : 0);
                insertCmd.Parameters.AddWithValue("losses", isWinner ? 0 : 1);
                insertCmd.Parameters.AddWithValue("score", player.Score);

                insertCmd.ExecuteNonQuery();
            }
            else
            {
                string updateQuery = @"
                    UPDATE player_stats
                    SET wins = wins + @wins,
                        losses = losses + @losses,
                        total_score = total_score + @score
                    WHERE player_name = @name";

                using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("name", player.Name);
                updateCmd.Parameters.AddWithValue("wins", isWinner ? 1 : 0);
                updateCmd.Parameters.AddWithValue("losses", isWinner ? 0 : 1);
                updateCmd.Parameters.AddWithValue("score", player.Score);

                updateCmd.ExecuteNonQuery();
            }
        }
    }
}