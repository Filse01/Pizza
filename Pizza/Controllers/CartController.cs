using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Pizza.Services.Contracts;

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
    public IActionResult Index()
    {
        return View();
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
}