using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorsController : Controller
{
    public IActionResult Index() => View(AuthorStore.Authors);

    [HttpGet]
    public IActionResult Create() => View(new Author());

    [HttpPost]
    public IActionResult Create(Author author)
    {
        if (!ModelState.IsValid) return View(author);
        author.Id = AuthorStore.Authors.Count > 0 ? AuthorStore.Authors.Max(a => a.Id) + 1 : 1;
        AuthorStore.Authors.Add(author);
        TempData["Success"] = $"Dodano autora \"{author.Name}\".";
        return RedirectToAction(nameof(Index));
    }
}
