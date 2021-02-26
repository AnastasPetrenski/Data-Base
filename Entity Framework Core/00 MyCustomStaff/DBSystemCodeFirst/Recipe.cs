using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBSystemCodeFirst
{
    public class Recipe
    {
        public Recipe()
        {
            this.Ingredients = new List<Ingredient>();
        }

        [Key]
        [Required]
        public int RecipeID { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? IngredientID { get; set; }

        public Ingredient Ingredient { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public override string ToString()
        {
            return $"{RecipeID}.{Name}{Environment.NewLine}{string.Join(", ", Ingredients)}";

        }
    }
}
