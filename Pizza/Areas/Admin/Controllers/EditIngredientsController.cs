using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]

public class EditIngredientsController : Controller
{
    private IAdminService _adminService;

    public EditIngredientsController(IAdminService adminService)
    {
        this._adminService = adminService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var ingredients = await _adminService.GetAllIngredients();
        var model = new EditIngredientsViewModel()
        {
            Ingredients = ingredients
        };
        return View(model);
    }

    public async Task<IActionResult> DeleteIngredient(Guid id)
    {
        bool result = await _adminService.DeleteIngredient(id);
        return result ? RedirectToAction("Index") : View();
    }

    public async Task<IActionResult> AddIngredient()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddIngredient(AddIngredientToMenu model)
    {
        if (ModelState.IsValid)
        {
            bool result = await _adminService.AddIngredientToMenu(model);
            return RedirectToAction("Index");
        }
        return View(model);
    }
}