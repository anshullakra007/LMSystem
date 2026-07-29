using FluentAssertions;
using LMSystem.Web.Data;
using LMSystem.Web.Models;
using LMSystem.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Tests.Repositories;

public class BookRepositoryTests
{
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnBookWithRelations()
    {
        // Arrange
        using var context = await GetDbContext();
        var author = new Author { Name = "Test Author" };
        var category = new Category { Name = "Test Category" };
        var publisher = new Publisher { Name = "Test Publisher" };
        
        await context.Authors.AddAsync(author);
        await context.Categories.AddAsync(category);
        await context.Publishers.AddAsync(publisher);
        await context.SaveChangesAsync();

        var book = new Book
        {
            Title = "Test Book",
            ISBN = "1234567890123",
            AuthorId = author.Id,
            CategoryId = category.Id,
            PublisherId = publisher.Id,
            Quantity = 5,
            AvailableQuantity = 5
        };
        await context.Books.AddAsync(book);
        await context.SaveChangesAsync();

        var repository = new BookRepository(context);

        // Act
        var result = await repository.GetByIdWithDetailsAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Book");
        result.Author.Should().NotBeNull();
        result.Author!.Name.Should().Be("Test Author");
        result.Category.Should().NotBeNull();
        result.Category!.Name.Should().Be("Test Category");
        result.Publisher.Should().NotBeNull();
        result.Publisher!.Name.Should().Be("Test Publisher");
    }

    [Fact]
    public async Task GetPagedBooksAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        using var context = await GetDbContext();
        var author = new Author { Name = "Test Author" };
        var category = new Category { Name = "Test Category" };
        var publisher = new Publisher { Name = "Test Publisher" };
        await context.AddRangeAsync(author, category, publisher);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 15; i++)
        {
            await context.Books.AddAsync(new Book
            {
                Title = $"Book {i:D2}",
                ISBN = $"ISBN{i:D2}",
                AuthorId = author.Id,
                CategoryId = category.Id,
                PublisherId = publisher.Id,
                Quantity = 1,
                AvailableQuantity = 1
            });
        }
        await context.SaveChangesAsync();

        var repository = new BookRepository(context);

        // Act
        var (books, totalCount) = await repository.GetPagedBooksAsync(page: 2, pageSize: 10, searchTerm: null);

        // Assert
        totalCount.Should().Be(18); // 15 added + 3 seeded
        books.Should().HaveCount(8); // Page 2 has the remaining 8
    }
}
