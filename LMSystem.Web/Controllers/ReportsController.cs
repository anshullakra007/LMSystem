using LMSystem.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _reportService.GetFullReportAsync();
        return View(model);
    }
}
