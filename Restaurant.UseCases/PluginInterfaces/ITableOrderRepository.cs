using System.Threading.Tasks;
using Restaurant.CoreBusiness;

namespace Restaurant.UseCases.PluginInterfaces;

/// <summary>
/// 1. File này giữ vai trò định nghĩa Interface quản lý Phiếu gọi món.
/// 2. Tương ứng với: IOrderRepository bên dự án eShop.
/// 3. Trả lời GV: "Dùng để lưu và cập nhật Phiếu gọi món cùng chi tiết món ăn (TableOrderLine) xuống cơ sở dữ liệu (thường cần dùng Transaction)."
/// </summary>
public interface ITableOrderRepository
{
    Task<TableOrder?> GetActiveOrderForTableAsync(int tableId);
    Task SaveOrderAsync(TableOrder order);
    Task UpdateOrderAsync(TableOrder order);
}
