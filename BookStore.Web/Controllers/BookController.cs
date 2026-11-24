using System.Text;
using BookStore.Web.Filters;
using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

[ServiceFilter(typeof(LogActionFilter))]
[MeasureTime]
public class BookController : Controller
{
    private readonly IBookService _books;

    public BookController(IBookService books) => _books = books;

    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    [HttpGet("Book/Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? search = null, string? sort = null, string? dir = null)
    {
        var result = await _books.GetPageAsync(page, pageSize, search, sort, dir);
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;
        return View(result);
    }

    public async Task<IActionResult> ByYear(int year)
    {
        var result = await _books.GetPageAsync(1, 100, null, "title", "asc");
        var books = result.Items.Where(b => b.PublicationDate?.Year == year).ToList();
        ViewBag.Filter = $"Rok: {year}";
        return View("Filtered", books);
    }

    public async Task<IActionResult> ByIsbn(string isbn)
    {
        var normalized = isbn.Replace("-", "").Replace(" ", "");
        var result = await _books.GetPageAsync(1, 100, normalized);
        var book = result.Items.FirstOrDefault();
        if (book == null) return NotFound();
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    public async Task<IActionResult> Archive(int year, int month)
    {
        var result = await _books.GetPageAsync(1, 100, null, "title", "asc");
        var books = result.Items.Where(b => b.PublicationDate?.Year == year).ToList();
        ViewBag.Filter = $"Archiwum: {year}/{month:D2}";
        return View("Filtered", books);
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
    public async Task<IActionResult> Create(BookFormViewModel vm)
    {
        if (await _books.IsbnExistsAsync(vm.Isbn))
        {
            ModelState.AddModelError(nameof(vm.Isbn), "Ksiazka o podanym numerze ISBN juz istnieje");
        }

        if (!ModelState.IsValid) return View(vm);

        var book = new Book
        {
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author,
            Publisher = vm.Publisher
        };

        await _books.AddAsync(book);
        TempData["Success"] = $"Dodano ksiazke \"{vm.Title}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null) return NotFound();

        var vm = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationDate = book.PublicationDate,
            NumPages = book.NumPages,
            Price = book.Price,
            Author = book.Author,
            Publisher = book.Publisher
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BookFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (await _books.IsbnExistsAsync(vm.Isbn, id))
        {
            ModelState.AddModelError(nameof(vm.Isbn), "Ksiazka o podanym numerze ISBN juz istnieje");
        }

        if (!ModelState.IsValid) return View(vm);

        var book = new Book
        {
            Id = id,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author,
            Publisher = vm.Publisher
        };

        var ok = await _books.UpdateAsync(book);
        if (!ok) return NotFound();

        TempData["Success"] = $"Zaktualizowano ksiazke \"{vm.Title}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _books.GetByIdAsync(id);
        var title = book?.Title ?? "";
        var ok = await _books.DeleteAsync(id);
        if (!ok) return NotFound();

        TempData["Success"] = $"Usunieto ksiazke \"{title}\".";
        return RedirectToAction(nameof(Index));
    }
}
