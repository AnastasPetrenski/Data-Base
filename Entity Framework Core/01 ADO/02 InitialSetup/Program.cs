using Microsoft.Data.SqlClient;
using System;

namespace _02_InitialSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = "Server=.;Database=MinionsDB;Integrated security=true";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string selectAllVallians = "SELECT v.[Name], COUNT(mv.MinionsId) AS MinionsCount FROM Villains AS v JOIN MinionsVillains AS mv ON v.Id = mv.MinionsId GROUP BY v.[Name] ORDER BY MinionsCount DESC";

            using SqlCommand sqlCommand = new SqlCommand(selectAllVallians, sqlConnection);
            using SqlDataReader reader = sqlCommand.ExecuteReader();

            using SqlDataAdapter adapter = new SqlDataAdapter();
            var param = adapter.GetFillParameters();
            var len = param.Length;

            while (reader.Read())
            {
                string name = (string)reader["Name"];
                int minionsCount = (int)reader["MinionsCount"];
                if (minionsCount >= 3)
                {
                    Console.WriteLine($"{name} - {minionsCount}");
                }
            }
        }
    }
}
