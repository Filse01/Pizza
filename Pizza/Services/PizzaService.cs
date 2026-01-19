using Microsoft.EntityFrameworkCore;
using Pizza.Data;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Services;

public class PizzaService : IPizzaService
{
    private readonly PizzaDbContext context;

    public PizzaService(PizzaDbContext context)
    {
        this.context = context;
    }
    public async Task<IEnumerable<IndexMenuViewModel>> GetAllPizzas()
    {
        var pizzas = await context
            .Pizzas
            .Include(p => p.Ingredients)
            .ThenInclude(p => p.Ingredient)
            .Select(c =>  new IndexMenuViewModel
        {
            Id = c.Id,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            Name = c.Name,
            Price = c.Price,
            Ingredients = c.Ingredients
        }).ToListAsync();
        if (pizzas != null)
        {
            return pizzas;
        }

        return null;
    }

    public async Task<IEnumerable<IngredientsViewModel>> GetAllIngredients()
    {
        var ings = await context
            .Ingredients
            .Select(i => new IngredientsViewModel
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

    public async Task<IEnumerable<IndexMenuViewModel>> GetFilteredPizzas(List<Guid> ingredients)
    {
        var query = context
            .Pizzas
            .Include(p => p.Ingredients)
            .ThenInclude(p => p.Ingredient)
            .AsQueryable();

        if (ingredients != null && ingredients.Any())
        {
            query = query.Where(p => ingredients.All(i => p.Ingredients.Any(pi  => pi.IngredientId == i)));
        }

        return await query
            .Select(p => new IndexMenuViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Ingredients = p.Ingredients
            })
            .ToListAsync();
    }
}