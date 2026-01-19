using Pizza.Data;
using Pizza.Models;
using Pizza.Services.Contracts;

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
                ProductId = pizzaId,
                Quantity = 1
            };
            await context.AddAsync(cartItem);
            await context.SaveChangesAsync();
            return true;
        }
        return false;
    }
}