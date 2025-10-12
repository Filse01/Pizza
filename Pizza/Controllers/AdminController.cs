using Microsoft.AspNetCore.Mvc;

namespace Pizza.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}