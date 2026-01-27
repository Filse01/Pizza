using Microsoft.EntityFrameworkCore;
using Pizza.Data;
using Pizza.Models;
using Pizza.Services.Contracts;
using Pizza.ViewModels;
using Pizza.ViewModels.Orders;

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

    public async Task<IEnumerable<OrderIndexViewModel>> GetAllOrders()
    {
        var orders = await context.Orders
            .Include(o => o.Pizzas)
            .ThenInclude(p => p.Pizza)
            .Select(o => new OrderIndexViewModel()
            {
                Id = o.Id,
                Address = o.Address,
                FirstName = o.FirstName,
                LastName = o.LastName,
                OrderDate = o.OrderDate,
                OrderItems = o.Pizzas,
                Price = o.Pizzas.Where(p => p.OrderId == o.Id).Sum(p => p.UnitPrice),
                PhoneNumber = o.PhoneNumber
            })
            .ToListAsync();
        return orders;


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
                    ImageUrl = pizza.ImageUrl
                };
                return model;
            }
        }
        return null;
    }

    public async Task<bool> EditPizza(EditPageViewModel model)
    {
        var pizza = await context.Pizzas
            .SingleOrDefaultAsync(p => p.Id == model.Pizza.Id);
        if (pizza != null)
        {
            pizza.Name = model.Pizza.Name;
            pizza.Price = model.Pizza.Price;
            pizza.ImageUrl = model.Pizza.ImageUrl;
            pizza.Description = model.Pizza.Description;
            await context.SaveChangesAsync();
            return true;
        }
        return false;
}

    public async Task<bool> AddIngredient(Guid ingId, Guid pizzaId)
    {
        if (ingId != null && pizzaId != null)
        {
            var ingredient = new PizzaIngredient()
            {
                IngredientId = ingId,
                PizzaId = pizzaId
            };
            await context.PizzaIngredients.AddAsync(ingredient);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> RemoveIngredient(Guid ingId, Guid pizzaId)
    {
        if (ingId != null && pizzaId != null)
        {
            var ingredient = new PizzaIngredient()
            {
                IngredientId = ingId,
                PizzaId = pizzaId
            };
            context.PizzaIngredients.Remove(ingredient);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
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

    public async Task<bool> DeleteIngredient(Guid? id)
    {
        if (id != null)
        {
            var ing = await context.Ingredients.SingleOrDefaultAsync(i => i.Id == id);
            if (ing != null)
            {
                context.Remove(ing);
                await context.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<bool> CreatePizza(EditPageViewModel model)
    {
        if (model != null)
        {
            var pizza = new Models.Pizza()
            {
                Id = Guid.NewGuid(),
                Name = model.Pizza.Name,
                Price = model.Pizza.Price,
                ImageUrl = model.Pizza.ImageUrl,
                Description = model.Pizza.Description,
            };
            if (pizza != null)
            {
                await context.Pizzas.AddAsync(pizza);
                await context.SaveChangesAsync();
                foreach (var ingredient in model.Ingredients)
                {
                    var pizzaIng = new PizzaIngredient()
                    {
                        IngredientId = ingredient.Id,
                        PizzaId = pizza.Id
                    };
                    await context.PizzaIngredients.AddAsync(pizzaIng);
                }
                await context.SaveChangesAsync();
                return true;
            }

            
        }
        return false;
    }
}