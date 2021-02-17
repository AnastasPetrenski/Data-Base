using EF_03_Intro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EF_03_Intro.Services
{
    public class SoftUniServices
    {
        private static StringBuilder sb = new StringBuilder();
        private readonly SoftUniContext context;

        public SoftUniServices(SoftUniContext context)
        {
            this.context = context;
        }

        public string GetEmployee24Projects()
        {
            var list = context
                .EmployeesProjects
                .Where(e => e.EmployeeId == 24)
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.Employee.FirstName,
                    ProjectName = e.Project.Name,
                    ProjectDate = e.Project.StartDate
                })
                .ToList();
            DateTime date = new DateTime(2005, 01, 01);

            foreach (var item in list)
            {

                var r = item.ProjectDate > date ? "NULL" : item.ProjectName;
                sb.AppendLine($"{item.EmployeeId} {item.FirstName} {r}");
            }

            return sb.ToString().TrimEnd();
        }

        public string GetEmployeesWithProjectAfter()
        {
            DateTime date = new DateTime(2002, 08, 13);

            var list = context
                .Projects                
                .Select(p => new
                {
                    Start = (p.StartDate > date && p.EndDate == null),
                    ProjectName = p.Name,
                    EmployeeId = p.EmployeesProjects
                        .Select(x => x.EmployeeId).Single(),
                    FirstName = p.EmployeesProjects
                        .Select(x => x.Employee.FirstName).Single()
                })
                .OrderBy(e => e.EmployeeId)
                .Where(e => e.Start == true)
                .Take(5)
                .ToList();

            foreach (var i in list)
            {

                sb.AppendLine($"{i.EmployeeId} {i.FirstName} {i.ProjectName}");

            }

            return sb.ToString().TrimEnd();
        }

        public string GetEmployeesHiredAfter()
        {
            DateTime date = new DateTime(1999, 01, 01);
            //DateTime.ParseExact(e.HireDate.ToString(), "yyyy-MM-dd h:mm:ss", CultureInfo.InvariantCulture)
            var list = context
                .Employees
                .Where(e => e.HireDate > date)
                .Where(e => e.Department.Name == "Sales" || e.Department.Name == "Finance")
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    HireDate = e.HireDate,
                    DeptName = e.Department.Name
                })
                .OrderBy(e => e.HireDate)
                .ToList();

            foreach (var i in list)
            {
                sb.AppendLine($"{i.FirstName} - {i.LastName} - {i.HireDate} - {i.DeptName}");
            }

            return sb.ToString().TrimEnd();
        }

        public string GetEmployeesWithoutProject()
        {
            var list = context
                .Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    EmployeeIdExist = e.EmployeesProjects.Any(ep => ep.EmployeeId == e.EmployeeId),
                    e.FirstName
                })
                .OrderBy(e => e.EmployeeId)
                .Where(e => e.EmployeeIdExist == false)
                .Take(3)
                .ToList();

            foreach (var e in list)
            {
                sb.AppendLine($"{e.EmployeeId} - {e.FirstName}");
            }

            return sb.ToString().TrimEnd();
        }

        public string GetEmployyeDepartments()
        {
            var employees = context
                .Employees
                .Where(e => e.Salary > 15000)
                .OrderBy(e => e.DepartmentId)
                .Take(5)
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FirstName + ' ' + e.LastName,
                    Salary = e.Salary,
                    Department = e.Department.Name
                })
                .ToList();
            sb.AppendLine($"EmployeeID - FullName - Salary - Department");
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.EmployeeId} - {item.FullName} - {item.Salary} - {item.Department}");
            }

            return sb.ToString().TrimEnd();
        }

        public string GetEmployeesInfo()
        {
            var test = context.Employees
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address = x.Address.AddressText
                })
                .ToList();

            foreach (var item in test)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - {item.Address}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
