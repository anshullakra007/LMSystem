namespace LMSystem.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalStudents { get; set; }
    public int TotalLibrarians { get; set; }
    public int TotalBorrowed { get; set; }
    public int TotalReturned { get; set; }
    public int OverdueBooks { get; set; }
    public decimal FineCollection { get; set; }
    
    public IEnumerable<dynamic> RecentBorrowings { get; set; } = new List<dynamic>();
    public IEnumerable<dynamic> RecentlyAddedBooks { get; set; } = new List<dynamic>();
}
