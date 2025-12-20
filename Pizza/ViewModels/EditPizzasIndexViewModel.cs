using Pizza.Models;

namespace Pizza.ViewModels;

public class EditPizzasIndexViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ICollection<PizzaIngredient> Ingredients { get; set; } =  new List<PizzaIngredient>();
    
}