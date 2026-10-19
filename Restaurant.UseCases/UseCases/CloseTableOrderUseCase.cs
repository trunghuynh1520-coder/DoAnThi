using System;
using System.Threading.Tasks;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là Use Case để đóng hóa đơn và tính tiền.
/// 2. Tương ứng với: ProcessOrderUseCase bên dự án eShop.
/// 3. Trả lời GV: "Class này gọi phương thức CloseOrder() của TableOrder để tính tổng tiền (gồm phí dịch vụ), sau đó cập nhật database qua Repository và giải phóng Bàn."
/// </summary>
public class CloseTableOrderUseCase
{
    private readonly ITableOrderRepository _orderRepository;
    private readonly IDiningTableRepository _tableRepository;
    private readonly double _serviceFeeRate;

    public CloseTableOrderUseCase(ITableOrderRepository orderRepository, IDiningTableRepository tableRepository, double serviceFeeRate)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _serviceFeeRate = serviceFeeRate;
    }

    public async Task ExecuteAsync(int tableId)
    {
        var activeOrder = await _orderRepository.GetActiveOrderForTableAsync(tableId);
        if (activeOrder == null) throw new Exception("Không tìm thấy hóa đơn nào đang mở cho bàn này.");

        // Gọi Business Rule để đóng hóa đơn và tính tổng tiền
        activeOrder.CloseOrder(_serviceFeeRate);

        await _orderRepository.UpdateOrderAsync(activeOrder);
        
        // Giải phóng bàn (IsOccupied = false)
        await _tableRepository.UpdateTableStatusAsync(tableId, false);
    }
}
