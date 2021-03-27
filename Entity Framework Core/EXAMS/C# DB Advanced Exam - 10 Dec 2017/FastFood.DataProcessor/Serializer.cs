using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Export;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var startFromEmployee = context
                .Employees
                .Where(e => e.Name == employeeName)
                .Select(e => new
                {
                    Name = e.Name,
                    Orders = e.Orders
                              .Where(o => o.Type.ToString() == orderType)
                              .Select(o => new
                              {
                                  Customer = o.Customer,
                                  Items = o.OrderItems
                                        .Select(i => new
                                        {
                                            Name = i.Item.Name,
                                            Price = i.Item.Price,
                                            Quantity = i.Quantity
                                        })
                                        .ToList(),
                                  TotalPrice = o.OrderItems.Sum(y => y.Item.Price * y.Quantity)
                              })
                              .ToList()
                })
                .ToList();


            var startFromOrder = context
                .Orders
                .Where(o => o.Employee.Name == employeeName && o.Type.ToString() == orderType)
                .Select(o => new
                {
                    Name = o.Employee.Name,
                    Orders = o.OrderItems
                    .Select(x => new
                    {
                        Customer = x.Order.Customer,
                        Items = o.OrderItems
                        .Select(i => new
                        {
                            Name = i.Item.Name,
                            Price = i.Item.Price,
                            Quantity = i.Quantity
                        })
                        .ToList(),
                        TotalPrice = o.OrderItems.Sum(y => y.Item.Price * y.Quantity),
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(startFromEmployee, Formatting.Indented);

            return json;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categories = categoriesString.Split(",", StringSplitOptions.RemoveEmptyEntries);

            var dtos = context.Categories
                .Where(x => categories.Contains(x.Name))
                .Select(x => new XmlExportCategoryDto()
                {
                    Name = x.Name,
                    MostPopularItem = x.Items.Select(i => new XmlExportMostPopularDto()
                    {
                        Name = i.Name,
                        //price = i.Price,
                        //countOrders = i.OrderItems.Count(),
                        TotalMade = i.OrderItems.Sum(f => f.Quantity * f.Item.Price),
                        TimesSold = i.OrderItems.Sum(f => f.Quantity),
                    })
                    .OrderByDescending(m => m.TotalMade)
                    .First()
                })
                .OrderByDescending(x => x.MostPopularItem.TotalMade)
                .ThenByDescending(x => x.MostPopularItem.TimesSold)
                .ToArray();

            var serializer = new XmlSerializer(typeof(XmlExportCategoryDto[]), new XmlRootAttribute("Categories"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), dtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}