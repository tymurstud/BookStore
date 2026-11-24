using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MeasureTimeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = Stopwatch.StartNew();
        context.HttpContext.Response.OnStarting(() =>
        {
            context.HttpContext.Response.Headers["X-Action-Ms"] = sw.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });
        await next();
    }
}
