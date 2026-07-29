using System.ComponentModel.DataAnnotations;

namespace LMSystem.Web.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }

    // Navigation property
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
