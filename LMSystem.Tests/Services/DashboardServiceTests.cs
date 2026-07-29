using FluentAssertions;
using LMSystem.Web.Data;
using LMSystem.Web.Models;
using LMSystem.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LMSystem.Tests.Services;

public class DashboardServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DashboardServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetTotalBooksAsync_ReturnsSumOfQuantities()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        context.Books.Add(new Book { Id = 1, Title = "Book 1", Quantity = 5, ISBN = "1", AuthorId = 1, CategoryId = 1, PublisherId = 1 });
        context.Books.Add(new Book { Id = 2, Title = "Book 2", Quantity = 3, ISBN = "2", AuthorId = 1, CategoryId = 1, PublisherId = 1 });
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var result = await service.GetTotalBooksAsync();

        // Assert
        result.Should().Be(8);
    }
}
