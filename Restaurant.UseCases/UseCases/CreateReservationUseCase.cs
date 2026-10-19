using System;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case thực hiện chức năng Đặt bàn của khách hàng.
/// 2. Tương ứng với: Không có trực tiếp.
/// 3. Trả lời GV: "Class này tích hợp kiểm tra Luật nghiệp vụ Đặt bàn từ CoreBusiness trước khi gọi Repository để lưu vào database. Nếu không hợp lệ sẽ ném ngoại lệ."
/// </summary>
public class CreateReservationUseCase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IDiningTableRepository _tableRepository;

    public CreateReservationUseCase(IReservationRepository reservationRepository, IDiningTableRepository tableRepository)
    {
        _reservationRepository = reservationRepository;
        _tableRepository = tableRepository;
    }

    public async Task ExecuteAsync(Reservation reservation)
    {
        // Lấy thông tin bàn
        var table = await _tableRepository.GetTableByIdAsync(reservation.TableId);
        if (table == null)
            throw new Exception("Bàn không tồn tại.");

        // Lấy danh sách các đặt bàn trước của bàn này để kiểm tra trùng lịch
        var existingReservations = (await _reservationRepository
            .GetReservationsForTableAsync(reservation.TableId, reservation.ReservationTime)).ToList();

        // Kiểm tra Business Rule 1 (từ CoreBusiness)
        if (!reservation.IsValidReservation(table, existingReservations))
        {
            throw new Exception("Đặt bàn không hợp lệ: Số khách vượt quá sức chứa hoặc khung giờ bị trùng (cách nhau dưới 2 tiếng).");
        }

        // Nếu hợp lệ, tiến hành lưu
        await _reservationRepository.AddReservationAsync(reservation);
    }
}
