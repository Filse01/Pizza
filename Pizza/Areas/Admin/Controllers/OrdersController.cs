using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.Services.Contracts;

namespace Pizza.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private IAdminService _adminService;

    public OrdersController(IAdminService adminService)
    {
        this._adminService = adminService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var orders = await _adminService.GetAllOrders();
        return View(orders);
    }
}