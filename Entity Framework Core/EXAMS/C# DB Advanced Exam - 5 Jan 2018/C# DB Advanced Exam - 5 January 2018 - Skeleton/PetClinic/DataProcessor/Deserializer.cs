using Newtonsoft.Json;

using PetClinic.Data;
using PetClinic.DataProcessor.Dto.Import;
using PetClinic.Models;

using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Globalization;

using System.Xml.Serialization;
using System.IO;

namespace PetClinic.DataProcessor
{
    public class Deserializer
    {
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string SuccessMessageAnimal = "Record {0} Passport №: {1} successfully imported.";
        private const string SuccessMessageProcedure = "Record successfully imported.";
        private const string ErrorMessage = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var data = JsonConvert.DeserializeObject<JsonAnimalAidDto[]>(jsonString);

            var sb = new StringBuilder();
            var animalAids = new List<AnimalAid>();

            foreach (var item in data)
            {
                if (!IsValid(item))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (animalAids.Any(a => a.Name == item.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                animalAids.Add(new AnimalAid()
                {
                    Name = item.Name,
                    Price = item.Price
                });

                sb.AppendLine(string.Format(SuccessMessage, item.Name));
            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var data = JsonConvert.DeserializeObject<JsonAnimalDto[]>(jsonString);

            var sb = new StringBuilder();
            var animals = new List<Animal>();

            foreach (var dto in data)
            {
                if (!IsValid(dto) || !IsValid(dto.Passport))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (animals.Any(a => a.PassportSerialNumber == dto.Passport.SerialNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animal = new Animal()
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    Age = dto.Age,

                };

                var passport = new Passport()
                {
                    SerialNumber = dto.Passport.SerialNumber,
                    Animal = animal,
                    OwnerPhoneNumber = dto.Passport.OwnerPhoneNumber,
                    OwnerName = dto.Passport.OwnerName,
                    RegistrationDate = DateTime.ParseExact(dto.Passport.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                };

                context.Passports.Add(passport);
                context.SaveChanges();

                animal.Passport = passport;

                animals.Add(animal);
                sb.AppendLine(string.Format(SuccessMessageAnimal, animal.Name, dto.Passport.SerialNumber));
            }

            //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [PetClinic].[dbo].[Animals] ON");

            //context.Animals.AddRange(animals);
            //context.SaveChanges();

            //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [PetClinic].[dbo].[Animals] OFF");

            return sb.ToString().TrimEnd();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlVetDto[]), new XmlRootAttribute("Vets"));
            var data = serializer.Deserialize(new StringReader(xmlString)) as XmlVetDto[];

            var vets = new List<Vet>();
            var sb = new StringBuilder();

            foreach (var dto in data)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (vets.Any(v => v.PhoneNumber == dto.PhoneNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                vets.Add(new Vet()
                {
                    Name = dto.Name,
                    Profession = dto.Profession,
                    Age = dto.Age,
                    PhoneNumber = dto.PhoneNumber
                });

                sb.AppendLine(string.Format(SuccessMessage, dto.Name));
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlProcedureDto[]), new XmlRootAttribute("Procedures"));
            var data = serializer.Deserialize(new StringReader(xmlString)) as XmlProcedureDto[];

            var sb = new StringBuilder();
            var procedures = new List<Procedure>();

            foreach (var dto in data)
            {
                var vet = context.Vets.FirstOrDefault(v => v.Name == dto.Vet);

                if (vet is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animal = context.Animals.FirstOrDefault(a => a.PassportSerialNumber == dto.Animal);

                if (vet is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime date;
                var isValidDate = DateTime.TryParseExact(dto.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                if (!isValidDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var procedure = new Procedure()
                {
                    Vet = vet,
                    Animal = animal,
                    DateTime = date,

                };

                var isValidProcedure = true;
                
                foreach (var aid in dto.AnimalAids)
                {
                    var animalAid = context.AnimalAids.FirstOrDefault(a => a.Name == aid.Name);

                    if (animalAid is null)
                    {
                        sb.AppendLine(ErrorMessage);
                        isValidProcedure = false;
                        break;
                    }

                    if (procedure.ProcedureAnimalAids.Any(a => a.AnimalAid.Name == animalAid.Name))
                    {
                        sb.AppendLine(ErrorMessage);
                        isValidProcedure = false;
                        break;
                    }

                    procedure.ProcedureAnimalAids.Add(new ProcedureAnimalAid()
                    {
                        AnimalAid = animalAid,
                        Procedure = procedure
                    });
                }

                if (!isValidProcedure)
                {
                    continue;
                }

                procedures.Add(procedure);
                sb.AppendLine(SuccessMessageProcedure);
            }

            context.Procedures.AddRange(procedures);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}