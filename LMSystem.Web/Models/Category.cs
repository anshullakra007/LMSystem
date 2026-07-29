using System.ComponentModel.DataAnnotations;

namespace LMSystem.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation property
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
