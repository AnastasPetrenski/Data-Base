using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            var employees = new List<Employee>();
            var positions = new List<Position>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var position = positions.FirstOrDefault(p => p.Name == dto.Position);

                if (position is null)
                {
                    position = new Position()
                    {
                        Name = dto.Position
                    };

                    positions.Add(position);
                }

                employees.Add(new Employee()
                {
                    Name = dto.Name,
                    Age = dto.Age,
                    Position = position
                });

                sb.AppendLine(string.Format(SuccessMessage, dto.Name));
            }

            //EF will add Positions for me
            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<ImportItemDto[]>(jsonString);
            var sb = new StringBuilder();
            var items = new List<Item>();
            var categories = new List<Category>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var category = categories.FirstOrDefault(c => c.Name == dto.Category);

                if (category is null)
                {
                    category = new Category()
                    {
                        Name = dto.Category
                    };

                    categories.Add(category);
                }

                var item = new Item()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Category = category
                };

                if (items.Any(i => i.Name == item.Name))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                items.Add(item);
                sb.AppendLine(string.Format(SuccessMessage, dto.Name));
            }

            context.Items.AddRange(items);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlOrderDto[]), new XmlRootAttribute("Orders"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlOrderDto[];

            var orders = new List<Order>();
            
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var employee = context.Employees.FirstOrDefault(e => e.Name == dto.EmployeeName);

                if (employee is null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var isValidDate = DateTime.TryParseExact(dto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                if (!isValidDate)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var isValidType = Enum.TryParse(dto.Type, out OrderType type);

                if (!isValidType)
                {
                    type = OrderType.ToGo;
                }

                var order = new Order()
                {
                    Customer = dto.Customer,
                    DateTime = date,
                    Type = type,
                    Employee = employee,
                };

                var isValidItems = true;

                foreach (var itemDto in dto.Items)
                {
                    if (!IsValid(itemDto))
                    {
                        sb.AppendLine(FailureMessage);
                        isValidItems = false;
                        break;
                    }

                    var item = context.Items.FirstOrDefault(i => i.Name == itemDto.ItemName);

                    if (item is null)
                    {
                        sb.AppendLine(FailureMessage);
                        isValidItems = false;
                        break;
                    }

                    order.OrderItems.Add( new OrderItem()
                    {
                        Item = item,
                        Order = order,
                        Quantity = itemDto.Quantity
                    });

                }

                if (!isValidItems)
                {
                    continue;
                }

                orders.Add(order);
                sb.AppendLine($"Order for {order.Customer} on {dto.DateTime} added");
            }

            context.Orders.AddRange(orders);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}