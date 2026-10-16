using System;
using System.Text;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case tính năng nâng cao (K4.3): Xuất báo cáo / hóa đơn từ dữ liệu thật.
/// 2. Trả lời GV: "Use Case này trích xuất thông tin Order hiện tại của bàn, chi tiết từng món ăn, tính phí phục vụ 10% và tổng thanh toán, định dạng thành hóa đơn chuẩn văn bản/phiếu in."
/// </summary>
public class ExportBillUseCase
{
    private readonly ITableOrderRepository _orderRepository;
    private readonly IDiningTableRepository _tableRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public ExportBillUseCase(
        ITableOrderRepository orderRepository,
        IDiningTableRepository tableRepository,
        IMenuItemRepository menuItemRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<string> ExecuteAsync(int tableId)
    {
        var table = await _tableRepository.GetTableByIdAsync(tableId);
        if (table == null)
            throw new Exception("Bàn không tồn tại.");

        var order = await _orderRepository.GetActiveOrderForTableAsync(tableId);
        if (order == null || !order.OrderLines.Any())
            throw new Exception($"Bàn {table.TableNumber} hiện chưa có món nào trong phiếu gọi món.");

        var allItems = (await _menuItemRepository.GetMenuItemsAsync()).ToDictionary(x => x.ItemId, x => x.Name);

        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("              NHÀ HÀNG MÓN HUẾ                   ");
        sb.AppendLine("         Tinh Hoa Ẩm Thực Cố Đô Huế              ");
        sb.AppendLine("==================================================");
        sb.AppendLine($" Số bàn: Bàn {table.TableNumber} (Sức chứa: {table.Capacity} khách)");
        sb.AppendLine($" Mã hóa đơn: #{order.OrderId}");
        sb.AppendLine($" Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine(string.Format("{0,-20} {1,5} {2,10} {3,12}", "Tên món", "SL", "Đơn giá", "Thành tiền"));
        sb.AppendLine("--------------------------------------------------");

        double subTotal = 0;
        foreach (var line in order.OrderLines)
        {
            string itemName = allItems.TryGetValue(line.ItemId, out var name) ? name : $"Món #{line.ItemId}";
            if (itemName.Length > 20) itemName = itemName.Substring(0, 17) + "...";
            double lineTotal = line.Price * line.Quantity;
            subTotal += lineTotal;

            sb.AppendLine(string.Format("{0,-20} {1,5} {2,10:N0}đ {3,11:N0}đ", itemName, line.Quantity, line.Price, lineTotal));
        }

        double serviceFee = subTotal * 0.1;
        double grandTotal = subTotal + serviceFee;

        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine(string.Format("{0,-35} {1,13:N0}đ", "Tổng tiền món:", subTotal));
        sb.AppendLine(string.Format("{0,-35} {1,13:N0}đ", "Phí phục vụ (10%):", serviceFee));
        sb.AppendLine("==================================================");
        sb.AppendLine(string.Format("{0,-35} {1,13:N0}đ", "TỔNG THANH TOÁN:", grandTotal));
        sb.AppendLine("==================================================");
        sb.AppendLine("         CẢM ƠN QUÝ KHÁCH VÀ HẸN GẶP LẠI!        ");
        sb.AppendLine("               Hotline: 1900 6868                ");

        return sb.ToString();
    }
}
