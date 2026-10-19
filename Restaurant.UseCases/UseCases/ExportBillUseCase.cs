using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case tính năng nâng cao (K4.3): Xuất hóa đơn Excel từ dữ liệu thật.
/// 2. Trả lời GV: "Use Case trích xuất phiếu gọi món đang mở, chi tiết món, phí phục vụ cấu hình và tổng tiền thành workbook Excel."
/// </summary>
public class ExportBillUseCase
{
    private readonly ITableOrderRepository _orderRepository;
    private readonly IDiningTableRepository _tableRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly double _serviceFeeRate;

    public ExportBillUseCase(
        ITableOrderRepository orderRepository,
        IDiningTableRepository tableRepository,
        IMenuItemRepository menuItemRepository,
        double serviceFeeRate)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _menuItemRepository = menuItemRepository;
        _serviceFeeRate = serviceFeeRate;
    }

    public async Task<byte[]> ExecuteAsync(int tableId)
    {
        var table = await _tableRepository.GetTableByIdAsync(tableId);
        if (table == null)
            throw new Exception("Bàn không tồn tại.");

        var order = await _orderRepository.GetActiveOrderForTableAsync(tableId);
        if (order == null || !order.OrderLines.Any())
            throw new Exception($"Bàn {table.TableNumber} hiện chưa có món nào trong phiếu gọi món.");

        var allItems = (await _menuItemRepository.GetMenuItemsAsync()).ToDictionary(x => x.ItemId, x => x.Name);

        var rows = new List<IReadOnlyList<object?>>
        {
            new object?[] { "NHÀ HÀNG MÓN HUẾ" },
            new object?[] { "Mã hóa đơn", order.OrderId },
            new object?[] { "Bàn", table.TableNumber },
            new object?[] { "Thời gian", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
            new object?[] { "Tên món", "Số lượng", "Đơn giá (₫)", "Thành tiền (₫)" }
        };
        double subTotal = 0;
        foreach (var line in order.OrderLines)
        {
            string itemName = allItems.TryGetValue(line.ItemId, out var name) ? name : $"Món #{line.ItemId}";
            double lineTotal = line.Price * line.Quantity;
            subTotal += lineTotal;
            rows.Add(new object?[] { itemName, line.Quantity, line.Price, lineTotal });
        }

        double serviceFee = subTotal * _serviceFeeRate;
        double grandTotal = subTotal + serviceFee;
        rows.Add(new object?[] { "Tổng tiền món", subTotal });
        rows.Add(new object?[] { $"Phí phục vụ ({_serviceFeeRate:P0})", serviceFee });
        rows.Add(new object?[] { "TỔNG THANH TOÁN", grandTotal });
        return ExcelWorkbookWriter.Create(rows);
    }
}
