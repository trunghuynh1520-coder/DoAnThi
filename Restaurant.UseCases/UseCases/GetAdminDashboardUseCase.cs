using Restaurant.UseCases.Models;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

public sealed class GetAdminDashboardUseCase(IAdminReportRepository reportRepository)
{
    public Task<AdminDashboardReport> ExecuteAsync(DateTime now) => reportRepository.GetDashboardAsync(now);
}

public sealed class GetInvoiceHistoryUseCase(IAdminReportRepository reportRepository)
{
    public Task<IEnumerable<InvoiceHistoryItem>> ExecuteAsync(DateTime from, DateTime toExclusive) =>
        reportRepository.GetInvoiceHistoryAsync(from, toExclusive);

    public Task<IEnumerable<InvoiceLineDetail>> GetLinesAsync(int orderId) =>
        reportRepository.GetInvoiceLinesAsync(orderId);
}
