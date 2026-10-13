using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class BookController : Controller
{
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    public IActionResult Details(int id)
    {
        ViewBag.Id = id;
        return View();
    }
}