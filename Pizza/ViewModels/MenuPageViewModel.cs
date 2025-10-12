namespace Pizza.ViewModels;

public class MenuPageViewModel
{
    public IEnumerable<IndexMenuViewModel> Pizzas { get; set; }
    public IEnumerable<IngredientsViewModel> Ingredients { get; set; }
}