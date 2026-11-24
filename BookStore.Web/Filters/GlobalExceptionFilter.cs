using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BookStore.Web.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _log;
    private readonly IHostEnvironment _env;
    private readonly IModelMetadataProvider _meta;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> log, IHostEnvironment env, IModelMetadataProvider meta)
    {
        _log = log;
        _env = env;
        _meta = meta;
    }

    public void OnException(ExceptionContext ctx)
    {
        _log.LogError(ctx.Exception, "Nieobsluzony wyjatek w {Action}", ctx.ActionDescriptor.DisplayName);
        var detail = _env.IsDevelopment() ? ctx.Exception.Message : null;

        if (ctx.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Result = new ObjectResult(new ProblemDetails
            {
                Status = 500,
                Title = "Blad serwera",
                Detail = detail
            }) { StatusCode = 500 };
        }
        else
        {
            var vm = new ErrorViewModel { RequestId = ctx.HttpContext.TraceIdentifier, Message = detail };
            ctx.Result = new ViewResult
            {
                ViewName = "Error",
                StatusCode = 500,
                ViewData = new ViewDataDictionary(_meta, ctx.ModelState) { Model = vm }
            };
        }
        ctx.ExceptionHandled = true;
    }
}
