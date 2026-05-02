using Pizza.ViewModels;

namespace Pizza.Services.Contracts;

public interface ICartService
{
    Task<bool> AddToCart(string userId, Guid pizzaId);
    Task<bool> RemoveFromCart(string userId, Guid pizzaId);
    Task<CartViewModel> GetCart(string userId);
    Task<Guid> CreatePendingOrder(AddOrderPageViewModel order, string userId);
    Task<bool> CreateMail(string userId);
    Task<CouponViewModel> ApplyCouponFrontend(string couponName); 
    Task<bool> MarkOrderAsPaid(Guid orderId);
    Task<bool> CancelOrder(Guid orderId);
}