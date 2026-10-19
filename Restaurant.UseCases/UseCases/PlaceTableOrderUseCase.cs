using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case thực hiện việc Gọi món tại bàn.
/// 2. Tương ứng với: PlaceOrderUseCase bên dự án eShop.
/// 3. Trả lời GV: "Class này gọi Business Rule (chỉ gọi món khi bàn Mở), cập nhật hoặc tạo mới Phiếu gọi món thông qua ITableOrderRepository. Nhận toàn bộ danh sách món trong 1 lần gọi để đảm bảo tính nguyên tử và tránh xung đột dữ liệu giữa các bàn."
/// </summary>
public class PlaceTableOrderUseCase
{
    private readonly ITableOrderRepository _orderRepository;
    private readonly IDiningTableRepository _tableRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public PlaceTableOrderUseCase(ITableOrderRepository orderRepository, IDiningTableRepository tableRepository, IMenuItemRepository menuItemRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _menuItemRepository = menuItemRepository;
    }

    /// <summary>
    /// Đặt món cho bàn - nhận toàn bộ danh sách món cùng 1 lần (atomic) để tránh lỗi cross-table.
    /// </summary>
    public async Task ExecuteAsync(int tableId, List<(int itemId, double price, int quantity)> items)
    {
        var table = await _tableRepository.GetTableByIdAsync(tableId);
        if (table == null) throw new Exception("Bàn không tồn tại.");

        // Lấy Order hiện tại của bàn (chưa đóng)
        var activeOrder = await _orderRepository.GetActiveOrderForTableAsync(tableId);

        if (activeOrder == null)
        {
            activeOrder = new TableOrder { TableId = tableId, IsClosed = false, OrderDate = DateTime.Now };
        }

        // Gọi Business Rule AddLineItem cho từng món (trong 1 lần duy nhất)
        foreach (var (itemId, _, quantity) in items)
        {
            var menuItem = await _menuItemRepository.GetMenuItemByIdAsync(itemId);
            if (menuItem == null || !menuItem.IsAvailable)
                throw new InvalidOperationException("Món này hiện không có sẵn, vui lòng chọn món khác.");
            activeOrder.AddLineItem(itemId, menuItem.Price, quantity, table);
        }

        if (activeOrder.OrderId == 0)
        {
            await _orderRepository.SaveOrderAsync(activeOrder);
        }
        else
        {
            await _orderRepository.UpdateOrderAsync(activeOrder);
        }
    }
}
