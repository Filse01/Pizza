using Pizza.ViewModels;

namespace Pizza.Services.Contracts;

public interface IPizzaService
{
    Task<IEnumerable<IndexMenuViewModel>> GetAllPizzas();
    
    Task<IEnumerable<IngredientsViewModel>> GetAllIngredients();
    
    Task<IEnumerable<IndexMenuViewModel>> GetFilteredPizzas(List<Guid> ingredients);
}