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
    // [HttpPost]
    // public async Task<IActionResult> AddOrder(AddOrderPageViewModel model)
    // {
    //     var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     var cart = await _cartService.GetCart(userId);
    //     model.Cart = cart;
    //     ModelState.Clear();
    //     if (TryValidateModel(model) && userId != null)
    //     {
    //         bool result = await _cartService.CreatePendingOrder(model, userId);
    //         bool mailResult = await _cartService.CreateMail(userId);
    //         if (result == true)
    //         {
    //             return RedirectToAction(nameof(Index));
    //         }
    //     }
    //     return View("Index", model);
    // }

    
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
    public async Task<IActionResult> Success(Guid id)
    {
        // 1. Find the order by ID
        // 2. Change status from "Pending" to "Paid"
        await _cartService.MarkOrderAsPaid(id);
    
        // 3. Send the confirmation email ONLY now that it's paid
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _cartService.CreateMail(userId);

        return View(); // Or a Thank You page
    }
    [HttpGet]
    public async Task<IActionResult> Cancel(Guid id)
    {
        // 1. Find the order by ID
        // 2. Change status from "Pending" to "Cancelled" (or just delete it from the DB)
        await _cartService.CancelOrder(id);

        // 3. Send them back to their cart to try again
        return RedirectToAction("Cancel"); 
    }
    public async Task<IActionResult> CreateCheckoutSession(AddOrderPageViewModel model)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currency = "eur";
        var cart = await _cartService.GetCart(userId);
        model.Cart = cart;
        ModelState.Clear();
        var finalPrice = cart.Price;
        
        if (!string.IsNullOrEmpty(model.Coupon?.Name))
        { 
            var couponResult = await _cartService.ApplyCouponFrontend(model.Coupon.Name);
            if (couponResult != null && couponResult.Percentage != null)
            {
                // Apply discount securely
                finalPrice = finalPrice * (100 - couponResult.Percentage.Value) / 100m;
            }
        }

        cart.Price = finalPrice;
        Guid orderId = await _cartService.CreatePendingOrder(model, userId);
        
        var successUrl = Url.Action("Success", "Cart", new { id = orderId }, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "Cart", new { id = orderId }, Request.Scheme);
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
                        UnitAmountDecimal = Math.Round(finalPrice * 100),
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
            CancelUrl = cancelUrl,
            ClientReferenceId = orderId.ToString()

        };
        var service = new SessionService();
        var session = service.Create(options);
        return Redirect(session.Url);
    }
}