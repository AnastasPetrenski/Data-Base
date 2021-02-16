using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _08_IncreaseMinionAge
{
    public class StartUp
    {
        static void Main()
        {
            var minionsIds = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            string connection = "Server=.;Database=MinionsDB;User Id=Client;Password=1234567";
            string sqlQueryAgeIncreaseByOne = "UPDATE Minions SET Age += 1, Name = UPPER(LEFT(Name,1)) + RIGHT(Name, LEN(Name) - 1) WHERE Id = @id";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            using SqlCommand sqlCommand = new SqlCommand(sqlQueryAgeIncreaseByOne, sqlConnection);
            sqlCommand.Parameters.Add("@id", System.Data.SqlDbType.Int);

            foreach (var id in minionsIds)
            {
                sqlCommand.Parameters[0].Value = id;
                sqlCommand.ExecuteNonQuery();
            }

            string sqlQueryGetMinionsInfo = "SELECT Name, Age FROM Minions";
            using SqlCommand command = new SqlCommand(sqlQueryGetMinionsInfo, sqlConnection);

            var reader = command.ExecuteReader();

            List<string> info = new List<string>();

            while (reader.Read())
            {
                string name = reader["Name"] as string;
                int? age = reader["Age"] as int?;

                string result = name + " " + age;
                info.Add(result);
            }

            Console.WriteLine(string.Join(Environment.NewLine, info));
        }
    }
}
