using Restaurant.UseCases.Models;

namespace Restaurant.UseCases.PluginInterfaces;

public interface IAdminReportRepository
{
    Task<AdminDashboardReport> GetDashboardAsync(DateTime now);
    Task<IEnumerable<InvoiceHistoryItem>> GetInvoiceHistoryAsync(DateTime from, DateTime toExclusive);
    Task<IEnumerable<InvoiceLineDetail>> GetInvoiceLinesAsync(int orderId);
}
