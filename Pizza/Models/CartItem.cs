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
    public Guid ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
}