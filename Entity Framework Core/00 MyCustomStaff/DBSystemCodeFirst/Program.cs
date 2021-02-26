using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBSystemCodeFirst
{
    class Program
    {
        static void Main(string[] args)
        {

            using var db = new RecipesDbContext();
            //db.Database.Migrate();
            //PopulateDataBase(db);

            //TODO Create Author class
            //public string Author { get; set; }

            //public class Author
            //public int Id { get; set; }
            //public string Name { get; set; }
            //public ICollection<Book> Books { get; set; }
            //public ICollection<Recipe> Recipes { get; set; }

            //public class Book
            //public int Id { get; set; }
            //public Recipe Recipes { get; set; }
            //public Author Author { get; set; }

            //var isExist = db.Database.EnsureCreated();
            //if (isExist)
            //{
            //    PopulateDataBase(db);
            //}

            var service = new RecipesServices();
            service.ChangeRecipeName(100, "Meat");

            service.RemoveRecipe("Drop4eta");
            
            db.SaveChanges();

            var result = service.GetTop10PopularRecipes();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(service.PrintRecipesIngredients());

            //N+1 Problem on Foreign key accoure
            Console.WriteLine("N+1 Problem on Foreign key accoure");
            Console.WriteLine(service.Nplus1Problem());

            var useProectionOfAnonymousMethod = db.Ingredients
                .Select(ingredient =>
                new
                {
                    Name = ingredient.Name,
                    Quantity = ingredient.Quantity,
                    Recipe = ingredient.Recipe.Name
                });

            foreach (var item in useProectionOfAnonymousMethod)
            {
                Console.WriteLine($"{item.Name} {item.Quantity} gr.");
                Console.WriteLine($"Ingredients: {string.Join(" ", item.Recipe)}");
            }
        }       

        private static void PopulateDataBase(RecipesDbContext db)
        {
            db.Add(new Recipe { Name = "Musaka", Description = "Nqma" });
            db.Add(new Recipe { Name = "Pacha" });
            db.Add(new Recipe { Name = "Chorba" });
            db.Add(new Recipe { Name = "Drob" });

            for (int i = 0; i < 20; i++)
            {
                db.Recipes.Add(new Recipe
                {
                    Name = i.ToString() + " Recipe",
                    Description = $"MyHashCode: {i.GetHashCode().ToString()}",
                    Ingredients = new List<Ingredient>
                        {
                            new Ingredient { Name = "Salt", Quantity = 50 + i },
                            new Ingredient { Name = "Meat", Quantity = 100 + i}
                        }
                });
            }
        }
    }
}
