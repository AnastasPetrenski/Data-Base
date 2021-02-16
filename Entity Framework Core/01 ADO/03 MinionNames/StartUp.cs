using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace _03_MinionNames
{
    class StartUp
    {
        private const string connection = @"Server=.;Database=MinionsDB;Integrated security=true;";

        static void Main(string[] args)
        {
            int villainID = int.Parse(Console.ReadLine());

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string result = GetMinionsInfoAboutVilliansID(sqlConnection, villainID);

            Console.WriteLine(result);

        }

        private static string GetMinionsInfoAboutVilliansID(SqlConnection sqlConnection, int villainID)
        {
            var sb = new StringBuilder();

            string getVillionName = "SELECT [Name] FROM Villains WHERE Villains.Id = @villianID";
            using SqlCommand sqlCommandCheckID = new SqlCommand(getVillionName, sqlConnection);
            sqlCommandCheckID.Parameters.AddWithValue("@villianID", villainID);

            string villianName = sqlCommandCheckID.ExecuteScalar()?.ToString();
            if (villianName == null)
            {
                sb.AppendLine($"No villain with ID {villainID} exists in the database.");
            }
            else
            {
                string queryGetMinions = "SELECT m.[Name], m.Age FROM Minions AS m JOIN MinionsVillains AS mv ON m.Id = mv.MinionsId JOIN Villains AS v ON mv.VillainId = v.Id WHERE mv.VillainId = @VillianID ORDER BY m.[Name]";

                SqlCommand sqlCommand = new SqlCommand(queryGetMinions, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@VillianID", villainID);

                using SqlDataReader reader = sqlCommand.ExecuteReader();
                List<object> rows = new List<object>();

                sb.AppendLine($"Villain: {villianName}");
                if (reader.HasRows)
                {
                    int count = 1;
                    while (reader.Read())
                    {
                        var columnsValue = new object[reader.FieldCount];
                        reader.GetValues(columnsValue);
                        for (int i = 0; i < columnsValue.Length; i++)
                        {
                            var obj = GetObjectValue<object>(reader, i); 
                                
                            rows.Add(obj);
                        }
                        

                        string minionName = reader["Name"]?.ToString();
                        string minionAge = reader["Age"]?.ToString();
                        sb.AppendLine($"{count++}. {minionName} {minionAge}");
                    }
                }
                else
                {
                    sb.AppendLine("(no minions)");
                }
            }
            return sb.ToString().TrimEnd();

        }

        private static object GetObjectValue<T>(SqlDataReader reader, int i)
        {
           var rows = new List<T>();
           return reader.GetFieldValue<T>(i);
        }
    }
}
