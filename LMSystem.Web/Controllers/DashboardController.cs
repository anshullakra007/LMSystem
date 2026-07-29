using LMSystem.Web.Models;
using LMSystem.Web.Services;
using LMSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalBooks = await _dashboardService.GetTotalBooksAsync(),
            TotalStudents = await _dashboardService.GetTotalStudentsAsync(),
            TotalLibrarians = await _dashboardService.GetTotalLibrariansAsync(),
            TotalBorrowed = await _dashboardService.GetBooksIssuedAsync(),
            TotalReturned = await _dashboardService.GetBooksReturnedAsync(),
            OverdueBooks = await _dashboardService.GetOverdueBooksAsync(),
            FineCollection = await _dashboardService.GetTotalFinesCollectedAsync(),
            RecentBorrowings = await _dashboardService.GetRecentBorrowingsAsync(),
            RecentlyAddedBooks = await _dashboardService.GetRecentlyAddedBooksAsync()
        };

        return View(model);
    }
}
