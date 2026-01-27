using System.ComponentModel.DataAnnotations;

namespace Pizza.ViewModels;

public class AddOrderViewModel
{
    [MaxLength(50)] 
    public string FirstName { get; set; } = null!;
    [MaxLength(50)] 
    public string LastName { get; set; } = null!;
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
}