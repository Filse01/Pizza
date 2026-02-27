using Pizza.ViewModels;

namespace Pizza.Services.Contracts;

public interface ICartService
{
    Task<bool> AddToCart(string userId, Guid pizzaId);
    Task<bool> RemoveFromCart(string userId, Guid pizzaId);
    Task<CartViewModel> GetCart(string userId);
    Task<bool> CreateOrder(AddOrderPageViewModel order, string userId);
    Task<bool> CreateMail(string userId);
}