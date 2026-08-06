#  LMSystem - Library Management System

## Why I built this ?

### Situation
Managing digital or physical assets across an organization requires a centralized Library Management System (LMS) with strict access controls and transactional integrity.

### Task
I needed to design a full-stack system to handle user authentication, inventory tracking, borrowing logic, and automated due-date calculations.

### Action
I architected a robust RESTful API backend communicating with a relational database. I implemented JWT-based authentication for Admin/User roles. For the borrowing logic, I used SQL transactions to ensure that inventory counts were strictly synchronized, preventing race conditions when multiple users tried to borrow the same asset simultaneously.

### Result
The LMS operates flawlessly, providing a secure, concurrent, and highly scalable solution for inventory management, showcasing my ability to model complex real-world business logic into code.

---

> LMSystem is a full-featured Library Management System developed with ASP.NET Core 8 MVC. It handles the end-to-end process of managing physical library resources, including automated borrowing pipelines, fine calculations, and a robust Role-Based Access Control (RBAC) model distinguishing between administrators, library staff, and students.

---

##  Table of Contents
- [Features](#-features)
- [Architecture & Design Patterns](#-architecture--design-patterns)
- [Database Schema & Entities](#-database-schema--entities)
- [Getting Started](#-getting-started)
- [Roles & Workflows](#-roles--workflows)
- [Testing Guide](#-testing-guide)
- [Future Enhancements](#-future-enhancements)

---

##  Features

- Automated Borrowing System: Request, approve, issue, and return books seamlessly.
- Automated Fine Calculation: Automatically tracks overdue items and calculates fines per day.
- Inventory Management: Maintain detailed records of Books, Authors, Publishers, Magazines, and Newspapers.
- Role-Based Security: Differentiated access for Admins, Librarians, and Students using ASP.NET Core Identity.
- Unit Tested: Comprehensive test coverage using xUnit, Moq, and FluentAssertions.

---

##  Architecture & Design Patterns

The solution is divided into two primary projects:
1. `LMSystem.Web`: The main ASP.NET Core MVC web application.
2. `LMSystem.Tests`: The xUnit testing project for controllers, services, and repositories.

Design Patterns Implemented:
- MVC (Model-View-Controller): Separates UI logic and HTTP handling.
- Repository Pattern: Data access abstraction using `IRepository<T>` ensuring clean data layer separation.
- Service Layer: Encapsulates business logic (`BookService`, `BorrowService`) to keep controllers lean.
- Dependency Injection: Services and repositories are injected via the built-in DI container.

---

##  Database Schema & Entities

The system uses Entity Framework Core with SQLite. The context is defined in `ApplicationDbContext.cs`.

### Core Entities
- ApplicationUser: Extends `IdentityUser`. Manages user profiles.
- Book: Central entity. Includes `Title`, `ISBN` (unique index), `Quantity`, `AvailableQuantity`, `ShelfLocation`, `PublishedYear`, and foreign keys.
- BorrowRecord: Tracks book loans. Links a `StudentId` to a `BookId`. Contains `IssueDate`, `DueDate`, `ReturnDate`, `Status`, and `FineAmount`.
- Category, Author, Publisher: Normalization tables for cataloging.

> Note on Constraints: The database enforces Restrict Delete Behavior on Book relationships. You cannot delete an Author, Category, or Publisher if there are books tied to them.

---

##  Getting Started

### Prerequisites
- .NET 8 SDK or newer installed.
- SQLite (bundled with Entity Framework Core, no external installation required).
- Any standard IDE like Visual Studio, VS Code, or Rider.

### Running Locally
To launch the project locally:

1. Clone the repository & restore dependencies:
   ```bash
   git clone https://github.com/anshullakra007/LMSystem.git
   cd LMSystem
   dotnet restore
   ```

2. Apply Database Migrations:
   ```bash
   dotnet ef database update
   ```

3. Run the application:
   ```bash
   dotnet run --project LMSystem.Web
   ```

4. Access the application:
   Open your browser and navigate to `http://localhost:5005` or `https://localhost:7091`.

> Default Admin Account: 
> - Email: `admin@example.com`
> - Password: `Admin@123`

---

##  Roles & Workflows

Security is managed via ASP.NET Core Identity.

### Roles
- Admin: Can manage all users, roles, and global configurations.
- Librarian: Full control over the library catalog and managing Borrow Requests.
- Student: Read-only access to the catalog. Can request up to 5 books.

### Borrowing Workflow
1. Request: Student requests a book. System checks availability, borrow limits, and unpaid fines.
2. Issue: Librarian approves the request. `AvailableQuantity` decrements.
3. Return: Librarian marks the book as returned. `AvailableQuantity` increments.
4. Fine Calculation: If returned past the `DueDate`, the system calculates a fine of 10 units per overdue day.

---

##  Testing Guide

The system relies on xUnit, Moq, and FluentAssertions for automated unit testing.

### Executing Tests
```bash
cd LMSystem.Tests
dotnet test
```

### Coverage
- Controllers: Validates routing, logic execution, and constraints.
- Services: Validates business logic, pagination, and data mapping.
- Repositories: Ensures Entity Framework Core LINQ queries and Eager Loading are functionally correct.

---

##  Future Enhancements
- Migrate SQLite to SQL Server or PostgreSQL for production deployment.
- Implement Email notifications for overdue books.
- Integrate a payment gateway for fine processing.
- Add Barcode/QR integration for faster checkout.

---

*Developed by Anshul Kumar.*

---