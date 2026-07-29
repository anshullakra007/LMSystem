using LMSystem.Web.Controllers;
using LMSystem.Web.Data;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LMSystem.Tests.Controllers;

public class BorrowControllerTests
{
    private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private Mock<UserManager<ApplicationUser>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Return_OverdueBook_CalculatesFineCorrectly()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var book = new Book { Title = "Test Book", AvailableQuantity = 0, Quantity = 1 };
            context.Books.Add(book);
            
            var record = new BorrowRecord
            {
                BookId = 1,
                UserId = "user1",
                IssueDate = DateTime.UtcNow.AddDays(-20),
                DueDate = DateTime.UtcNow.AddDays(-5.5), // exactly 5.5 days overdue
                Status = "Issued",
                Book = book
            };
            context.BorrowRecords.Add(record);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var controller = new BorrowController(context, GetMockUserManager().Object);
            controller.TempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>().Object;

            // Act
            var result = await controller.Return(1);

            // Assert
            var record = await context.BorrowRecords.FindAsync(1);
            var book = await context.Books.FindAsync(1);
            
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Returned", record.Status);
            Assert.NotNull(record.ReturnDate);
            Assert.Equal(1, book.AvailableQuantity); // Book returned
            Assert.Equal(60, record.FineAmount); // 6 days * 10 units
        }
    }

    [Fact]
    public async Task Return_OnTimeBook_NoFineCalculated()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using (var context = new ApplicationDbContext(options))
        {
            var book = new Book { Title = "Test Book", AvailableQuantity = 0, Quantity = 1 };
            context.Books.Add(book);
            
            var record = new BorrowRecord
            {
                BookId = 1,
                UserId = "user1",
                IssueDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(9), // Not due yet
                Status = "Issued",
                Book = book
            };
            context.BorrowRecords.Add(record);
            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var controller = new BorrowController(context, GetMockUserManager().Object);
            controller.TempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>().Object;

            // Act
            var result = await controller.Return(1);

            // Assert
            var record = await context.BorrowRecords.FindAsync(1);
            
            Assert.Equal("Returned", record.Status);
            Assert.Equal(0, record.FineAmount); // No fine
        }
    }
}
