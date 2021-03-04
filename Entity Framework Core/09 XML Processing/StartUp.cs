using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var relativeUserPath = GetUserDirectoryPath();
            //var relativeProductPath = GetProductDirectoryPath();
            //var relativeCategoryPath = GetCategoryDirectoryPath();
            //var relativeCategoryProductPath = GetCategoryProductDirectoryPath();

            //var inputUsersXML = File.ReadAllText(relativeUserPath);
            //var inputProductsXML = File.ReadAllText(relativeProductPath);
            //var inputCategoriesXML = File.ReadAllText(relativeCategoryPath);
            //var inputCategoriesProductsXML = File.ReadAllText(relativeCategoryProductPath);

            //var userResult = ImportUsers(context, inputUsersXML);
            //var result = ImportProducts(context, inputProductsXML);
            //var categoryResult = ImportCategories(context, inputCategoriesXML);
            //var categoryProductResult = ImportCategoryProducts(context, inputCategoriesProductsXML);

            var result = GetSoldProducts(context);

            if (!Directory.Exists("../../../Result"))
            {
                Directory.CreateDirectory("../../../Result");
            }

            File.WriteAllText("../../../Result/users-sold-products.xml", result);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {

        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                               .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                               .Select(u => new ExportUsersSoldProductsDTO
                               {
                                   FirstName = u.FirstName,
                                   LastName = u.LastName,
                                   SoldProducts = u.ProductsSold
                                                   //.Where(p => p.BuyerId != null)
                                                   .Select(x => new UserProductDTO
                                                   {
                                                       Name = x.Name,
                                                       Price = x.Price
                                                   })
                                                   .ToArray()
                               })
                               .OrderBy(u => u.LastName)
                               .ThenBy(u => u.FirstName)
                               .Take(5)
                               .ToArray();

            var root = "Users";

            var result = XmlConverter.Serialize(users, root);

            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                                  .Where(p => p.Price >= 500 && p.Price <= 1000)
                                  .OrderBy(p => p.Price)
                                  .Select(p => new ExportProductDTO
                                  {
                                      Name = p.Name,
                                      Price = p.Price,
                                      Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                                  })
                                  .Take(10)
                                  .ToArray();

            var root = "Products";

            var result = XMLHelper.XmlConverter.Serialize(products, root);

            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var root = "CategoryProducts";

            var deserializedObjects = XmlConverter.Deserializer<ImportCategoryProductDTO>(inputXml, root);

            var result = deserializedObjects
                            .Where(c => context.Categories.Any(x => x.Id == c.CategoryId) &&
                                        context.Products.Any(x => x.Id == c.ProductId))
                            .Select(c => new CategoryProduct
                            {
                                CategoryId = c.CategoryId,
                                ProductId = c.ProductId
                            })
                            .ToList();

            context.CategoryProducts.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var root = "Categories";

            var deserializedCateries = XmlConverter.Deserializer<ImportCategoryDTO>(inputXml, root);

            var categories = deserializedCateries
                                .Where(x => x.Name != null)
                                .Select(x => new Category()
                                {
                                    Name = x.Name
                                })
                                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var root = "Products";
            var deserializedObjects = DeserializeXML<ImportProductDTO>(inputXml, root);

            var usingXMLHelper = XmlConverter.Deserializer<ImportProductDTO>(inputXml, root);

            var result = usingXMLHelper
                            .Select(x => new Product
                            {
                                Name = x.Name,
                                Price = x.Price,
                                BuyerId = x.BuyerId,
                                SellerId = x.SellerId
                            })
                            .ToList();

            context.Products.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Users");

            var serializer = new XmlSerializer(typeof(ImportUserDTO[]), root);

            var inportedUsers = serializer.Deserialize(new StringReader(inputXml)) as ImportUserDTO[];

            var result = inportedUsers
                            .Select(u => new User
                            {
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Age = u.Age
                            })
                            .ToList();

            context.Users.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }


        private static T[] DeserializeXML<T>(string inputXml, string root)
        {
            var serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));

            var deserializedObjects = serializer.Deserialize(new StringReader(inputXml)) as T[];

            return deserializedObjects;
        }

        private static T DeserializeXML<T>(string inputXML, string root, bool isSampleObject)
            where T : class
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            var deserializedObjects = serializer.Deserialize(new StringReader(inputXML)) as T;

            return deserializedObjects;
        }

        private static string GetUserDirectoryPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            return directories.Any(x => x.Contains("Datasets")) ? "/users.xml" : "../../../Datasets/users.xml";
        }

        private static string GetProductDirectoryPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            return directories.Any(x => x.Contains("Datasets")) ? "/products.xml" : "../../../Datasets/products.xml";
        }

        private static string GetCategoryDirectoryPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            return directories.Any(x => x.Contains("Datasets")) ? "/categories.xml" : "../../../Datasets/categories.xml";
        }

        private static string GetCategoryProductDirectoryPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directories = Directory.GetDirectories(currentDirectory);

            return directories.Any(x => x.Contains("Datasets")) ?
                                    "/categories-products.xml" :
                                    "../../../Datasets/categories-products.xml";
        }

        private static XmlSerializerNamespaces GetXmlNamespaces()
        {
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);
            return xmlNamespaces;
        }

        //var builder = new StringBuilder();
        //using var writer = new StringWriter(builder);
        //Stream stream = new MemoryStream();

        //    using (TextWriter text = new StreamWriter(stream, Encoding.UTF8))
        //    {
        //        serializer.Serialize(stream, writer);
        //    }
    }
}