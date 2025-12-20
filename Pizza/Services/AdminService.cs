using Microsoft.EntityFrameworkCore;
using Pizza.Data;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Services;

public class AdminService : IAdminService
{
    private readonly PizzaDbContext context;

    public AdminService(PizzaDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<EditPizzasIndexViewModel>> GetAllPizzas()
    {
        var pizzasList = await context.Pizzas
            .Include(p => p.Ingredients)
            .ThenInclude(p => p.Ingredient)
            .Select(p => new EditPizzasIndexViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Ingredients = p.Ingredients
            }).ToListAsync();
        if (pizzasList != null)
        {
            return pizzasList;
        }

        return null;
    }

    public async Task<IEnumerable<EditPizzasIngredientsViewModel>> GetAllIngredients()
    {
        var ings = await context.Ingredients
            .Select(i => new EditPizzasIngredientsViewModel()
            {
                Id = i.Id,
                Name = i.Name,
            }).ToListAsync();
        if (ings != null)
        {
            return ings;
        }
        return null;
    }

    public async Task<EditPizzaViewModel> GetAPizza(Guid? id)
    {
        if (id != null)
        {
            var pizza = await  context.Pizzas
                .Include(p => p.Ingredients)
                .ThenInclude(p => p.Ingredient)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (pizza != null)
            {
                var model = new EditPizzaViewModel()
                {
                    Id = pizza.Id,
                    Name = pizza.Name,
                    Price = pizza.Price,
                    Ingredients = pizza.Ingredients,
                    Description = pizza.Description,
                };
                return model;
            }
        }
        return null;
    }


    public async Task<bool> DeletePizza(Guid? id)
    {
        if (id != null)
        {
            Models.Pizza pizza = await context.Pizzas
                .SingleOrDefaultAsync(p => p.Id == id);
            if (pizza != null)
            {
                context.Pizzas.Remove(pizza);
                await context.SaveChangesAsync();
            }
            return true;
        }
        
        return false;
    }
    
}