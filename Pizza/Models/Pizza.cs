using System.ComponentModel.DataAnnotations;

namespace Pizza.Models;

public class Pizza
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } =  null!;
    [Required]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public ICollection<PizzaIngredient> Ingredients { get; set; } = new List<PizzaIngredient>();

}