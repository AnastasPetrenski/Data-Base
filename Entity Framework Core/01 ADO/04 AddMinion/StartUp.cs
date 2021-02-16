using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Text;

namespace _04_AddMinion
{
    class StartUp
    {
        private const string connection = "Server=.;Database=MinionsDB;Integrated security=true;";

        static void Main()
        {
            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            string[] minionsInput = ReadInput();
            string[] villianInput = ReadInput();

            string result = AddMinionToVillian(sqlConnection, minionsInput, villianInput);

            Console.WriteLine(result);
        }

        private static string AddMinionToVillian(SqlConnection sqlConnection, string[] minionsInput, string[] villianInput)
        {
            var output = new StringBuilder();

            string townName = minionsInput[2];
            string townId = GetTownId(sqlConnection, townName, output);

            string villainName = villianInput[0];
            string villainId = GetVillainID(sqlConnection, villainName, output);

            string minionName = minionsInput[0];
            string minionAge = minionsInput[1];
            string minionId = GetMinionID(sqlConnection, minionName, minionAge, townId);

            string addMinionToVillainQuery = "INSERT INTO MinionsVillains(MinionsId, VillainId) VALUES (@minionID, @villainID)";

            using SqlCommand addMinionToVillainCmd = new SqlCommand(addMinionToVillainQuery, sqlConnection);
            addMinionToVillainCmd.Parameters.AddRange(new[]
            {
                new SqlParameter("@minionID", minionId),
                new SqlParameter("@villainID", villainId)
            });

            var result = addMinionToVillainCmd.ExecuteScalar()?.ToString();
            //try
            //{
            //    addMinionToVillainCmd.ExecuteScalar();
            //}
            //catch (Exception)
            //{
            //    return "Already added!";
            //    //throw new ArgumentException(String.Format("Already added!"));
            //}


            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

            return output.ToString().TrimEnd();
        }

        /// <summary>
        /// Get minion if it exists. Else add the minion into the DB and return the newest ID.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="minionName"></param>
        /// <param name="minionAge"></param>
        /// <param name="townID"></param>
        /// <returns></returns>
        private static string GetMinionID(SqlConnection sqlConnection, string minionName, string minionAge, string townID)
        {
            string getMinionIdQuery = "SELECT Id FROM Minions WHERE [Name] = @minionName AND Age = @minionAge";
            using SqlCommand getMinionIdCmd= new SqlCommand(getMinionIdQuery, sqlConnection);
            getMinionIdCmd.Parameters.AddWithValue("@minionName", minionName);
            getMinionIdCmd.Parameters.AddWithValue("@minionAge", minionAge);
            string minionID = getMinionIdCmd.ExecuteScalar()?.ToString();

            if (minionID == null)  
            {
                string insertIntoMinionsQuery = "INSERT INTO Minions ([Name], Age, TownID) VALUES (@minionName, @age, @townID)";
                using SqlCommand insertIntoMinionsCmd = new SqlCommand(insertIntoMinionsQuery, sqlConnection);
                insertIntoMinionsCmd.Parameters.AddRange(new[]
                {
                    new SqlParameter("@minionName", minionName),
                    new SqlParameter("@age", minionAge),
                    new SqlParameter("townID", townID)
            });
                insertIntoMinionsCmd.ExecuteNonQuery();

                minionID = getMinionIdCmd.ExecuteScalar().ToString();
            }

            return minionID;
        }

        /// <summary>
        /// Get villain ID from database. If it does not exist, add it to DB and return the newest villain's ID.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="villainName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static string GetVillainID(SqlConnection sqlConnection, string villainName, StringBuilder output)
        {
            string getVillainIdQuery = "SELECT Id FROM Villains WHERE [Name] = @villainName";
            using SqlCommand getVillainIdCmd = new SqlCommand(getVillainIdQuery, sqlConnection);
            getVillainIdCmd.Parameters.AddWithValue("@villainName", villainName);
            string villainID = getVillainIdCmd.ExecuteScalar()?.ToString();

            if (villainID == null)
            {
                string getFactorIdQuery = "SELECT Id FROM EvilnessFactors WHERE[Name] = 'Evil'";
                using SqlCommand getFactorIdCmd = new SqlCommand(getFactorIdQuery, sqlConnection);
                string factorId = getFactorIdCmd.ExecuteScalar()?.ToString();

                string insertVillainIntoDB = "INSERT INTO Villains([Name], EvilnessFactorID) VALUES(@villainName, @factorId)";
                using SqlCommand insertVillainCmd = new SqlCommand(insertVillainIntoDB, sqlConnection);
                insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                insertVillainCmd.Parameters.AddWithValue("@factorId", factorId);
                insertVillainCmd.ExecuteNonQuery();

                villainID = getVillainIdCmd.ExecuteScalar().ToString();

                output.AppendLine($"Villain {villainName} was added to the database.");
            }

            return villainID;
        }

        /// <summary>
        /// Check if given town exists in the database. If it does not exist, then insert it and return townId.
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="townName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static string GetTownId(SqlConnection sqlConnection, string townName, StringBuilder output)
        {
            string getTownIdQuery = "SELECT Id FROM Towns WHERE[Name] = @townName";

            using SqlCommand getTownIdCmd = new SqlCommand(getTownIdQuery, sqlConnection);
            getTownIdCmd.Parameters.AddWithValue("@townName", townName);
            string townId = getTownIdCmd.ExecuteScalar()?.ToString();

            if (townId == null)
            {
                string insertTownQuery = "INSERT INTO Towns ([Name], CountryCode) VALUES (@townName, 1)";
                using SqlCommand insertTownCmd = new SqlCommand(insertTownQuery, sqlConnection);
                insertTownCmd.Parameters.AddWithValue("@townName", townName);
                insertTownCmd.ExecuteNonQuery();

                townId = getTownIdCmd.ExecuteScalar().ToString();

                output.AppendLine($"Town {townName} was added to the database.");
            }

            return townId;
        }

        private static string[] ReadInput()
        {
            return Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1)
                .ToArray();
        }
    }
}
