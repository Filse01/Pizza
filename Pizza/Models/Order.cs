namespace Pizza.Models;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<Pizza> Pizzas { get; set; }
}