using Microsoft.Data.SqlClient;
using System;

namespace _09_IncreaseAgeStoredProcedure
{
    public class StartUp
    {
        static void Main()
        {
            int id = int.Parse(Console.ReadLine());

            string connection = "Server=.;Database=MinionsDB;User Id=Client;Password=1234567";
            string sqlQueryExecuteProcedure = "EXEC dbo.usp_GetOlder @id";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            using SqlCommand sqlCommand = new SqlCommand(sqlQueryExecuteProcedure, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", id);

            var reader = sqlCommand.ExecuteReader();
            string result = string.Empty;

            while (reader.Read())
            {
                string name = reader["Name"] as string;
                int? age = reader["Age"] as int?;

                result = name + " " + age;
            }

            Console.WriteLine(result);
        }
    }
}
