using System.Diagnostics;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IActionDescriptorCollectionProvider _provider;

    public HomeController(IActionDescriptorCollectionProvider provider)
        => _provider = provider;

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    public IActionResult Docs(string? path)
    {
        ViewBag.Path = path;
        return View();
    }

    public IActionResult Routes()
    {
        var routes = _provider.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .OrderBy(a => a.ControllerName)
            .ThenBy(a => a.ActionName)
            .Select(a => new RouteEntry(
                Template: a.AttributeRouteInfo?.Template ?? $"/{a.ControllerName}/{a.ActionName}",
                Action: $"{a.ControllerName}.{a.ActionName}",
                Methods: string.Join(", ", a.ActionConstraints?
                    .OfType<Microsoft.AspNetCore.Mvc.ActionConstraints.HttpMethodActionConstraint>()
                    .SelectMany(c => c.HttpMethods)
                    .Distinct() ?? Enumerable.Empty<string>())
            ))
            .ToList();
        return View(routes);
    }

    public IActionResult HttpStatus(int code)
    {
        Response.StatusCode = code;
        return View(code);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
