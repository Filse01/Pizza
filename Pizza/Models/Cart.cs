using System.ComponentModel.DataAnnotations;

namespace Pizza.Models;

public class Cart
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public IEnumerable<CartItem> CartItems { get; set; } =  new List<CartItem>();
}