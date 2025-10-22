using Microsoft.AspNetCore.Mvc;

namespace Pizza.Areas.Admin.Controllers;
[Area("Admin")]
public class Home : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
}