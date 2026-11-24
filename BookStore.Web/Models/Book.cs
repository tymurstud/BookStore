using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public string Isbn { get; set; } = "";

    public DateTime? PublicationDate { get; set; }
    public int NumPages { get; set; }
    public decimal Price { get; set; }

    [Required]
    public string Author { get; set; } = "";
}
