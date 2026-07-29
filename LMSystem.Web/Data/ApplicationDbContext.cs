using LMSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<BorrowRecord> BorrowRecords { get; set; }
    public DbSet<Magazine> Magazines { get; set; }
    public DbSet<Newspaper> Newspapers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Indexes
        builder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        // Restrict delete on relationships
        builder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BorrowRecord>()
            .HasOne(br => br.Book)
            .WithMany(b => b.BorrowRecords)
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Roles
        var adminRoleId = "a1b2c3d4-e5f6-7g8h-9i0j-k1l2m3n4o5p6";
        var librarianRoleId = "b2c3d4e5-f6g7-h8i9-j0k1-l2m3n4o5p6q7";
        var studentRoleId = "c3d4e5f6-g7h8-i9j0-k1l2-m3n4o5p6q7r8";

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = librarianRoleId, Name = "Librarian", NormalizedName = "LIBRARIAN" },
            new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" }
        );

        // Seed Admin User
        var adminUserId = "u1v2w3x4-y5z6-a7b8-c9d0-e1f2g3h4i5j6";
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                Name = "System Administrator",
                PasswordHash = hasher.HashPassword(null!, "Admin@123")
            }
        );

        // Assign Admin Role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            }
        );

        // Seed Student User
        var studentUserId = "u9v8w7x6-y5z4-a3b2-c1d0-e9f8g7h6i5j4";
        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = studentUserId,
                UserName = "student@example.com",
                NormalizedUserName = "STUDENT@EXAMPLE.COM",
                Email = "student@example.com",
                NormalizedEmail = "STUDENT@EXAMPLE.COM",
                EmailConfirmed = true,
                Name = "Demo Student",
                PasswordHash = hasher.HashPassword(null!, "Student@123")
            }
        );

        // Assign Student Role
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = studentRoleId,
                UserId = studentUserId
            }
        );
        
        // Additional seed data for Categories
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Non-Fiction" },
            new Category { Id = 3, Name = "Science" },
            new Category { Id = 4, Name = "Technology" }
        );

        // Seed Authors
        builder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "F. Scott Fitzgerald", Biography = "American novelist, essayist, and short story writer." },
            new Author { Id = 2, Name = "George Orwell", Biography = "English novelist, essayist, journalist and critic." },
            new Author { Id = 3, Name = "J.K. Rowling", Biography = "British author and philanthropist." }
        );

        // Seed Publishers
        builder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Scribner", Address = "New York, USA" },
            new Publisher { Id = 2, Name = "Secker & Warburg", Address = "London, UK" },
            new Publisher { Id = 3, Name = "Bloomsbury", Address = "London, UK" }
        );

        // Seed Books
        builder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "The Great Gatsby",
                ISBN = "9780743273565",
                Description = "A novel set in the Jazz Age that tells the story of Jay Gatsby.",
                AuthorId = 1,
                PublisherId = 1,
                CategoryId = 1,
                Quantity = 10,
                AvailableQuantity = 10,
                ShelfLocation = "A1-Shelf1",
                Language = "English",
                PublishedYear = 1925,
                ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=300&auto=format&fit=crop"
            },
            new Book
            {
                Id = 2,
                Title = "1984",
                ISBN = "9780451524935",
                Description = "A dystopian social science fiction novel and cautionary tale.",
                AuthorId = 2,
                PublisherId = 2,
                CategoryId = 1,
                Quantity = 5,
                AvailableQuantity = 5,
                ShelfLocation = "B2-Shelf2",
                Language = "English",
                PublishedYear = 1949,
                ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?q=80&w=300&auto=format&fit=crop"
            },
            new Book
            {
                Id = 3,
                Title = "Harry Potter and the Sorcerer's Stone",
                ISBN = "9780590353427",
                Description = "The first novel in the Harry Potter series.",
                AuthorId = 3,
                PublisherId = 3,
                CategoryId = 1,
                Quantity = 20,
                AvailableQuantity = 20,
                ShelfLocation = "C3-Shelf3",
                Language = "English",
                PublishedYear = 1997,
                ImageUrl = "https://images.unsplash.com/photo-1629196914214-4113e6d8a39a?q=80&w=300&auto=format&fit=crop"
            }
        );
    }
}
