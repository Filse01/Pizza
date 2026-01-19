namespace Pizza.Services.Contracts;

public interface ICartService
{
    Task<bool> AddToCart(string userId, Guid pizzaId);
}