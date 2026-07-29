using System.ComponentModel.DataAnnotations;

namespace LMSystem.Web.Models;

public class Newspaper
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Required]
    [Range(0, 1000)]
    public int Quantity { get; set; }
}
