using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSystem.Web.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(13)]
    public string ISBN { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int AuthorId { get; set; }
    
    [ForeignKey("AuthorId")]
    public virtual Author? Author { get; set; }

    [Required]
    public int PublisherId { get; set; }
    
    [ForeignKey("PublisherId")]
    public virtual Publisher? Publisher { get; set; }

    [Required]
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    [Required]
    [Range(0, 1000)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, 1000)]
    public int AvailableQuantity { get; set; }

    [StringLength(50)]
    public string? ShelfLocation { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(2000)]
    public string? ImageUrl { get; set; }

    [StringLength(50)]
    public string? Edition { get; set; }

    public int? PublishedYear { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
