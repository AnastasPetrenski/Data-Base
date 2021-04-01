namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context
                .Projects
                .Where(p => p.Tasks.Count > 0)
                .Select(p => new XmlProjectDto()
                {
                    ProjectName = p.Name,
                    TasksCount = p.Tasks.Count,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new XmlTaskDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(t => t.Name)
                    .ToList()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();

            var serializer = new XmlSerializer(typeof(XmlProjectDto[]), new XmlRootAttribute("Projects"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var sb = new StringBuilder();

            serializer.Serialize(new Utf8StringWriter(sb, Encoding.UTF8), projects, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context
                .Employees
                .Where(e => e.EmployeesTasks
                             .Any(t => t.Task.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                    .Where(t => t.Task.OpenDate >= date)
                    .OrderByDescending(t => t.Task.DueDate)
                    .ThenBy(t => t.Task.Name)
                    .Select(t => new
                    {
                        TaskName = t.Task.Name,
                        OpenDate = t.Task.OpenDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        DueDate = t.Task.DueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        LabelType = t.Task.LabelType.ToString(),
                        ExecutionType = t.Task.ExecutionType.ToString()
                    })
                    .ToList()
                })
                .OrderByDescending(e => e.Tasks.Count)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }

        public sealed class Utf8StringWriter : StringWriter 
        {
            private readonly Encoding encoding;

            public Utf8StringWriter(StringBuilder sb, Encoding encoding)
                : base(sb)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding => this.encoding;

        }
    }
}