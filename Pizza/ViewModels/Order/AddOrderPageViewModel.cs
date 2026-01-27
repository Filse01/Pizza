using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pizza.ViewModels;

public class AddOrderPageViewModel
{
    public AddOrderViewModel Order { get; set; }
    public CartViewModel Cart  { get; set; }
}