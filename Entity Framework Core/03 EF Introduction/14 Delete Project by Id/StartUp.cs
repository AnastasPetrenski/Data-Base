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

            var result = GetEmployeesByFirstNameStartingWithSa(context);

            Console.WriteLine(result);
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var sb = new StringBuilder();

            int id = 2;

            var projectToDelete = context.Projects.First(p => p.ProjectId == id);

            if (projectToDelete != null)
            {
                var employeesProjectsToDelete = context.EmployeesProjects
                    .Where(ep => ep.ProjectId == id);

                if (employeesProjectsToDelete.Count() > 0)
                {
                    context.EmployeesProjects.RemoveRange(employeesProjectsToDelete);
                }
                
                context.Projects.Remove(projectToDelete);
                context.SaveChanges();
            }

            context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList()
                .ForEach(p => sb.AppendLine($"{p}"));

            return sb.ToString().TrimEnd();
        }

    }
}
