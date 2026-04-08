using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Hearts_Logic.Services
{
    public class DatabaseManager
    {
        private string connectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=1377;Database=hearts_db";
    public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}