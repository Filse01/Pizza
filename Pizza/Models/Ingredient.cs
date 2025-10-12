using System.ComponentModel.DataAnnotations;

namespace Pizza.Models;

public class Ingredient
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<PizzaIngredient> Pizzas { get; set; } = new List<PizzaIngredient>();
}