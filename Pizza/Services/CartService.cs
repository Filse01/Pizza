using Microsoft.EntityFrameworkCore;
using Pizza.Data;
using Pizza.Models;
using Pizza.Services.Contracts;
using Pizza.ViewModels;

namespace Pizza.Services;

public class CartService : ICartService
{
    private readonly PizzaDbContext context;

    public CartService(PizzaDbContext context)
    {
        this.context = context;
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
                };
                return model;
            }
        }
        return null;
    }

    public async Task<bool> CreateOrder(AddOrderPageViewModel model, string userId)
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
                Address = model.Order.Address
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

            order.Pizzas = orderItems;
            await context.AddAsync(order);
            await context.SaveChangesAsync();
            context.RemoveRange(model.Cart.CartItems);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}