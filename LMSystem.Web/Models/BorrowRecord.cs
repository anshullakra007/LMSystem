using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMSystem.Web.Models;

public class BorrowRecord
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [ForeignKey("BookId")]
    public virtual Book? Book { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Requested"; // Requested, Issued, Returned

    [Column(TypeName = "decimal(18,2)")]
    public decimal FineAmount { get; set; }
}
