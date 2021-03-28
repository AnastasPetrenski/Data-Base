using Newtonsoft.Json;

using PetClinic.Data;
using PetClinic.DataProcessor.Dto.Export;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor
{
    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context
                .Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                })
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber)
                .ToList();

            var json = JsonConvert.SerializeObject(animals, Formatting.Indented);

            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {

            var test = context.Procedures.Select(p => new
            {
                Name = p.Animal.Name,
                Aids = p.ProcedureAnimalAids.Select(x => x.AnimalAid.Name).ToArray()
            }).ToArray();

            var procedures = context
                .Procedures
                .OrderBy(p => p.DateTime)
                .Select(p => new XmlProcedureDto()
                {
                    Passport = p.Animal.PassportSerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids.Select(pa => new XmlAnimalAidDto()
                    {
                        Name = pa.AnimalAid.Name,
                        Price = pa.AnimalAid.Price
                    })
                    .ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(pa => pa.AnimalAid.Price),
                })
                .OrderBy(p => p.Passport)
                .ToArray();

            var serializer = new XmlSerializer(typeof(XmlProcedureDto[]), new XmlRootAttribute("Procedures"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), procedures, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}