using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là một Entity chứa các luật nghiệp vụ độc lập UI trong lớp CoreBusiness.
/// 2. Tương ứng với: Không có trong eShop (Entity mới cho Restaurant).
/// 3. Trả lời GV: "Class này quản lý thông tin đặt bàn và chứa luật nghiệp vụ: không trùng giờ đặt trong 2 tiếng và số khách không vượt sức chứa bàn."
/// </summary>
public class Reservation
{
    public const string ConfirmedStatus = "Confirmed";
    public const string ArrivedStatus = "Arrived";
    public const string CancelledStatus = "Cancelled";
    public const string NoShowStatus = "NoShow";

    public int ReservationId { get; set; }
    public int TableId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public int GuestCount { get; set; }
    public string Status { get; set; } = ConfirmedStatus;
    public DateTime? CheckedInAt { get; set; }

    // Navigation property
    public DiningTable? Table { get; set; }

    /// <summary>
    /// Luật Đặt bàn 1: Khung giờ đặt không bị trùng trong 2 tiếng và GuestCount &lt;= Capacity.
    /// </summary>
    public bool IsValidReservation(DiningTable table, List<Reservation> existingReservationsForTable)
    {
        // 1. Kiểm tra sức chứa
        if (GuestCount > table.Capacity) return false;

        // 2. Kiểm tra trùng giờ trong vòng 2 tiếng
        foreach (var existing in existingReservationsForTable)
        {
            var timeDifference = Math.Abs((ReservationTime - existing.ReservationTime).TotalHours);
            if (timeDifference < 2)
            {
                return false; // Trùng lịch (cách nhau chưa tới 2 tiếng)
            }
        }
        return true;
    }

    public void MarkArrived()
    {
        EnsureCanChangeStatus();
        Status = ArrivedStatus;
        CheckedInAt = DateTime.Now;
    }

    public void Cancel()
    {
        EnsureCanChangeStatus();
        Status = CancelledStatus;
        CheckedInAt = null;
    }

    public void MarkNoShow(DateTime now)
    {
        EnsureCanChangeStatus();
        if (ReservationTime > now.AddMinutes(-20))
            throw new InvalidOperationException("Chỉ đánh dấu không đến sau khi khách đã trễ ít nhất 20 phút.");

        Status = NoShowStatus;
        CheckedInAt = null;
    }

    private void EnsureCanChangeStatus()
    {
        if (Status != ConfirmedStatus)
            throw new InvalidOperationException("Chỉ lịch đặt đã xác nhận mới có thể cập nhật trạng thái.");
    }
}
