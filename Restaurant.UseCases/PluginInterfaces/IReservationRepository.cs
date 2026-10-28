using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;

namespace Restaurant.UseCases.PluginInterfaces;

/// <summary>
/// 1. File này giữ vai trò định nghĩa Interface lưu trữ Reservation.
/// 2. Tương ứng với: Không có trực tiếp.
/// 3. Trả lời GV: "Định nghĩa phương thức để thêm phiếu đặt bàn mới và lấy danh sách các phiếu đặt trước cho một bàn cụ thể để kiểm tra trùng lịch."
/// </summary>
public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetReservationsForTableAsync(int tableId, DateTime reservationTime);
    Task AddReservationAsync(Reservation reservation);
    Task<IEnumerable<Reservation>> GetTodayReservationsAsync();
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task UpdateReservationStatusAsync(Reservation reservation);
}
