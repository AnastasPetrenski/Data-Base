using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;

namespace EFPerformanceProblems
{
    class Program
    {
        static void Main()
        {
            var db = new StudentsDbContext();
            //db.Database.Migrate();

            //for (int i = 0; i < 10000; i++)
            //{
            //    var student = new Student();
            //    student.Name = "Pesho" + i.ToString();
            //    student.City = new City { Name = "Sofia" + i };
            //    student.Picture = new byte[10000];
            //    db.Students.Add(student);
            //    Console.WriteLine(i);
            //}

            foreach (var item in db.Students)
            {
                Console.WriteLine($"{item.Name} {item.City.Name}");
            }

            db.SaveChanges();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var students = db.Students.ToList();
            foreach (var item in students)
            {
                Console.WriteLine(item.Name);
            }
            var time = sw.ElapsedMilliseconds;
            
            sw.Reset();

            var studentsQuerableProection = db.Students.Select(student =>
            new
            {
                student.Name
            });

            foreach (var item in studentsQuerableProection)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine(time + "<=============== toList");
            Console.WriteLine(sw.ElapsedMilliseconds + "<=============== IQuerable");

            db.Database.ExecuteSqlRaw("DELETE FROM [Students]");
        }
    }
}
