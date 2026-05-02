using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Pizza.Data;
using Pizza.Models;
using Pizza.Secrets;
using Pizza.Services.Contracts;
using Pizza.ViewModels;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Pizza.Services;

public class CartService : ICartService
{
    private readonly PizzaDbContext context;
    private readonly UserManager<IdentityUser> userManager;
    private readonly GmailOptions gmailOptions;

    public CartService(PizzaDbContext context,  UserManager<IdentityUser> userManager,  IOptions<GmailOptions> gmailOptions)
    {
        this.context = context;
        this.userManager = userManager;
        this.gmailOptions = gmailOptions.Value;
    }
    public async Task<bool> AddToCart(string userId, Guid pizzaId)
    {
        if (userId != null && pizzaId != null)
        {
            if (!context.Carts.Any(u => u.UserId.ToString() == userId))
            {
                var cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                await context.AddAsync(cart);
                await context.SaveChangesAsync();
            }

            var cartItem = new CartItem()
            {
                Id = Guid.NewGuid(),
                CartId = context.Carts.SingleOrDefault(c => c.UserId == userId).Id,
                PizzaId = pizzaId,
                Quantity = 1
            };
            await context.AddAsync(cartItem);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    
    public async Task<bool> RemoveFromCart(string userId, Guid cartItemId)
    {
        if (userId != null && cartItemId != null)
        {
            var cart = await context.Carts
                .Include(p => p.CartItems)
                .SingleOrDefaultAsync(u => u.UserId.ToString() == userId);
            if (cart != null)
            {
                foreach (var item in cart.CartItems)
                {
                    if (item.Id == cartItemId)
                    {
                        context.Remove(item);
                    }
                }
            }
            await context.SaveChangesAsync();
            
            return true;
        }
        return false;
    }

    public async Task<CartViewModel> GetCart(string userId)
    {
        if (userId != null)
        {
            var cart = await context.Carts
                .Include(ci => ci.CartItems)
                .ThenInclude(p => p.Pizza)
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (cart != null)
            {
                var model = new CartViewModel()
                {
                    Id = cart.Id,
                    CartItems = cart.CartItems,
                    Price = cart.CartItems.Sum(c => c.Pizza.Price * c.Quantity),
                };
                return model;
            }
        }
        return null;
    }

    public async Task<Guid> CreatePendingOrder(AddOrderPageViewModel model, string userId)
    {
        if (model != null)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                FirstName = model.Order.FirstName,
                LastName = model.Order.LastName,
                UserId = userId,
                PhoneNumber = model.Order.PhoneNumber,
                OrderDate = DateTime.Now,
                Address = model.Order.Address,
                OrderStatus = "Pending"
            };
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var cartItem in model.Cart.CartItems)
            {
                orderItems.Add(new OrderItem()
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    PizzaId = cartItem.PizzaId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Pizza.Price * cartItem.Quantity
                });
            }

            if (model.Coupon.Name != null)
            {
                var coupon =
                    await context.Coupons.SingleOrDefaultAsync(c => c.Name.ToLower() == model.Coupon.Name.ToLower());
                if (coupon != null)
                {
                    foreach (var orderItem in orderItems)
                    {
                        orderItem.UnitPrice = (orderItem.UnitPrice * (100 - coupon.DiscountPercentage)) / 100;
                    }
                }
            }
            order.Pizzas = orderItems;
            await context.AddAsync(order);
            await context.SaveChangesAsync();
            context.RemoveRange(model.Cart.CartItems);
            await context.SaveChangesAsync();
            return order.Id;
        }
        return Guid.Empty;
    }

    public async Task<bool> CreateMail(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Philip Ognyanov", "philipdimitrov31@gmail.com"));
            message.To.Add(new MailboxAddress($"{user.UserName}", $"{user.Email}"));
            message.Subject = "Order";
            message.Body = new TextPart("plain")
            {
                Text = "Order created!"
            };
            
            var message1 = new MimeMessage();
            message1.From.Add(new MailboxAddress("Philip Ognyanov", "philipdimitrov31@gmail.com"));
            message1.To.Add(new MailboxAddress($"{user.UserName}", $"{user.Email}"));
            message1.Subject = "Order";
            message1.Body = new TextPart("plain")
            {
                Text = "Order created!"
            };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("philipdimitrov31@gmail.com", gmailOptions.AppPassword);
                await client.SendAsync(message);
                await client.SendAsync(message1);
                await client.DisconnectAsync(true);
            }

            return true;
        }

        return false;
    }

    public async Task<CouponViewModel> ApplyCouponFrontend(string couponName)
    {
        if (couponName != null)
        {
            var coupon =
                await context.Coupons.SingleOrDefaultAsync(c => c.Name.ToLower() == couponName.ToLower());
            if (coupon != null)
            {
                return new CouponViewModel()
                {
                    Name =
                        coupon.Name,
                    Percentage = coupon.DiscountPercentage
                };
            }
        }
        return null;
    }

    public async Task<bool> MarkOrderAsPaid(Guid orderId)
    {
        var order = await context.Orders.Where(o => o.Id == orderId).FirstOrDefaultAsync();
        if (order != null)
        {
            order.OrderStatus = "Paid";
            context.Update(order);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> CancelOrder(Guid orderId)
    {
        var order = await context
            .Orders
            .Where(o => o.Id == orderId)
            .FirstOrDefaultAsync();
        if (order != null)
        {
            order.OrderStatus = "Canceled";
            context.Update(order);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}