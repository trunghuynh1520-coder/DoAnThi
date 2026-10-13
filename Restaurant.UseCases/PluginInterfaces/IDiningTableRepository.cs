using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;

namespace Restaurant.UseCases.PluginInterfaces;

/// <summary>
/// 1. File này giữ vai trò định nghĩa Interface cho Bàn ăn.
/// 2. Tương ứng với: Không có trực tiếp (Entity mới).
/// 3. Trả lời GV: "Dùng để thao tác (lấy thông tin, cập nhật trạng thái trống/bận) của bàn ăn trong cơ sở dữ liệu."
/// </summary>
public interface IDiningTableRepository
{
    Task<IEnumerable<DiningTable>> GetTablesAsync();
    Task<DiningTable?> GetTableByIdAsync(int tableId);
    Task UpdateTableStatusAsync(int tableId, bool isOccupied);
    Task OpenTableAsync(int tableId);   // Mở bàn khi khách vào (Admin action)
}
