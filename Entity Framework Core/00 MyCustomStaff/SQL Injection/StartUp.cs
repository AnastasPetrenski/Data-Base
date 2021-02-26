using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Text;

namespace SQL_Injection
{
    class StartUp
    {
        static void Main(string[] args)
        {
            Console.WriteLine("User:");
            string userName = Console.ReadLine();
            Console.WriteLine("Password");
            string passWord = Console.ReadLine();

            //1.Data authentication
            string connection = "Server=.;Database=Education;Integrated Security=true";
            //2.Connection initialization with USING
            using SqlConnection sqlConnection = new SqlConnection(connection);
            //3.Open connection
            sqlConnection.Open();
            //4.Create command <-- Vulnerable for SQL Injection
            string query = $"SELECT * FROM Students WHERE [USER] = '{userName}' AND [Password] = '{passWord}'";
            //Use Parametars for prevention
            string queryParameters = "SELECT * FROM Students WHERE [USER] = @User AND [Password] = @Password";
            SqlCommand command = new SqlCommand(queryParameters, sqlConnection);
            //command.Parameters.Add("@User", System.Data.SqlDbType.VarChar);
            //command.Parameters["@User"].Value = userName;
            //command.Parameters.Add("@Password", System.Data.SqlDbType.VarChar);
            //command.Parameters["@Password"].Value = passWord;
            command.Parameters.AddWithValue("@User", userName);
            command.Parameters.AddWithValue("@Password", passWord);
            //5.Execute
            using SqlDataReader reader = command.ExecuteReader();
            
            using FileStream stream = new FileStream("../../../test.txt", FileMode.OpenOrCreate);

            var buffer = new byte[4096];

            while (reader.Read())
            {
                int id = (int)reader["Id"];
                string user = (string)reader["User"];
                string fullName = (string)reader["Full Name"];
                int studentFk = (int)reader["StudentFK"];
                string pass = (string)reader["Password"];

                var fullData = $"{id} {user} {fullName} {studentFk} {pass}{Environment.NewLine}";
                buffer = Encoding.UTF8.GetBytes(fullData);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
