using Pizza.Migrations;
using OrderItem = Pizza.Models.OrderItem;

namespace Pizza.ViewModels.Orders;

public class OrderIndexViewModel
{
    public Guid  Id { get; set; }
    public string UserId  { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber  { get; set; }
    public decimal Price  { get; set; }
}