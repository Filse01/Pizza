using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Pizza.Models;

public class PizzaIngredient
{
    [Required]
    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; } = null!;
    [Required]
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
}