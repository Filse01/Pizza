namespace Pizza.Models;

public class Coupon
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int DiscountPercentage { get; set; }
}