using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace _06_RemoveVillain
{
    class StartUp
    {
        static void Main()
        {
            string villainId = Console.ReadLine();

            string connection = "Server=.;Database=MinionsDB;Integrated security = true";

            using SqlConnection sqlConnection = new SqlConnection(connection);

            sqlConnection.Open();

            var villain = GetVillain(villainId, sqlConnection);

            sqlConnection.Close();
            
            if (villain.Count == 0)
            {
                Console.WriteLine("No such villain was found.");
            }
            else
            {
                var sb = new StringBuilder();

                sqlConnection.Open();

                var servingMinionsIds = GetServingMinionsIds(villainId, sqlConnection);

                sqlConnection.Close();
                sqlConnection.Open();

                using SqlTransaction transaction = sqlConnection.BeginTransaction();

                DeleteVillainFromDatabase(villainId, sqlConnection, transaction);

                sb.AppendLine($"{villain[0]} was deleted.");
                sb.AppendLine($"{servingMinionsIds.Count} minions were released.");

                Console.WriteLine(sb.ToString().TrimEnd());

                InsertBackDeletedVillain(sqlConnection, servingMinionsIds, villain, transaction);


            }
            
        }

        private static void InsertBackDeletedVillain(SqlConnection sqlConnection, List<string> servingMinionsIds, List<string> villain, SqlTransaction transaction)
        {
            string sqlQueryInsertVillain = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@Name, @FactorId)";

            using SqlCommand sqlCommand = new SqlCommand(sqlQueryInsertVillain, sqlConnection, transaction);
            sqlCommand.Parameters.AddRange(new[]
            {
                new SqlParameter("@Name", villain[0]),
                new SqlParameter("@FactorId", villain[1])
            });

            int result = sqlCommand.ExecuteNonQuery();

            if (result != 1)
            {
                transaction.Rollback();
            }

            string sqlQueryGetVillainNewId = "SELECT Id FROM Villains WHERE Name = @name";

            using SqlCommand sqlCommandGetId = new SqlCommand(sqlQueryGetVillainNewId, sqlConnection, transaction);
            sqlCommandGetId.Parameters.AddWithValue("@name", villain[0]);

            string villainId = sqlCommandGetId.ExecuteScalar()?.ToString();


            string sqlQueryAddMinionsToVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            using SqlCommand sqlCommandMinions = new SqlCommand(sqlQueryAddMinionsToVillain, sqlConnection, transaction);
            sqlCommandMinions.Parameters.Add(new SqlParameter("@minionId", System.Data.SqlDbType.VarChar));
            sqlCommandMinions.Parameters.Add(new SqlParameter("@villainId", villainId));

            try
            {
                foreach (var minionId in servingMinionsIds)
                {
                    sqlCommandMinions.Parameters[0].Value = minionId;
                    if (sqlCommandMinions.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidProgramException();
                    }
                }

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }


            transaction.Commit();

        }

        private static void DeleteVillainFromDatabase(string villainId, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            string sqlQueryDeleteMinions = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            string sqlQueryDeleteVillain = "DELETE FROM Villains WHERE Id = @villainId";

            using SqlCommand commandMinions = new SqlCommand(sqlQueryDeleteMinions, sqlConnection, transaction);
            commandMinions.Parameters.AddWithValue("@villainId", villainId);
            commandMinions.ExecuteNonQuery();

            using SqlCommand commandVillain = new SqlCommand(sqlQueryDeleteVillain, sqlConnection, transaction);
            commandVillain.Parameters.AddWithValue(@"villainId", villainId);

            int number = commandVillain.ExecuteNonQuery();

            if (number != 1)
            {
                transaction.Rollback();
            }

        }

        private static List<string> GetVillain(string villainId, SqlConnection sqlConnection)
        {
            string sqlQueryGetVillain = "SELECT * FROM Villains WHERE Id = @VillainId";

            using SqlCommand command = new SqlCommand(sqlQueryGetVillain, sqlConnection);
            command.Parameters.AddWithValue("@VillainId", villainId);

            var reader = command.ExecuteReader();

            List<string> villainParameters = new List<string>();

            while (reader.Read())
            {
                string name = reader["Name"] as string;
                villainParameters.Add(name);

                int? evilnessFactorId = reader["EvilnessFactorId"] as int?;
                villainParameters.Add(evilnessFactorId.ToString());
            }

            return villainParameters;
        }

        private static List<string> GetServingMinionsIds(string villainId, SqlConnection sqlConnection)
        {
            List<string> minionsIds = new List<string>();

            string sqlQueryGetMinionsIds = "SELECT MinionId FROM MinionsVillains WHERE VillainId = @villainId";

            using SqlCommand command = new SqlCommand(sqlQueryGetMinionsIds, sqlConnection);
            command.Parameters.AddWithValue("@villainId", villainId);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int? id = reader["MinionId"] as int?;
                minionsIds.Add(id.ToString());
            }

            return minionsIds;
        }
    }
}
