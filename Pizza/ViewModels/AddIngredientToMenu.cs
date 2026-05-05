using System.ComponentModel.DataAnnotations;
using Pizza.Models;

namespace Pizza.ViewModels;

public class AddIngredientToMenu
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}