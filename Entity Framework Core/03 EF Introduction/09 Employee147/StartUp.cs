using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main()
        {
            SoftUniContext context = new SoftUniContext();

            var result = GetEmployeesInPeriod(context);

            Console.WriteLine(result);
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects
                                //.Select(p => p.Project.Name)
                                .Select(p => new
                                {
                                    ProjectName = p.Project.Name
                                })
                                //.OrderBy(s => s) <== IEnumerable<string> order by itself
                                .OrderBy(s => s.ProjectName) //<== IOrderedEnumerable
                                .ToList() // <== List<string>
                })
                .Single();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            foreach (var p in employee.Projects)
            {
                sb.AppendLine($"{p.ProjectName}");
            }

            return sb.ToString().TrimEnd();
        }

        
    }
}
