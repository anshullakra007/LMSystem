using LMSystem.Web.Models;

namespace LMSystem.Web.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetAllWithDetailsAsync();
    Task<Book?> GetByIdWithDetailsAsync(int id);
    Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedBooksAsync(int page, int pageSize, string? searchTerm);
}
