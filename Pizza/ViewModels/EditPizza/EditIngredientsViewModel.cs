namespace Pizza.ViewModels;

public class EditIngredientsViewModel
{
    public IEnumerable<EditPizzasIngredientsViewModel> Ingredients { get; set; } = new List<EditPizzasIngredientsViewModel>();
}