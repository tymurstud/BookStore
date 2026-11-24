namespace BookStore.Web.Models;

public static class AuthorStore
{
    public static readonly List<Author> Authors = new()
    {
        new() { Id = 1, Name = "Stanislaw Lem" },
        new() { Id = 2, Name = "Boleslaw Prus" },
        new() { Id = 3, Name = "Adam Mickiewicz" }
    };
}
