using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Imie i nazwisko")]
    public string Name { get; set; } = "";
}
