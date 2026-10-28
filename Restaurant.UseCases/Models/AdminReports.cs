namespace Restaurant.UseCases.Models;

public sealed class AdminDashboardReport
{
    public double TodayRevenue { get; set; }
    public int TodayInvoiceCount { get; set; }
    public double AverageInvoice { get; set; }
    public int OccupiedTables { get; set; }
    public int TotalTables { get; set; }
    public int TodayReservationCount { get; set; }
    public int TodayReservedGuests { get; set; }
    public int TodayArrivedCount { get; set; }
    public int TodayArrivedGuests { get; set; }
    public int DistinctVisitedCustomers30Days { get; set; }
    public int ReturningVisitedCustomers30Days { get; set; }
    public int PeakReservationHour30Days { get; set; }
    public List<DailyRevenuePoint> DailyRevenue { get; set; } = new();
    public List<TopMenuItemPoint> TopMenuItems { get; set; } = new();
    public List<ReservationHourPoint> ReservationHours { get; set; } = new();
}

public sealed class DailyRevenuePoint
{
    public DateTime Date { get; set; }
    public double Revenue { get; set; }
}

public sealed class TopMenuItemPoint
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double Revenue { get; set; }
}

public sealed class ReservationHourPoint
{
    public int Hour { get; set; }
    public int Guests { get; set; }
}

public sealed class InvoiceHistoryItem
{
    public int OrderId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public double TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}

public sealed class InvoiceLineDetail
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
}
