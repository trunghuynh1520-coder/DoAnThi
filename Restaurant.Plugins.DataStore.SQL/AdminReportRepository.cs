using Dapper;
using Microsoft.Data.SqlClient;
using Restaurant.UseCases.Models;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.Plugins.DataStore.SQL;

public sealed class AdminReportRepository(string connectionString) : IAdminReportRepository
{
    public async Task<AdminDashboardReport> GetDashboardAsync(DateTime now)
    {
        var today = now.Date;
        var tomorrow = today.AddDays(1);
        var thirtyDaysAgo = today.AddDays(-29);
        using var connection = new SqlConnection(connectionString);

        const string summarySql = @"
SELECT COUNT(*) AS TodayInvoiceCount,
       COALESCE(SUM(TotalAmount), 0) AS TodayRevenue,
       COALESCE(AVG(NULLIF(TotalAmount, 0)), 0) AS AverageInvoice
FROM dbo.TableOrder
WHERE IsClosed = 1 AND COALESCE(ClosedAt, OrderDate) >= @today AND COALESCE(ClosedAt, OrderDate) < @tomorrow;

SELECT COUNT(*) AS TodayReservationCount, COALESCE(SUM(GuestCount), 0) AS TodayReservedGuests
     , COALESCE(SUM(CASE WHEN Status = 'Arrived' THEN 1 ELSE 0 END), 0) AS TodayArrivedCount
     , COALESCE(SUM(CASE WHEN Status = 'Arrived' THEN GuestCount ELSE 0 END), 0) AS TodayArrivedGuests
FROM dbo.Reservation WHERE ReservationTime >= @today AND ReservationTime < @tomorrow AND Status IN ('Confirmed', 'Arrived');

SELECT COUNT(*) AS TotalTables, COALESCE(SUM(CASE WHEN IsOccupied = 1 THEN 1 ELSE 0 END), 0) AS OccupiedTables
FROM dbo.DiningTable;

SELECT COUNT(*) AS DistinctVisitedCustomers30Days,
       COALESCE(SUM(CASE WHEN VisitCount > 1 THEN 1 ELSE 0 END), 0) AS ReturningVisitedCustomers30Days
FROM (
    SELECT PhoneNumber, COUNT(*) AS VisitCount
    FROM dbo.Reservation
    WHERE ReservationTime >= @thirtyDaysAgo AND ReservationTime < @tomorrow AND Status = 'Arrived'
    GROUP BY PhoneNumber
) customers;

SELECT TOP (1) DATEPART(HOUR, ReservationTime) AS PeakReservationHour30Days
FROM dbo.Reservation
WHERE ReservationTime >= @thirtyDaysAgo AND ReservationTime < @tomorrow AND Status = 'Arrived'
GROUP BY DATEPART(HOUR, ReservationTime)
ORDER BY SUM(GuestCount) DESC, DATEPART(HOUR, ReservationTime);";

        using var results = await connection.QueryMultipleAsync(summarySql, new { today, tomorrow, thirtyDaysAgo });
        var report = await results.ReadSingleAsync<AdminDashboardReport>();
        var reservations = await results.ReadSingleAsync<AdminDashboardReport>();
        report.TodayReservationCount = reservations.TodayReservationCount;
        report.TodayReservedGuests = reservations.TodayReservedGuests;
        report.TodayArrivedCount = reservations.TodayArrivedCount;
        report.TodayArrivedGuests = reservations.TodayArrivedGuests;
        var tables = await results.ReadSingleAsync<AdminDashboardReport>();
        report.TotalTables = tables.TotalTables;
        report.OccupiedTables = tables.OccupiedTables;
        var customers = await results.ReadSingleAsync<AdminDashboardReport>();
        report.DistinctVisitedCustomers30Days = customers.DistinctVisitedCustomers30Days;
        report.ReturningVisitedCustomers30Days = customers.ReturningVisitedCustomers30Days;
        var peak = await results.ReadFirstOrDefaultAsync<AdminDashboardReport>();
        report.PeakReservationHour30Days = peak?.PeakReservationHour30Days ?? 0;

        const string revenueSql = @"
WITH days AS (
    SELECT CAST(@startDate AS date) AS SalesDate
    UNION ALL SELECT DATEADD(day, 1, SalesDate) FROM days WHERE SalesDate < CAST(@today AS date)
)
SELECT days.SalesDate AS [Date], COALESCE(SUM(o.TotalAmount), 0) AS Revenue
FROM days
LEFT JOIN dbo.TableOrder o ON o.IsClosed = 1
    AND COALESCE(o.ClosedAt, o.OrderDate) >= days.SalesDate
    AND COALESCE(o.ClosedAt, o.OrderDate) < DATEADD(day, 1, days.SalesDate)
GROUP BY days.SalesDate ORDER BY days.SalesDate
OPTION (MAXRECURSION 7);";
        report.DailyRevenue = (await connection.QueryAsync<DailyRevenuePoint>(revenueSql,
            new { startDate = today.AddDays(-6), today })).ToList();

        const string topItemsSql = @"
SELECT TOP (5) m.Name AS ItemName, SUM(l.Quantity) AS Quantity,
       SUM(l.Quantity * l.Price) AS Revenue
FROM dbo.TableOrder o
JOIN dbo.TableOrderLine l ON l.OrderId = o.OrderId
JOIN dbo.MenuItem m ON m.ItemId = l.ItemId
WHERE o.IsClosed = 1 AND COALESCE(o.ClosedAt, o.OrderDate) >= @thirtyDaysAgo AND COALESCE(o.ClosedAt, o.OrderDate) < @tomorrow
GROUP BY m.Name ORDER BY SUM(l.Quantity) DESC, SUM(l.Quantity * l.Price) DESC;";
        report.TopMenuItems = (await connection.QueryAsync<TopMenuItemPoint>(topItemsSql,
            new { thirtyDaysAgo, tomorrow })).ToList();

        const string hoursSql = @"
SELECT DATEPART(HOUR, ReservationTime) AS [Hour], SUM(GuestCount) AS Guests
FROM dbo.Reservation
WHERE ReservationTime >= @thirtyDaysAgo AND ReservationTime < @tomorrow AND Status = 'Arrived'
GROUP BY DATEPART(HOUR, ReservationTime) ORDER BY [Hour];";
        report.ReservationHours = (await connection.QueryAsync<ReservationHourPoint>(hoursSql,
            new { thirtyDaysAgo, tomorrow })).ToList();

        return report;
    }

