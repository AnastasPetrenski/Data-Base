using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EFDemoDBFirst.Recipes
{
    [Table("Recipe")]
    public partial class Recipe
    {
        public Recipe()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        [Key]
        [Column("RecipeID")]
        public int RecipeId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column("IngredientID")]
        public int? IngredientId { get; set; }

        [ForeignKey(nameof(IngredientId))]
        [InverseProperty("Recipes")]
        public virtual Ingredient Ingredient { get; set; }
        [InverseProperty("Recipe")]
        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
