using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case thực hiện việc Gọi món tại bàn.
/// 2. Tương ứng với: PlaceOrderUseCase bên dự án eShop.
/// 3. Trả lời GV: "Class này gọi Business Rule (chỉ gọi món khi bàn Mở), cập nhật hoặc tạo mới Phiếu gọi món thông qua ITableOrderRepository."
/// </summary>
public class PlaceTableOrderUseCase
{
    private readonly ITableOrderRepository _orderRepository;
    private readonly IDiningTableRepository _tableRepository;

    public PlaceTableOrderUseCase(ITableOrderRepository orderRepository, IDiningTableRepository tableRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
    }

    public async Task ExecuteAsync(int tableId, int itemId, double price, int quantity)
    {
        var table = await _tableRepository.GetTableByIdAsync(tableId);
        if (table == null) throw new Exception("Bàn không tồn tại.");

        // Lấy Order hiện tại của bàn (chưa đóng)
        var activeOrder = await _orderRepository.GetActiveOrderForTableAsync(tableId);
        
        if (activeOrder == null)
        {
            activeOrder = new TableOrder { TableId = tableId, IsClosed = false, OrderDate = DateTime.Now };
        }

        // Gọi method chứa Business Rule 2
        activeOrder.AddLineItem(itemId, price, quantity, table);

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
