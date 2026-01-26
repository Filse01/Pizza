namespace Pizza.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}