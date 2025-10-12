using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza.Models;

namespace Pizza.Data.Config;

public class PizzaIngredientConfig :  IEntityTypeConfiguration<PizzaIngredient>
{
    public void Configure(EntityTypeBuilder<PizzaIngredient> builder)
    {
        builder.HasKey(p => new { p.PizzaId, p.IngredientId });
    }
}