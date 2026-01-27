using Pizza.Models;

namespace Pizza.ViewModels;

public class EditPizzaViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public ICollection<PizzaIngredient> Ingredients { get; set; } = new List<PizzaIngredient>();
}