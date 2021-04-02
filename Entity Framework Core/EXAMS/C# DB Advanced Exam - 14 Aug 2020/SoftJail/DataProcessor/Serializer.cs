namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisonersDtos = context
                .Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(o => new
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name
                    })
                    .OrderBy(o => o.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = decimal.Parse(x.PrisonerOfficers.Sum(o => o.Officer.Salary).ToString("f2"))
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(prisonersDtos, Formatting.Indented);

            return json;

        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var prisoners = context
                .Prisoners
                .Where(p => prisonersNames.Contains(p.FullName))
                .Select(p => new XmlPrisonerDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Messages = p.Mails.Select(m => new XmlMessageDto()
                    {
                        Description = string.Join("", m.Description.Reverse())
                    })
                    .ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var serializer = new XmlSerializer(typeof(XmlPrisonerDto[]), new XmlRootAttribute("Prisoners"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), prisoners, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}