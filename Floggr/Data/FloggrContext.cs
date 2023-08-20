using Microsoft.EntityFrameworkCore;
using RootFoundationFoods;

namespace Floggr.Data
{
    public class FloggrContext : DbContext
    {
        public FloggrContext(DbContextOptions<FloggrContext> options)
            : base(options)
        {
        }

        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<FoodNutrient> FoodNutrients { get; set; }
        public DbSet<FoodNutrientDerivation> FoodNutrientDerivations { get; set; }
        public DbSet<FoodNutrientSource> FoodNutrientSources { get; set; }
        public DbSet<FoodPortion> FoodPortions { get; set; }
        public DbSet<FoundationFood> FoundationFoods { get; set; }
        public DbSet<InputFood> InputFoods { get; set; }

        public DbSet<InputFood2> InputFoods2 { get; set; }
        public DbSet<MeasureUnit> MeasureUnits { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }
        public DbSet<NutrientConversionFactor> NutrientConversionFactors { get; set;}
        public DbSet<Root> RootFoundationFoods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoodCategory>().ToTable("FoodCategories");
            modelBuilder.Entity<FoodNutrient>().ToTable("FoodNutrients");
            modelBuilder.Entity<FoodNutrientDerivation>().ToTable("FoodNutrientDerivations");
            modelBuilder.Entity<FoodNutrientSource>().ToTable("FoodNutrientSource");
            modelBuilder.Entity<FoodPortion>().ToTable("FoodPortions");
            modelBuilder.Entity<FoundationFood>().ToTable("FoundationFoods");
            modelBuilder.Entity<InputFood>().ToTable("InputFoods");
            modelBuilder.Entity<InputFood2>().ToTable("InputFoods2");
            modelBuilder.Entity<MeasureUnit>().ToTable("MeasureUnits");
            modelBuilder.Entity<Nutrient>().ToTable("Nutrients");
            modelBuilder.Entity<NutrientConversionFactor>().ToTable("NutrientConversionFactors");
            modelBuilder.Entity<Root>().ToTable("RootFoundationFoods");

        }
    }
}
