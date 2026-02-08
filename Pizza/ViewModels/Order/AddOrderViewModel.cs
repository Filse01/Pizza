using System.ComponentModel.DataAnnotations;

namespace Pizza.ViewModels;

public class AddOrderViewModel
{
    [MaxLength(50)] 
    [Required]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)] 
    [Required]
    public string LastName { get; set; } = null!;

    [Required] 
    public string Address { get; set; } = null!;
    [Required]
    public string PhoneNumber { get; set; }
}