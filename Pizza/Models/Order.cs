using System.ComponentModel.DataAnnotations;

namespace Pizza.Models;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Pizzas { get; set; }
    [MaxLength(50)] 
    public string FirstName { get; set; } = null!;
    [MaxLength(50)] 
    public string LastName { get; set; } = null!;
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    
    public string OrderStatus { get; set; }
}