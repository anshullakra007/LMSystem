using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBookImageAndSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "u1v2w3x4-y5z6-a7b8-c9d0-e1f2g3h4i5j6",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c220446f-5261-4853-a069-d7fc5a4869b4", new DateTime(2026, 7, 26, 6, 34, 48, 977, DateTimeKind.Utc).AddTicks(6470), "AQAAAAIAAYagAAAAEABcPoFHgRIZM9orekWqlmoKI7jJR3k5Risu+8r9ecxasw5FATNTsKvwTL5faueM4A==", "b237810a-7919-496c-b3da-5a3322885820" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "u9v8w7x6-y5z4-a3b2-c1d0-e9f8g7h6i5j4", 0, "a485944c-1fb2-407f-a6d9-a6f23eb6ea03", new DateTime(2026, 7, 26, 6, 34, 49, 15, DateTimeKind.Utc).AddTicks(1840), "student@example.com", true, false, null, "Demo Student", "STUDENT@EXAMPLE.COM", "STUDENT@EXAMPLE.COM", "AQAAAAIAAYagAAAAEBcKbJZhQT5J9l3BLzmVz/PUMeSci9MnkIldWdd7gPl95tHiUXLhTlJvzFjGIJW2tA==", null, false, null, "6f29bad4-14a0-47b9-9977-a11345815cfb", false, "student@example.com" });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "Name" },
                values: new object[,]
                {
                    { 1, "American novelist, essayist, and short story writer.", "F. Scott Fitzgerald" },
                    { 2, "English novelist, essayist, journalist and critic.", "George Orwell" },
                    { 3, "British author and philanthropist.", "J.K. Rowling" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { 1, "New York, USA", "Scribner" },
                    { 2, "London, UK", "Secker & Warburg" },
                    { 3, "London, UK", "Bloomsbury" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c3d4e5f6-g7h8-i9j0-k1l2-m3n4o5p6q7r8", "u9v8w7x6-y5z4-a3b2-c1d0-e9f8g7h6i5j4" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "AvailableQuantity", "CategoryId", "CreatedAt", "Description", "Edition", "ISBN", "ImageUrl", "Language", "PublishedYear", "PublisherId", "Quantity", "ShelfLocation", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, 10, 1, new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6270), "A novel set in the Jazz Age that tells the story of Jay Gatsby.", null, "9780743273565", "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=300&auto=format&fit=crop", "English", 1925, 1, 10, "A1-Shelf1", "The Great Gatsby", new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6270) },
                    { 2, 2, 5, 1, new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6270), "A dystopian social science fiction novel and cautionary tale.", null, "9780451524935", "https://images.unsplash.com/photo-1512820790803-83ca734da794?q=80&w=300&auto=format&fit=crop", "English", 1949, 2, 5, "B2-Shelf2", "1984", new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6270) },
                    { 3, 3, 20, 1, new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6280), "The first novel in the Harry Potter series.", null, "9780590353427", "https://images.unsplash.com/photo-1629196914214-4113e6d8a39a?q=80&w=300&auto=format&fit=crop", "English", 1997, 3, 20, "C3-Shelf3", "Harry Potter and the Sorcerer's Stone", new DateTime(2026, 7, 26, 6, 34, 49, 52, DateTimeKind.Utc).AddTicks(6280) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c3d4e5f6-g7h8-i9j0-k1l2-m3n4o5p6q7r8", "u9v8w7x6-y5z4-a3b2-c1d0-e9f8g7h6i5j4" });

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "u9v8w7x6-y5z4-a3b2-c1d0-e9f8g7h6i5j4");

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "u1v2w3x4-y5z6-a7b8-c9d0-e1f2g3h4i5j6",
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3a52c7d2-82e3-4729-810c-8db414daa340", new DateTime(2026, 7, 26, 5, 7, 19, 382, DateTimeKind.Utc).AddTicks(4840), "AQAAAAIAAYagAAAAEE2bnM44iGq0c8M0ipIDg2l6dXjFLEGMeFmZRiDLoIQQI04wUJRyVlMxAFhxX636UQ==", "852e2eaf-666e-4b19-bed3-b8e4ecae9ee4" });
        }
    }
}
