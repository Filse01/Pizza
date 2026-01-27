using Pizza.Models;

namespace Pizza.ViewModels;

public class CartViewModel
{
    public Guid Id { get; set; }
    public IEnumerable<CartItem> CartItems { get; set; } =  new List<CartItem>();
    
}