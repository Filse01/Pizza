using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pizza.Models;

namespace Pizza.Data;

public class PizzaDbContext : IdentityDbContext
{
    public PizzaDbContext(DbContextOptions<PizzaDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<PizzaIngredient> PizzaIngredients { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<Models.Pizza> Pizzas { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}