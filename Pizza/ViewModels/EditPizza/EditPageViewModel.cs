using Pizza.Models;

namespace Pizza.ViewModels;

public class EditPageViewModel
{
    public EditPizzaViewModel Pizza { get; set; }
    public IEnumerable<EditPizzasIngredientsViewModel> Ingredients { get; set; } = new List<EditPizzasIngredientsViewModel>();
}