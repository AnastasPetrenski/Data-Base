using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EFDemoDBFirst.Recipes
{
    [Table("Ingredient")]
    public partial class Ingredient
    {
        public Ingredient()
        {
            Recipes = new HashSet<Recipe>();
        }

        [Key]
        [Column("IngredientID")]
        public int IngredientId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int Quantity { get; set; }

        [Column("RecipeID")]
        public int? RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        [InverseProperty("Ingredients")]
        public virtual Recipe Recipe { get; set; }

        [InverseProperty("Ingredient")]
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
