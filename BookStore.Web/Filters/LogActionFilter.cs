using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStore.Web.Filters;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _log;

    public LogActionFilter(ILogger<LogActionFilter> log) => _log = log;

    public void OnActionExecuting(ActionExecutingContext c) =>
        _log.LogInformation("START {Action}({Args})",
            c.ActionDescriptor.DisplayName,
            string.Join(", ", c.ActionArguments.Select(a => $"{a.Key}={a.Value}")));

    public void OnActionExecuted(ActionExecutedContext c) =>
        _log.LogInformation("KONIEC {Action}", c.ActionDescriptor.DisplayName);
}

public class LogActionFilter2 : IActionFilter
{
    private readonly ILogger<LogActionFilter2> _log;

    public LogActionFilter2(ILogger<LogActionFilter2> log) => _log = log;

    public void OnActionExecuting(ActionExecutingContext c) =>
        _log.LogInformation("FILTER2 START {Action}", c.ActionDescriptor.DisplayName);

    public void OnActionExecuted(ActionExecutedContext c) =>
        _log.LogInformation("FILTER2 KONIEC {Action}", c.ActionDescriptor.DisplayName);
}
