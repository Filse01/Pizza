using Pizza.Models;

namespace Pizza.ViewModels;

public class AddIngredient
{
    public Guid PizzaId { get; set; }
    public Models.Pizza Pizza { get; set; } = null!;
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
}