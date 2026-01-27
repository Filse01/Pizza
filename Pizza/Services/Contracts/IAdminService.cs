using Pizza.ViewModels;
using Pizza.ViewModels.Orders;

namespace Pizza.Services.Contracts;

public interface IAdminService
{
    Task<IEnumerable<EditPizzasIndexViewModel>> GetAllPizzas();
    Task<IEnumerable<EditPizzasIngredientsViewModel>> GetAllIngredients();
    Task<IEnumerable<OrderIndexViewModel>> GetAllOrders();
    
    Task<EditPizzaViewModel> GetAPizza(Guid? id);
    Task<bool> EditPizza(EditPageViewModel model);
    Task<bool> AddIngredient(Guid ingId, Guid pizzaId);
    Task<bool> RemoveIngredient(Guid ingId, Guid pizzaId);
    Task<bool> DeletePizza(Guid? id);
    Task<bool> DeleteIngredient(Guid? id);
    Task<bool> CreatePizza(EditPageViewModel model);
    

}