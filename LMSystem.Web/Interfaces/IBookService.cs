using LMSystem.Web.Models;
using LMSystem.Web.ViewModels;

namespace LMSystem.Web.Interfaces;

public interface IBookService
{
    Task<BookListViewModel> GetPagedBooksAsync(int page, int pageSize, string? searchTerm);
    Task<Book?> GetBookByIdAsync(int id);
    Task CreateBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Publisher>> GetAllPublishersAsync();
}
