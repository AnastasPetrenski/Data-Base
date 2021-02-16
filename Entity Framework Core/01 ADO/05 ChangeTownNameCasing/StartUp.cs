using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace _05_ChangeTownNameCasing
{
    class StartUp
    {
        static void Main()
        {
            Console.WriteLine("Country name:");
            string countryName = Console.ReadLine();

            string connectionWithPass = "Server=.;Database=MinionsDB;User Id=Client;Password=1234567";

            using SqlConnection sqlConnection = new SqlConnection(connectionWithPass);

            sqlConnection.Open();

            //try SqlCommandbuilder
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Towns", sqlConnection);

            SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder(adapter);

            DataSet ds = new DataSet("Towns");
            adapter.Fill(ds, "Towns");

            DataTable table = ds.Tables["Towns"];
            DataRow row = table.NewRow();
            row["Name"] = "Petrich";
            row["CountryCode"] = "1";
            table.Rows.Add(row);

            adapter.Update(ds, "Towns");
            //MessageBox.Show

            var countOfChangedTowns = GetNumberOfChangedTowns(sqlConnection, countryName);

            if (countOfChangedTowns == "0")
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                var result = GetNamesOfChangedTowns(countOfChangedTowns, sqlConnection, countryName);

                Console.WriteLine(result);
            }

            sqlConnection.Close();
            sqlConnection.Open();

            int deletedCount = DeleteFromTowns(sqlConnection);

            Console.WriteLine($"{deletedCount} towns was removed from Db MinionsDS, table Towns.");
        }

        private static int DeleteFromTowns(SqlConnection sqlConnection)
        {
            string[] townToBeDeleted = new string[] { "Petrich", "1" };
            string sqlQueryForDeletingTowns = "DELETE FROM Towns WHERE Name = @Town AND CountryCode = @Code";

            using SqlCommand command = new SqlCommand(sqlQueryForDeletingTowns, sqlConnection);
            command.Parameters.AddRange(new[]
            {
                new SqlParameter("@Town", townToBeDeleted[0]),
                new SqlParameter("@Code", townToBeDeleted[1])
            });

            return command.ExecuteNonQuery();
        }

        private static string GetNamesOfChangedTowns(string countOfChangedTowns, SqlConnection sqlConnection, string countryName)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{countOfChangedTowns} town names were affected.");

            var sqlQueryGetAllChangedTownsName = "SELECT Name FROM Towns WHERE CountryCode = (SELECT Id FROM Countries WHERE Name = @Country)";

            using (SqlCommand command = new SqlCommand(sqlQueryGetAllChangedTownsName, sqlConnection))
            {
                command.Parameters.AddWithValue("@Country", countryName);

                var reader = command.ExecuteReader();

                List<string> towns = new List<string>();

                while (reader.Read())
                {
                    string townName = reader["Name"]?.ToString();
                    towns.Add(townName);
                }
                sb.AppendLine($"[{string.Join(", ", towns)}]");
            }

            return sb.ToString().TrimEnd();
        }

        private static string GetNumberOfChangedTowns(SqlConnection sqlConnection, string countryName)
        {
            string sqlQueryUpdateTownsToUppercase = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT Id FROM Countries WHERE Name = @Country)";

            using SqlCommand command = new SqlCommand(sqlQueryUpdateTownsToUppercase, sqlConnection);
            command.Parameters.AddWithValue("@Country", countryName);

            return command.ExecuteNonQuery().ToString();
        }
    }
}
