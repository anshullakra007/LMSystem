using FluentAssertions;
using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using LMSystem.Web.Services;
using Moq;

namespace LMSystem.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IRepository<Author>> _authorRepositoryMock;
    private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
    private readonly Mock<IRepository<Publisher>> _publisherRepositoryMock;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IRepository<Author>>();
        _categoryRepositoryMock = new Mock<IRepository<Category>>();
        _publisherRepositoryMock = new Mock<IRepository<Publisher>>();

        _bookService = new BookService(
            _bookRepositoryMock.Object,
            _authorRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _publisherRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CreateBookAsync_ShouldSetDefaultsAndSave()
    {
        // Arrange
        var book = new Book
        {
            Title = "New Book",
            Quantity = 10
        };

        // Act
        await _bookService.CreateBookAsync(book);

        // Assert
        book.AvailableQuantity.Should().Be(10);
        book.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        book.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        _bookRepositoryMock.Verify(r => r.AddAsync(book), Times.Once);
        _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPagedBooksAsync_ShouldReturnViewModel()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Title = "Book 1" },
            new Book { Title = "Book 2" }
        };
        
        _bookRepositoryMock.Setup(r => r.GetPagedBooksAsync(1, 10, null))
            .ReturnsAsync((books, 20)); // 20 total records

        // Act
        var result = await _bookService.GetPagedBooksAsync(1, 10, null);

        // Assert
        result.Should().NotBeNull();
        result.Books.Should().HaveCount(2);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(2); // 20 / 10 = 2
        result.SearchTerm.Should().BeNull();
    }
}
