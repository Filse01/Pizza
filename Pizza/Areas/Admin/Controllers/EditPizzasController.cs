using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Data;
using Pizza.Services;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Areas.Admin.Controllers;

[Area("Admin")]
public class EditPizzasController : Controller
{
    private IAdminService _adminService;

    public EditPizzasController(IAdminService adminService)
    {
        this._adminService = adminService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var piazzas = await _adminService.GetAllPizzas();
        var ingredients = await _adminService.GetAllIngredients();
        var model = new EditPizzasViewModel()
        {
            Pizzas = piazzas,
            Ingredients = ingredients
        };
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> EditPizza(Guid id)
    {
        var pizza = await _adminService.GetAPizza(id);
        var ings = await _adminService.GetAllIngredients();
        var model = new EditPageViewModel()
        {
            Pizza = pizza,
            Ingredients = ings,
        };
        
        return View(model);
    }

    public async Task<IActionResult> DeletePizza(Guid? id)
    {
        bool result = await _adminService.DeletePizza(id);
        TempData["SuccessMessage"] = result ? "Pizza deleted successfully" : "Pizza not deleted";
        return RedirectToAction("Index");
    }
}