using System.Text;
using BookStore.Web.Filters;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

[ServiceFilter(typeof(LogActionFilter))]
[MeasureTime]
public class BookController : Controller
{
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    [HttpGet("Book/Details/{id:int}")]
    public IActionResult Details(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    public IActionResult Index() => View(BookStoreMemory.Books);

    public IActionResult ByYear(int year)
    {
        var books = BookStoreMemory.Books
            .Where(b => b.PublicationDate?.Year == year)
            .ToList();
        ViewBag.Filter = $"Rok: {year}";
        return View("Index", books);
    }

    public IActionResult ByIsbn(string isbn)
    {
        var normalized = isbn.Replace("-", "").Replace(" ", "");
        var book = BookStoreMemory.Books
            .FirstOrDefault(b => b.Isbn.Replace("-", "").Replace(" ", "") == normalized);
        if (book == null) return NotFound();
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    public IActionResult Archive(int year, int month)
    {
        var books = BookStoreMemory.Books
            .Where(b => b.PublicationDate?.Year == year)
            .ToList();
        ViewBag.Filter = $"Archiwum: {year}/{month:D2}";
        return View("Index", books);
    }

    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1) =>
        Content(string.Join("\n", Enumerable.Repeat($"Czesc, {name}!", Math.Clamp(count, 1, 20))));

    public IActionResult Square(int id) => Content($"{id}^2 = {id * id}");

    [HttpPost]
    public IActionResult Import([FromBody] BookDto dto) =>
        Json(new { received = dto.Title, dto.Isbn, dto.Price });

    public IActionResult Inspect()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Method: {Request.Method}");
        sb.AppendLine($"Path:   {Request.Path}{Request.QueryString}");
        foreach (var h in Request.Headers) sb.AppendLine($"{h.Key}: {h.Value}");
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult Sum(int a, int b)
    {
        if (!ModelState.IsValid)
            return BadRequest("Niepoprawne parametry");
        return Content((a + b).ToString());
    }

    public IActionResult UserAgent() =>
        Content(Request.Headers.UserAgent.ToString());

    public IActionResult Health() =>
        Json(new { status = "ok", time = DateTime.Now });

    public IActionResult Boom() =>
        throw new InvalidOperationException("Celowy wyjatek");

    [HttpGet]
    public IActionResult Create() => View(new BookFormViewModel());

    [HttpPost]
    public IActionResult Create(BookFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        BookStoreMemory.Books.Add(new Book
        {
            Id = BookStoreMemory.Books.Max(b => b.Id) + 1,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        });

        TempData["Success"] = $"Dodano ksiazke \"{vm.Title}\".";
        return RedirectToAction(nameof(Index));
    }
}
