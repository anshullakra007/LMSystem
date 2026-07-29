using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LMSystem.Web.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? ProfilePicture { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