    public async Task<IEnumerable<InvoiceHistoryItem>> GetInvoiceHistoryAsync(DateTime from, DateTime toExclusive)
    {
        using var connection = new SqlConnection(connectionString);
        const string sql = @"
SELECT o.OrderId, t.TableNumber, o.OrderDate, o.ClosedAt, o.TotalAmount, o.PaymentMethod,
       COALESCE(SUM(l.Quantity), 0) AS ItemCount
FROM dbo.TableOrder o
JOIN dbo.DiningTable t ON t.TableId = o.TableId
LEFT JOIN dbo.TableOrderLine l ON l.OrderId = o.OrderId
WHERE o.IsClosed = 1 AND COALESCE(o.ClosedAt, o.OrderDate) >= @from AND COALESCE(o.ClosedAt, o.OrderDate) < @toExclusive
GROUP BY o.OrderId, t.TableNumber, o.OrderDate, o.ClosedAt, o.TotalAmount, o.PaymentMethod
ORDER BY COALESCE(o.ClosedAt, o.OrderDate) DESC;";
        return await connection.QueryAsync<InvoiceHistoryItem>(sql, new { from, toExclusive });
    }

    public async Task<IEnumerable<InvoiceLineDetail>> GetInvoiceLinesAsync(int orderId)
    {
        using var connection = new SqlConnection(connectionString);
        const string sql = @"
SELECT m.Name AS ItemName, l.Quantity, l.Price AS UnitPrice
FROM dbo.TableOrderLine l
JOIN dbo.MenuItem m ON m.ItemId = l.ItemId
WHERE l.OrderId = @orderId
ORDER BY l.OrderLineId;";
        return await connection.QueryAsync<InvoiceLineDetail>(sql, new { orderId });
    }
}
