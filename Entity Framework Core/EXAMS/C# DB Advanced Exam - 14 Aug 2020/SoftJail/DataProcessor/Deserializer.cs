namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string SuccessDepartmentMessage = "Imported {0} with {1} cells";
        private const string SuccessPrisonerMessage = "Imported {0} {1} years old";
        private const string SuccessOfficerMessage = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonDepartmentDto[]>(jsonString);

            var departments = new List<Department>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var department = new Department()
                {
                    Name = dto.Name
                };

                var isCellsValid = true;
                foreach (var cell in dto.Cells)
                {
                    if (!IsValid(cell))
                    {
                        isCellsValid = false;
                        break;
                    }

                    department.Cells.Add(new Cell()
                    {
                        CellNumber = cell.CellNumber,
                        HasWindow = cell.HasWindow,
                        Department = department
                    });
                }

                if (department.Cells.Count == 0 || !isCellsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                departments.Add(department);
                sb.AppendLine(string.Format(SuccessDepartmentMessage, department.Name, department.Cells.Count));
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonPrisonerDto[]>(jsonString);

            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidIncarcerationDate = DateTime.TryParseExact(dto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incarcerationDate);

                DateTime? releaseDate = null;

                if (dto.ReleaseDate != null)
                {
                    releaseDate = DateTime.ParseExact(dto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                var prisoner = new Prisoner()
                {
                    FullName = dto.FullName,
                    Nickname = dto.Nickname,
                    Age = dto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = dto.Bail,
                    CellId = dto.CellId
                };

                bool isValidMail = true;

                foreach (var mailDto in dto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        isValidMail = false;
                        break;
                    }

                    prisoner.Mails.Add(new Mail()
                    {
                        Address = mailDto.Address,
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Prisoner = prisoner
                    });
                }

                if (!isValidMail)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                prisoners.Add(prisoner);
                sb.AppendLine(string.Format(SuccessPrisonerMessage, prisoner.FullName, prisoner.Age));
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlOfficerDto[]), new XmlRootAttribute("Officers"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlOfficerDto[];

            var sb = new StringBuilder();
            var officers = new List<Officer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidPosiotion = Enum.TryParse(dto.Position, out Position position);
                var isValidWeapon = Enum.TryParse(dto.Weapon, out Weapon weapon);

                if (!isValidPosiotion || !isValidWeapon)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var officer = new Officer()
                {
                    FullName = dto.FullName,
                    Salary = dto.Salary,
                    Position = position,
                    Weapon = weapon,
                    DepartmentId = dto.DepartmentId
                };

                foreach (var prisonerId in dto.Prisoners)
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner()
                    {
                        PrisonerId = prisonerId.Id,
                        Officer = officer
                    });
                }

                officers.Add(officer);
                sb.AppendLine(string.Format(SuccessOfficerMessage, officer.FullName, officer.OfficerPrisoners.Count));
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}