using LMSystem.Web.Models;

namespace LMSystem.Web.ViewModels;

public class BookListViewModel
{
    public IEnumerable<Book> Books { get; set; } = new List<Book>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
    
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public int OutOfStockBooks { get; set; }
}
