namespace Pizza.ViewModels;

public class EditPizzasViewModel
{
    public IEnumerable<EditPizzasIndexViewModel> Pizzas { get; set; } = new List<EditPizzasIndexViewModel>();
    public IEnumerable<EditPizzasIngredientsViewModel> Ingredients { get; set; } = new List<EditPizzasIngredientsViewModel>();
    
}