using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarDealer
{
    public class StartUp
    {
        private static string Results = "../../../Datasets/Results";
        private static IMapper _mapper;

        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            _mapper = new Mapper(config);


            EnsureDataBaseExist(context);
            EnsureDirectoryExists();

            var inputSuppliersJson = ReadSuppliersFile();
            var resultImportSuppliers = ImportSuppliers(context, inputSuppliersJson);

            var inputPartsJson = ReadPartsFile();
            var resultImportParts = ImportParts(context, inputPartsJson);

            var inputCarsJson = ReadCarsFile();
            var resultImportCars = ImportCars(context, inputCarsJson);

            var inputCustomersJson = ReadCustomersFile();
            var resultImportCusmoers = ImportCustomers(context, inputCustomersJson);

            var inputSalesJson = ReadSalesFile();
            var resultImportSales = ImportSales(context, inputSalesJson);
            Console.WriteLine(TestJson(context));

            var result = GetLocalSuppliers(context);

            File.WriteAllText(Results + "/ordered-customers.json", result);
            File.WriteAllText(Results + "/toyota-cars.json", result);
            File.WriteAllText(Results + "/local-suppliers.json", result);
            File.WriteAllText(Results + "/cars-and-parts.json", result);
            File.WriteAllText(Results + "/sales-discounts.json", result);
        }

        private static string TestJson(CarDealerContext context)
        {
            var text = File.ReadAllText("../../../Datasets/test.json");

            var result = JsonConvert.DeserializeObject<JToken>(text);

            return "";
        }

        /************************ Export Tasks ***********************/

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                                   .Where(s => !s.IsImporter)
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       PartsCount = s.Parts.Count
                                   })
                                   .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                               .Take(10)
                               .Select(x => new
                               {
                                   car = new
                                   {
                                       Make = x.Car.Make,
                                       Model = x.Car.Model,
                                       TravelledDistance = x.Car.TravelledDistance
                                   },
                                   customerName = x.Customer.Name,
                                   Discount = x.Discount,
                                   price = x.Car.PartCars.Sum(s => s.Part.Price).ToString("F2"),
                                   priceWithDiscount = (x.Car.PartCars.Sum(s => s.Part.Price) *
                                                       (1 - x.Discount/100)).ToString("F2"),
                               })
                               .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                   .Customers
                   .ProjectTo<GetCustomerWithCarDto>(_mapper.ConfigurationProvider)
                   .Where(c => c.BoughtCars >= 1)
                   .OrderByDescending(c => c.SpentMoney)
                   .ThenByDescending(c => c.BoughtCars)
                   .ToList();

            var customersWithCars = context.Customers
                                            .Where(c => c.Sales.Count > 0)
                                            .Select(c => new
                                            {
                                                FullName = c.Name,
                                                BoughtCars = c.Sales.Count,
                                                SpentMoney = c.Sales.Sum(x => x.Car.PartCars.Sum(z => z.Part.Price))
                                            })
                                            .OrderByDescending(c => c.SpentMoney)
                                            .ThenByDescending(c => c.BoughtCars)
                                            .ToList();

            var json = JsonConvert.SerializeObject(customersWithCars, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithoutParts = context.Cars
                .Select(x => new
                {
                    car = x.PartCars
                           .Select(s => new
                           {
                               Make = x.Make,
                               Model = x.Model,
                               TravelledDistance = x.TravelledDistance
                           }).FirstOrDefault(),
                    parts = x.PartCars
                             .Select(s => new
                             {
                                 Name = s.Part.Name,
                                 Price = s.Part.Price.ToString("F2")
                             }).ToList()
                })
                .ToList();

            var result = JsonConvert.SerializeObject(carsWithoutParts, Formatting.Indented);

            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var derivedCars = context.Cars
                              .Where(c => c.Make == "Toyota")
                              .OrderBy(c => c.Model)
                              .ThenByDescending(c => c.TravelledDistance)
                              .Select(c => new
                              {
                                  Id = c.Id,
                                  Make = c.Make,
                                  Model = c.Model,
                                  TravelledDistance = c.TravelledDistance
                              })
                              .ToList();

            var cars = JsonConvert.SerializeObject(derivedCars, Formatting.Indented);

            return cars;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customersDto = context.Customers
                                 .OrderBy(c => c.BirthDate)
                                 .ThenBy(c => c.IsYoungDriver)
                                 .Select(c => new GetCustomersDto
                                 {
                                     Name = c.Name,
                                     BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                     IsYoungDriver = c.IsYoungDriver
                                 })
                                 .ToList();

            var drivers = JsonConvert.SerializeObject(customersDto, Formatting.Indented);

            return drivers;
        }

        /************************ Import Tasks ***********************/

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        private static string ReadSalesFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);
            var path = directories.Any(x => x.Contains("Datasets")) ?
                                        @"Datasets/sales.json" :
                                        @"../../../Datasets/sales.json";

            var data = File.ReadAllText(path);

            return data;
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        private static string ReadCustomersFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);
            var path = directories.Any(x => x.Contains("Datasets")) ?
                                   @"Datasets/customers.json" :
                                   @"../../../Datasets/customers.json";

            var data = File.ReadAllText(path);

            return data;
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDTO = JsonConvert.DeserializeObject<List<ImportCarsDTO>>(inputJson);

            //var cars = _mapper.Map<List<Car>>(carsDTO);
            var cars = new List<Car>();

            foreach (var carDto in carsDTO)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                context.Cars.Add(car);

                foreach (var partId in carDto.PartsId.Distinct())
                {
                    var partCar = new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            var partsCount = carsDTO.Sum(x => x.PartsId.Length);
            var distParts = cars.Sum(x => x.PartCars.Count);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        private static string ReadCarsFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);
            var path = directories.Any(x => x.Contains("Datasets")) ?
                                    "/Datasets/cars.json" :
                                    "../../../Datasets/cars.json";

            var data = File.ReadAllText(path);

            return data;
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliersId = context.Suppliers.Select(x => x.Id).ToList();
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                                    .Where(x => suppliersId.Contains(x.SupplierId))
                                    .ToList();

            //var result = parts.Where(x => suppliersId.Contains(x.SupplierId)).ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        private static string ReadPartsFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);
            var path = directories.Any(x => x.Contains("/Datasets")) ?
                                  "/Datasets/parts.json" :
                                  "../../../Datasets/parts.json";

            var data = File.ReadAllText(path);

            return data;
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        private static string ReadSuppliersFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            var relativePath = directories.Any(x => x.Contains("Datasets")) ?
                                               @"Datasets/suppliers.json" :
                                               @"../../../Datasets/suppliers.json";

            var data = File.ReadAllText(relativePath);

            return data;
        }

        private static void EnsureDataBaseExist(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was Successfully deleted");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was Successfully created");
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(Results))
            {
                Directory.CreateDirectory(Results);
            }
        }
    }
}