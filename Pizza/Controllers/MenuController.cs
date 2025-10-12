using Microsoft.AspNetCore.Mvc;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Controllers;

public class MenuController : Controller
{
    private readonly IPizzaService _pizzaService;

    public MenuController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var pizzas = await _pizzaService.GetAllPizzas();
        var ings = await _pizzaService.GetAllIngredients();
        var model = new MenuPageViewModel
        {
            Pizzas = pizzas,
            Ingredients = ings,
        };
        return View(model);
    }

    public async Task<IActionResult> FilterPizzas([FromQuery] List<Guid> ingredients)
    {
        var pizzas = await _pizzaService.GetFilteredPizzas(ingredients);
        return PartialView("_PizzaListPartial", pizzas);

    }
}