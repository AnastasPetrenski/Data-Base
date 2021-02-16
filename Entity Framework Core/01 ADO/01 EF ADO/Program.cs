using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _01_EF_ADO
{
    class Program
    {
        static void Main()
        {
            //Nullable test
            List<string> list = new List<string>();

            string name = "Nas";
            string id = null;

            list.Add(name);
            list.Add(id);

            List<int?> ints = new List<int?>();

            int one = 1;
            int? two = null;

            ints.Add(one);
            ints.Add(two);

            //1."Data source=" or "Server=name of the server"
            //"ANASTAS-PC" or "." or "localhost" or "127.0.0.1" <== my comp address for DB

            //2.Database name:  sa
            //Database=name of database to which we are connecting

            //3.Authentication type
            //"Integrated Security=true"  <== for localhost
            //"User Id=userName;Password=123456" <== if we have Credentials

            string connection = "Server=.;Database=SoftUni;Integrated Security=true";
            //Connection initialization
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {
                //We open TCP connection
                sqlConnection.Open();
                //Begin transaction
               
                //SQL command query
                string queryForExecuteReaderReturnTable = "SELECT FirstName, LastName, Salary FROM [Employees]";
                SqlCommand sqlReaderCommand = new SqlCommand(queryForExecuteReaderReturnTable, sqlConnection);
                using (SqlDataReader reader = sqlReaderCommand.ExecuteReader())
                {
                    using FileStream writer = new FileStream("../../../test.txt", FileMode.OpenOrCreate);

                    var buffer = new byte[4096];

                    while (reader.Read())
                    {
                        string firstName = (string)reader["FirstName"];
                        string lastName = (string)reader["LastName"];
                        decimal? salary = reader["Salary"] as decimal?;
                        Console.WriteLine($"{firstName} {lastName} {salary:f2}");

                        var fullName = $"{firstName} {lastName} {salary:f2}{Environment.NewLine}";
                        buffer = ConvertToByteArray(fullName, Encoding.UTF8);
                        writer.Write(buffer, 0, buffer.Length);
                    }
                }


                using var transaction = sqlConnection.BeginTransaction();
                transaction.Commit();
                
                if (transaction.Connection is null)
                {
                    
                }
                else
                {
                    transaction.Save("asd");
                    transaction.Rollback();
                }
                //sqlConnection.Open();

                string queryForScalarReturnOnlyOneResult = "SELECT COUNT(*) FROM [Employees]";
                var sqlScalarCommand = new SqlCommand(queryForScalarReturnOnlyOneResult, sqlConnection);
                var singleResult = sqlScalarCommand.ExecuteScalar();
                Console.WriteLine(singleResult);

                string queryForNonQuery = "UPDATE [Employees] SET Salary = Salary * 1.10 WHERE Salary >= 20000";
                var sqlNonQueryCommand = new SqlCommand(queryForNonQuery, sqlConnection);
                var effectedColumn = sqlNonQueryCommand.ExecuteNonQuery(); 
                Console.WriteLine(effectedColumn);
                Console.WriteLine(effectedColumn);

                
            }

        }

        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }
    }
}
