using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _07_PrinAllMinionsName
{
    public class Startup
    {
        static void Main()
        {
            string connection = "Server=.;Database=MinionsDB;User Id=Client;Password=1234567";
            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string sqlQueryReadMinions = "SELECT Name FROM Minions";

            using SqlCommand sqlCommand = new SqlCommand(sqlQueryReadMinions, sqlConnection);

            var reader = sqlCommand.ExecuteReader();

            List<string> names = new List<string>();

            while (reader.Read())
            {
                string name = reader["Name"] as string;
                names.Add(name);
            }



            for (int i = 0, z = names.Count - 1; i < names.Count/2; i++)
            {
                Console.WriteLine(names[i]);
                Console.WriteLine(names[z - i]);
                if (names.Count % 2 != 0 && names.Count / 2 == i+1)
                {
                    Console.WriteLine(names[i+1]);
                    break;
                }

            }
        }
    }
}
