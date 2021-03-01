using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.ModelsDTO;
using ProductShop.ModelsDTO.GetSoldProducts;
using ProductShop.ModelsDTO.GetUsersAndProducts;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper _mapper;
        private static string MinifingJSON = "../../../Datasets/MinifingJSON";
        private static string Results = "../../../Datasets/Results";

        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            //ResetDataBase(context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            IMapper mapper = new Mapper(config);
            _mapper = config.CreateMapper();

            string inputJSON = ReadFile();

            //var result = ImportUsers(context, inputJSON);
            //var result = ImportProducts(context, inputJSON);
            //var result = ImportCategories(context, inputJSON);
            //var result = ImportCategoryProducts(context, inputJSON);
            //var resultDTO = ImportUserInfos(context, inputJSON, mapper);
            //Console.WriteLine(resultDTO); 

            //var result = GetProductsInRange(context, mapper);
            var result = GetUsersWithProducts(context, mapper);

            EnsureDirectoryExist(Results);

            //File.WriteAllText(Results + "/products-in-range.json", result);
            //File.WriteAllText(Results + "/categories-by-products.json", result);
            File.WriteAllText(Results + "/users-and-products.json", result);

        }

        private static void EnsureDirectoryExist(string results)
        {
            if (!Directory.Exists(Results))
            {
                Directory.CreateDirectory(Results);
            }
        }

        private static string ImportUserInfos(ProductShopContext context, string inputJSON, IMapper mapper)
        {
            //for single object
            //JObject source = JObject.Parse(inputJSON);

            //JSONPath - "Items" if have name or $.[*]
            //var tokenCount = source.SelectTokens("$[*]").Children().Count();

            //already have JSON objects array
            JArray jObject = new JArray(inputJSON); //<== does not work as expected
            var tokenCountItems = jObject.Children().Count(); //<== return 1 ??? must be 56

            var list = JsonConvert.DeserializeObject<IEnumerable<JToken>>(inputJSON);

            foreach (var token in list)
            {
                var result = mapper.Map<UserInfo>(token);

                if (result.Age != 0)
                {
                    ChangeUserAgeCategory(result);
                }

                context.UserInfos.Add(result);
                context.SaveChanges();

            }

            return $"{context.UserInfos.Count()} users was added successfully!";
        }

        private static void ChangeUserAgeCategory(UserInfo result)
        {
            if (result.Age < 14)
            {
                result.AgeCategory = Models.Enumerators.AgeCategory.Child;
            }
            else if (result.Age < 18)
            {
                result.AgeCategory = Models.Enumerators.AgeCategory.Teenager;
            }
            else if (result.Age < 60)
            {
                result.AgeCategory = Models.Enumerators.AgeCategory.TargetGroup;
            }
            else
            {
                result.AgeCategory = Models.Enumerators.AgeCategory.Adult;
            }

        }

        private static string ReadFile()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            var relativePath = directories
                                    .Any(x => x.Contains("Datasets"))
                                    ? @"Datasets/users.json"
                                    : @"../../../Datasets/categories-products.json";

            //@"../../../Datasets/users.json"
            //@"../../../Datasets/products.json"
            //@"../../../Datasets/categories.json"
            //@"../../../Datasets/categories-products.json"

            var fileData = File.ReadAllText(relativePath);

            return fileData;
        }

        //Query 1. Import Data

        private static void ResetDataBase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");
            context.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }

        //Query 2. Import Data
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert
                            .DeserializeObject<User[]>(inputJson, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            });

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //Query 3. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                                        .Where(x => x.Name != null)
                                        .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //Query 4. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            //JsonSettings can be used as a parameter for Deserializing method
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                                .Where(x => x.Name != null)
                                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //Query 5. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //1. Query and Export Data
        //Export Products in Range
        public static string GetProductsInRange(ProductShopContext context, IMapper mapper)
        {
            //using Anonymous object
            var products = context.Products
                            //.AsEnumerable() <= Lazy loading Exception Error 
                            .Where(p => p.Price >= 500 && p.Price <= 1000)
                            .OrderBy(p => p.Price)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price,//.ToString("f:2"),
                                seller = p.Seller.FirstName + " " + p.Seller.LastName
                            })
                            .ToArray();
            //Add new lines
            string json = JsonConvert.SerializeObject(products, Formatting.Indented);

            //JSON Minifing all information on one row
            string myJSON = JsonConvert.SerializeObject(products, Formatting.None); // <= JSON Minifing

            Regex regex = new Regex(@"(""(?:[^""\\]|\\.)*"")|\s+");
            var instantResult = regex.Replace(myJSON, string.Empty);
            var staticResult = Regex.Replace(myJSON, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

            if (!Directory.Exists(MinifingJSON))
            {
                Directory.CreateDirectory(MinifingJSON);
            }

            File.WriteAllText(MinifingJSON + "/MinifingInstantResult.json", myJSON);
            File.WriteAllText(MinifingJSON + "/MinifingStaticResult.json", instantResult);

            //using AutoMapper
            var productsMapp = context.Products
                                      .Where(p => p.Price >= 500 && p.Price <= 1000)
                                      .OrderBy(p => p.Price)
                                      .ProjectTo<ListOfProductsInRangeDTO>(mapper.ConfigurationProvider)
                                      .ToList();

            string productsDTO = JsonConvert.SerializeObject(productsMapp, Formatting.Indented);
            File.WriteAllText(Results + "/ResultDTO.json", productsDTO);

            return json;
        }

        //Query 6. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context, IMapper mapper)
        {
            var usersSoldProducts = context.Users
                                        .Where(u => u.ProductsSold.Any(x => x.Buyer != null))
                                        .OrderBy(u => u.LastName)
                                        .ThenBy(u => u.FirstName)
                                        .Select(u => new
                                        {
                                            firstName = u.FirstName,
                                            lastName = u.LastName,
                                            soldProducts = u.ProductsSold
                                                .Where(p => p.Buyer != null)
                                                .Select(p => new
                                                {
                                                    name = p.Name,
                                                    price = p.Price,
                                                    buyerFirstName = p.Buyer.FirstName,
                                                    buyerLastName = p.Buyer.LastName
                                                })
                                                .ToArray()
                                        })
                                        .ToArray();

            string json = JsonConvert.SerializeObject(usersSoldProducts, Formatting.Indented);

            //AutoMapping With DTO
            var usersSoldProductsDTO = context.Users
                                        .Where(u => u.ProductsSold.Any(x => x.Buyer != null))
                                        .OrderBy(u => u.LastName)
                                        .ThenBy(u => u.FirstName)
                                        .ProjectTo<UserWithSoldProducts>(mapper.ConfigurationProvider)
                                        .ToArray();

            var result = JsonConvert.SerializeObject(usersSoldProductsDTO, Formatting.Indented);

            File.WriteAllText(Results + "/UserWithSoldProductsDTO.json", result);

            return json;
        }

        //Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context, IMapper mapper)
        {
            var categories = context.Categories
                                .Select(c => new
                                {
                                    category = c.Name,
                                    productsCount = c.CategoryProducts.Count(),
                                    averagePrice = c.CategoryProducts
                                                    .Average(x => x.Product.Price).ToString("F2"),
                                    totalRevenue = c.CategoryProducts.Where(x => x.Product.BuyerId != null)
                                                    .Sum(x => x.Product.Price).ToString("F2")
                                })
                                .OrderByDescending(c => c.productsCount)
                                .ToArray();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            //AutoMapping With DTO
            var categoriesDTO = context.Categories
                                    .ProjectTo<CategoriesByProductsCountDTO>(mapper.ConfigurationProvider)
                                    .OrderByDescending(x => x.ProductsCount)
                                    .ToArray();

            var categoriesJSON = JsonConvert.SerializeObject(categoriesDTO, Formatting.Indented);

            File.WriteAllText(Results + "/categories-by-products-count.json", categoriesJSON);

            return json;
        }

        //Query 7. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context, IMapper mapper)
        {
            var users = context.Users
                            .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                            .Select(u => new
                            {
                                lastName = u.LastName,
                                age = u.Age,
                                soldProducts = new
                                {
                                    count = u.ProductsSold.Count(p => p.BuyerId != null),
                                    products = u.ProductsSold.Where(p => p.BuyerId != null)
                                                .Select(p => new
                                                {
                                                    name = p.Name,
                                                    price = p.Price
                                                })                                                
                                                .ToArray()
                                                
                                }
                            })
                            .OrderByDescending(u => u.soldProducts.count)
                            .ToArray();

            var result = new
            {
                usersCount = users.Count(),
                users = users
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            var json = JsonConvert.SerializeObject(result, settings);

            //AutoMapping With DTO
            var usersDTO = context.Users
                            .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                            .ProjectTo<SoldedProductsDTO>(mapper.ConfigurationProvider)
                            .OrderByDescending(u => u.Count)
                            .ToArray();



            //var usersResult = new
            //{
            //    usersCount = usersDTO.Count(),
            //    users = usersDTO
            //};

            return json;
        }
    }
}