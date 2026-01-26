using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Controllers;
[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartService.GetCart(userId);
        var model = new AddOrderPageViewModel()
        {
            Order = null,
            Cart = cart
        };
        return View(model);
    }

    public async Task<IActionResult> AddToCart(Guid id)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool result = await _cartService.AddToCart(userId, id);
        if (result == true)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index), "Menu");
    }
    [HttpPost]
    public async Task<IActionResult> AddOrder(AddOrderPageViewModel model)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartService.GetCart(userId);
        model.Cart = cart;
        ModelState.Clear();
        if (TryValidateModel(model) && userId != null)
        {
            bool result = await _cartService.CreateOrder(model, userId);
            if (result == true)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View("Index", model);
    }
}