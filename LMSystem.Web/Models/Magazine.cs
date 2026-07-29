using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSystem.Web.Models;

public class Magazine
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int PublisherId { get; set; }

    [ForeignKey("PublisherId")]
    public virtual Publisher? Publisher { get; set; }

    public DateTime IssueDate { get; set; }

    [Required]
    [Range(0, 1000)]
    public int Quantity { get; set; }
}
