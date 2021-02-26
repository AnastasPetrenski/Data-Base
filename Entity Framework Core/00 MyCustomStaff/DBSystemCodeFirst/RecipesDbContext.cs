using Microsoft.EntityFrameworkCore;

namespace DBSystemCodeFirst
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext()
        {
        }

        public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
            :base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=RecipesCodeFirst;Integrated Security=true;");
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
