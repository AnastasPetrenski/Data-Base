using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DBSystemCodeFirst
{
    public class Ingredient
    {
        public Ingredient()
        {
            this.Recipes = new List<Recipe>();
        }

        [Key]
        public int IngredientID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public double Quantity { get; set; }

        [Column("RecipeID")]
        public int? RecipeID { get; set; }

        [ForeignKey(nameof(RecipeID))]
        public virtual Recipe Recipe { get; set; }

        [InverseProperty(nameof(Ingredient))]
        public virtual ICollection<Recipe> Recipes { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, Quantity:{Quantity}";
        }
    }
}
