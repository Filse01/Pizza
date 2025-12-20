using Pizza.ViewModels;

namespace Pizza.Services.Contracts;

public interface IAdminService
{
    Task<IEnumerable<EditPizzasIndexViewModel>> GetAllPizzas();
    Task<IEnumerable<EditPizzasIngredientsViewModel>> GetAllIngredients();
    Task<EditPizzaViewModel> GetAPizza(Guid? id);
    Task<bool> DeletePizza(Guid? id);

}