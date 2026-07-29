namespace LMSystem.Web.ViewModels;

public class ReportsViewModel
{
    // Overall Stats
    public int TotalBooks { get; set; }
    public int TotalBorrowed { get; set; }
    public int OverdueBooks { get; set; }
    public decimal FineCollection { get; set; }

    // Chart Data
    public Dictionary<string, int> BooksByCategory { get; set; } = new();
    public Dictionary<string, int> BorrowingsByMonth { get; set; } = new();

    // Data Tables
    public IEnumerable<dynamic> OverdueRecords { get; set; } = new List<dynamic>();
    public IEnumerable<dynamic> MostBorrowedBooks { get; set; } = new List<dynamic>();
    public IEnumerable<dynamic> RecentFines { get; set; } = new List<dynamic>();
}
