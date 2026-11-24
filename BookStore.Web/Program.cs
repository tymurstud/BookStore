using System.Globalization;
using BookStore.Web.Data;
using BookStore.Web.Filters;
using BookStore.Web.Middleware;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddSingleton<RequestStats>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<LogActionFilter2>();
builder.Services.AddScoped<GlobalExceptionFilter>();
builder.Services.AddControllersWithViews(o => o.Filters.Add<GlobalExceptionFilter>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<RequestTimingMiddleware>();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = new[] { new CultureInfo("en-US") },
    SupportedUICultures = new[] { new CultureInfo("en-US") }
});

app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute("books-by-year", "books/year/{year:int:min(1450)}",
    new { controller = "Book", action = "ByYear" });
app.MapControllerRoute("books-by-isbn", "books/isbn/{isbn:regex(^\\d{{13}}$)}",
    new { controller = "Book", action = "ByIsbn" });
app.MapControllerRoute("archive", "archive/{year:int}/{month:int:range(1,12)}",
    new { controller = "Book", action = "Archive" });
app.MapControllerRoute("docs", "docs/{*path}",
    new { controller = "Home", action = "Docs" });
app.MapControllerRoute("admin-area", "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
