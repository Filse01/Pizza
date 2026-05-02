using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pizza.Secrets;
using Pizza.Services.Contracts;
using Pizza.ViewModels;
using Stripe;
using Stripe.Checkout;

namespace Pizza.Controllers;
[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly StripeOptions _stripeSettings;

    public CartController(ICartService cartService,  IOptions<StripeOptions> stripeSettings)
    {
        _cartService = cartService;
        _stripeSettings = stripeSettings.Value;
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
            bool mailResult = await _cartService.CreateMail(userId);
            if (result == true)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return View("Index", model);
    }

    
    public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && cartItemId != null)
        {
            bool result = await _cartService.RemoveFromCart(userId, cartItemId);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        return BadRequest("Failed");
    }
    [HttpGet]
    public async Task<int?> ApplyCoupon([FromQuery] string couponName)
    {
        var result = await _cartService.ApplyCouponFrontend(couponName);
        return result.Percentage;
    }
    [Authorize]
    public async Task<AddOrderPageViewModel> ApplyCouponBackend(AddOrderPageViewModel model, int percentage)
    {
        model.Cart.Price = (model.Cart.Price * (100 - percentage) / 100);
        return model;
    }

    [HttpGet]
    public async Task<IActionResult> Success()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Cancel()
    {
        return View();
    }
    public IActionResult CreateCheckoutSession(string amount)
    {
        var currency = "eur";
        var successUrl = "http://localhost:5069/Cart/Success";
        var cancelUrl = "http://localhost:5069/Cart/Cancel";

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = currency,
                        UnitAmountDecimal = Convert.ToDecimal(amount) * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Pizza",
                            Description = "Pizza"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl

        };
        var service = new SessionService();
        var session = service.Create(options);
        return Redirect(session.Url);
    }
}