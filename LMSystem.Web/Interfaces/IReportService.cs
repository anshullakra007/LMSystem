using LMSystem.Web.ViewModels;

namespace LMSystem.Web.Interfaces;

public interface IReportService
{
    Task<ReportsViewModel> GetFullReportAsync();
}
