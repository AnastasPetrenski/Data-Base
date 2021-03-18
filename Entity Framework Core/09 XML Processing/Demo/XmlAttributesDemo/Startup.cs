using System.Xml.Serialization;
using XmlAttributesDemo.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace XmlAttributesDemo
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            //XDocument
            XDocument xmlDoc = new XDocument();
            var root = new XElement("cinema");
            xmlDoc.Add(root);
            xmlDoc.Root.SetAttributeValue("cinema", "Arena Sofia");

            XAttribute attribute = new XAttribute("Name", 2);
            //root.Add("Movies");

            for (int i = 0; i < 20; i++)
            {
                var movie = new XElement("movie");
                root.Add(movie);
                movie.Add(new XElement("title", "Moon" + i.ToString()));
                movie.Add(new XElement("price", (12.99 + i).ToString()));
            }

            xmlDoc.Root.SetAttributeValue("movies", xmlDoc.Root.Elements().Count());
            xmlDoc.Save("cinema.xml");

            //LINQ Queries
            var document = XDocument.Load("cinema.xml");
            var element = document.Root.Elements()
                .Where(x => x.Element("title").Value.ToString().Contains("5"))
                .Select(x => new
                {
                    Name = x.Element("title").Value
                })
                .ToList();
            //.First(x => x.Element("Title").Value == "Moon19");
            var directory = Directory.GetCurrentDirectory();
            var directories = Directory.GetFiles(directory);

            var text = File.ReadAllText(directories[0]);
            var serializer = new XmlSerializer(typeof(MoviesDTO[]), new XmlRootAttribute("cinema"));
            var tickets = serializer.Deserialize(File.OpenRead("cinema.xml")) as MoviesDTO[];

            var listOfNewMovies = new MoviesDTO[] { new MoviesDTO() { Title = "Rambo", Price = "96.00" },
                                                    new MoviesDTO() { Title = "RamboII", Price = "96.00" } };

            serializer.Serialize(File.OpenWrite("newmovies.xml"), listOfNewMovies);

            var binaryFormater = new BinaryFormatter();
            binaryFormater.Serialize(File.OpenWrite("movies.bin"), tickets);
           

            //DEMO   
            var ser = new XmlSerializer(typeof(LibraryDto[]), new XmlRootAttribute("Libraries"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            var libraries = GetLibraries();

            using (TextWriter writer = new StreamWriter("../../../Library.xml"))
            {
                ser.Serialize(writer, libraries, namespaces);
            }
        }

        private static LibraryDto[] GetLibraries()
        {
            LibraryDto firstLibrary = new LibraryDto
            {
                LibraryName = "Jo Bowl",
                Sections = new SectionDto()
                {
                    Name = "Horror",
                    Books = new BookDto[]
                    {
                        new BookDto() {
                            Name = "It",
                            Author = "Stephen King", Description = "Here you can put description about the book"
                        },
                        new BookDto() {
                            Name = "Frankenstein",
                            Author = "Mary Shelley", Description = "Here you can put description about the book"
                        }
                    }
                },
                CardPrice = 20.30m
            };

            LibraryDto secondLibrary = new LibraryDto
            {
                LibraryName = "Kevin Sanchez",
                Sections = new SectionDto()
                {
                    Name = "Comedy",
                    Books = new BookDto[]
                    {
                        new BookDto()
                        {
                            Name = "The Diary of a Nobody",
                            Author = "George Grossmith and Weeden Grossmith",
                            Description = "Here you can put description about the book"
                        },
                        new BookDto()
                        {
                            Name = "Queen Lucia",
                            Author = "E F Benson",
                            Description = "Here you can put description about the book"
                        }
                    }
                },
                CardPrice = 43.35m
            };

            return new LibraryDto[] { firstLibrary, secondLibrary };
        }
    }
}
