using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using LMSystem.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IRepository<Author> _authorRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Publisher> _publisherRepository;

    public BookService(
        IBookRepository bookRepository,
        IRepository<Author> authorRepository,
        IRepository<Category> categoryRepository,
        IRepository<Publisher> publisherRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _publisherRepository = publisherRepository;
    }

    public async Task<BookListViewModel> GetPagedBooksAsync(int page, int pageSize, string? searchTerm)
    {
        var (books, totalCount) = await _bookRepository.GetPagedBooksAsync(page, pageSize, searchTerm);

        var query = _bookRepository.AsQueryable();
        int totalBooks = await query.SumAsync(b => b.Quantity);
        int availableBooks = await query.SumAsync(b => b.AvailableQuantity);
        int borrowedBooks = totalBooks - availableBooks;
        int outOfStockBooks = await query.CountAsync(b => b.AvailableQuantity == 0);

        return new BookListViewModel
        {
            Books = books,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            SearchTerm = searchTerm,
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            BorrowedBooks = borrowedBooks,
            OutOfStockBooks = outOfStockBooks
        };
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task CreateBookAsync(Book book)
    {
        book.AvailableQuantity = book.Quantity;
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        book.UpdatedAt = DateTime.UtcNow;
        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book != null)
        {
            _bookRepository.Remove(book);
            await _bookRepository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await _authorRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
    {
        return await _publisherRepository.GetAllAsync();
    }
}
