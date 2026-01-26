using System.ComponentModel.DataAnnotations;

namespace Pizza.Models;

public class CartItem
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid CartId { get; set; }
    [Required]
    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; }
    [Required]
    public int Quantity { get; set; }
}