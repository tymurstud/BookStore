using BookStore.Web.Middleware;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RequestStats _stats;

    public DashboardController(RequestStats stats) => _stats = stats;

    public IActionResult Index()
    {
        ViewBag.Total = _stats.Total;
        ViewBag.Top5 = _stats.Top5();
        return View();
    }
}
