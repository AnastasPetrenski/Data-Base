using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Models;
using CarDealer.Models.ExportsDtos;
using CarDealer.Models.ImportsDtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            mapper = InitializeMapper(mapper);

            //string inputSuppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string inputPartsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //string inputCarsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //string inputCustomersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //string inputSalesXml = File.ReadAllText("../../../Datasets/sales.xml");

            //var suppliers = ImportSuppliers(context, inputSuppliersXml);
            //var parts = ImportParts(context, inputPartsXml);
            //var cars = ImportCars(context, inputCarsXml);
            //var customers = ImportCustomers(context, inputCustomersXml);
            //var sales = ImportSales(context, inputSalesXml);

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new ExportCustomerCarSalesDto
                {
                    Car = new ExportCustomerCarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    Name = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWirhDiscount = s.Car.PartCars.Sum(x => x.Part.Price) - 
                                        (s.Car.PartCars.Sum(x => x.Part.Price) * s.Discount / 100m)
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCustomerCarSalesDto[]), new XmlRootAttribute("sales"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                                   .Where(c => c.Sales.Count > 0)
                                   .Select(x => new ExportCustomerDto
                                   {
                                       Name = x.Name,
                                       Count = x.Sales.Count,
                                       SpentMoney = x.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                                   })
                                   .OrderByDescending(x => x.SpentMoney)
                                   .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("customers"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
            serializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var mapped = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarPartsDto>(mapper.ConfigurationProvider)
                .ToArray();

            var cars = context.Cars
                              .OrderByDescending(x => x.TravelledDistance)
                              .ThenBy(x => x.Model)
                              .Take(5)
                              .Select(x => new ExportCarPartsDto()
                              {
                                  Make = x.Make,
                                  Model = x.Model,
                                  TravelledDistance = x.TravelledDistance,
                                  Parts = x.PartCars.Select(s => new ExportCarPartsArrayDto()
                                  {
                                      Name = s.Part.Name,
                                      Price = s.Part.Price
                                  })
                                  .OrderByDescending(s => s.Price)
                                  .ToArray()
                              })
                              .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCarPartsDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            serializer.Serialize(new StringWriter(sb), mapped, namespaces);

            using (TextWriter writer = new StreamWriter("../../../Datasets/Exports/cars-and-parts.xml"))
            {
                serializer.Serialize(writer, cars, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                                   .Where(s => !s.IsImporter)
                                   .ProjectTo<ExportSupplierDto>(mapper.ConfigurationProvider)
                                   .ToArray();

            var serializer = new XmlSerializer(typeof(ExportSupplierDto[]), new XmlRootAttribute("suppliers"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            serializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            using (TextWriter writer = new StreamWriter("../../../Datasets/Exports/local-suppliers.xml"))
            {
                serializer.Serialize(writer, suppliers, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                              .Where(c => c.Make == "BMW")
                              .OrderBy(c => c.Model)
                              .ThenByDescending(c => c.TravelledDistance)
                              .ProjectTo<ExportBmwDto>(mapper.ConfigurationProvider)
                              .ToArray();

            var serializer = new XmlSerializer(typeof(ExportBmwDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            EnsureDirectoryExist();

            using (TextWriter writer = new StreamWriter("../../../Datasets/Exports/bmw-cars.xml"))
            {
                serializer.Serialize(writer, cars, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var distance = 2000000;
            var cars = context.Cars
                              .Where(c => c.TravelledDistance > distance)
                              .OrderBy(c => c.Make)
                              .ThenBy(c => c.Model)
                              .Take(10)
                              .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider)
                              .ToArray();

            var serializer = new XmlSerializer(typeof(ExportCarDto[]), new XmlRootAttribute("cars"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            EnsureDirectoryExist();

            using (TextWriter writer = new StreamWriter("../../../Datasets/Exports/cars.xml"))
            {
                serializer.Serialize(writer, cars, namespaces);
            }

            return sb.ToString().TrimEnd();

        }

        private static void EnsureDirectoryExist()
        {
            if (!Directory.Exists("../../../Datasets/Exports"))
            {
                Directory.CreateDirectory("../../../Datasets/Exports");
            }
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            inputXml = GetXmlData();
            var serializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));
            var saleDtos = serializer.Deserialize(new StringReader(inputXml)) as ImportSaleDto[];
            var carIds = context.Cars.Select(c => c.Id).ToList();

            var filterCars = saleDtos.Where(x => carIds.Contains(x.CarId)).ToList();

            var sales = mapper.Map<List<Sale>>(filterCars);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            inputXml = GetXmlData();

            var serializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            var customersDtos = serializer.Deserialize(new StringReader(inputXml)) as ImportCustomerDto[];

            //Initialize auto mapper here or make it public for Judge
            var customers = mapper.Map<List<Customer>>(customersDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            inputXml = GetXmlData();

            var serializer = new XmlSerializer(typeof(List<ImportCarDto>), new XmlRootAttribute("Cars"));
            var carDtos = serializer.Deserialize(new StringReader(inputXml)) as List<ImportCarDto>;

            var partIds = context.Parts.Select(p => p.Id).ToList();
            var cars = new List<Car>();

            var linqCars = carDtos
                .Select(c => new Car()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TraveledDistance,
                    PartCars = c.PartIds
                                .Select(p => p.PartId)
                                .Distinct()
                                .Intersect(partIds)
                                .Select(pc => new PartCar()
                                {
                                    PartId = pc
                                })
                                .ToList()
                })
                .ToList();

            foreach (var dto in carDtos)
            {
                var car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TravelledDistance = dto.TraveledDistance,
                };

                //Select only unique Ids
                var parts = dto.PartIds.Select(x => x.PartId).Distinct();
                //Check Parts contains that id
                var ckeckedParts = parts.Intersect(partIds);

                foreach (var partId in parts)
                {
                    car.PartCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                //EF will add them for us automatically
                //context.PartCars.AddRange(car.PartCars);
                //context.SaveChanges();

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            inputXml = GetXmlData();

            var serializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));
            var partDtos = serializer.Deserialize(new StringReader(inputXml)) as ImportPartDto[];

            var supplierIds = context.Suppliers.Select(x => x.Id).ToList();

            var parts = partDtos
                                .Where(p => supplierIds.Contains(p.SupplierId))
                                .Select(p => new Part
                                {
                                    Name = p.Name,
                                    Price = p.Price,
                                    Quantity = p.Quantity,
                                    SupplierId = p.SupplierId
                                })
                                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            inputXml = GetXmlData();

            var serializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));
            var supplierDtos = serializer.Deserialize(new StringReader(inputXml)) as ImportSupplierDto[];

            var suppliers = new List<Supplier>();
            foreach (var dto in supplierDtos)
            {
                suppliers.Add(new Supplier()
                {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter
                });
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string GetXmlData()
        {
            StackTrace stackTrace = new StackTrace();
            var callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;

            var path = string.Empty;

            if (callingMethodName.StartsWith("Import"))
            {
                var fileName = callingMethodName.ToLower().Substring(6);
                var directory = Directory.GetCurrentDirectory();
                var directories = Directory.GetDirectories(directory);
                path = directories.Any(x => x.Contains("Datasets")) ?
                                            $"Datasets/{fileName}.xml" :
                                            $"../../../Datasets/{fileName}.xml";
            }

            //var types = Assembly.GetCallingAssembly().GetTypes();
            //foreach (var type in types)
            //{
            //    var methods = type.GetMethods();
            //    foreach (var method in methods)
            //    {
            //       Console.WriteLine(method.Name); 
            //    }
            //}            

            return File.ReadAllText(path);
        }

        public static IMapper InitializeMapper(IMapper mapper)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();

            return mapper;

        }
    }
}