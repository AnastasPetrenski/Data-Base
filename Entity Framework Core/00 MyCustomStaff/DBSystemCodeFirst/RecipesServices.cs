using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBSystemCodeFirst
{
    public class RecipesServices
    {
        private RecipesDbContext db;

        public RecipesServices()
        {
            this.db = new RecipesDbContext();
        }

        public string RemoveRecipe(string ingredientName)
        {
            var removeEntity = db.Recipes.FirstOrDefault(x => x.Name == ingredientName);
            if (removeEntity != null)
            {
                string recipeName = removeEntity.Name;
                db.Recipes.Remove(removeEntity);
                return $"Recipe {recipeName}, with ingredient {ingredientName}";
            }

            return $"There is no recipe with ingredient {ingredientName}";
        }

        public string ChangeRecipeName(int quantity, string ingredient)
        {
            string recipeNewName = "Banana";

            var updateEntity = db.Recipes
                .FirstOrDefault(x => x.Ingredients.Any(j => j.Quantity == quantity && j.Name == ingredient));
            if (updateEntity != null)
            {
                var recipeOldName = updateEntity.Name;
                updateEntity.Name = recipeNewName;
                return $"Successfully changed recipe's name {recipeOldName} to {recipeNewName}";
            }

            return $"There is no recipe with gradient:{ingredient}:{quantity} gr.";
        }

        public string PrintRecipesIngredients()
        {
            var sb = new StringBuilder();

            var resultQ = db.Recipes.Select(x =>
            new
                {
                    Name = x.Name,
                    Ingredients = new List<Ingredient>(
                        db.Ingredients.Where(y => y.RecipeID == x.RecipeID).ToList())
                })
            .ToList();

            foreach (var item in resultQ)
            {
                int counter = 1;
                sb.AppendLine($"{item.Name} ingredients:");
                foreach (var ingredient in item.Ingredients)
                {
                    sb.AppendLine($"    {counter++}.{ingredient}");
                }
                //sb.AppendLine(string.Join(Environment.NewLine, item.Ingredients));
            }

            return sb.ToString().TrimEnd();

            var query =
                db.Recipes.Where(recip =>
                    db.Ingredients.All(ingredient =>
                        recip.RecipeID == ingredient.RecipeID));

            var resultJoin = db.Recipes.Join(db.Ingredients, r => r.RecipeID, i => i.RecipeID, (r, i) => r);

            foreach (var item in resultJoin)
            {
                Console.WriteLine(item.Name);
                foreach (var i in item.Ingredients)
                {
                    Console.WriteLine(i.Name, i.Quantity);
                    Console.WriteLine("Next entity:");
                }
            }

            var result = db.Recipes.GroupJoin(db.Ingredients, r => r.RecipeID, i => i.RecipeID,
                                 (r, i) => new { Key = r.Name, list = i });

            foreach (var item in result)
            {
                sb.AppendLine($"Recipe name: {item.Key}");

                sb.AppendLine($"    -{string.Join(", ", item.list)}");
            }

            return sb.ToString().TrimEnd();
        }

        public IList<Recipe> GetTop10PopularRecipes()
        {
            return db.Recipes.OrderByDescending(x => x.RecipeID).Take(10).ToList();
        }

        public string Nplus1Problem()
        {
            var sb = new StringBuilder();

            foreach (var item in db.Recipes)
            {
                sb.AppendLine($"WE take all information for entity :name, description ...");
                sb.Append($"{item}");
                sb.AppendLine();
            }
            sb.AppendLine("********************************************");

            return sb.ToString().TrimEnd();
        }
    }
}
