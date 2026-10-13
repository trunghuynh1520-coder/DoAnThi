using System.Collections.Generic;
using Restaurant.CoreBusiness;

namespace Restaurant.WebApp.State;

/// <summary>
/// 1. File này giữ vai trò là Scoped Service lưu trữ trạng thái giỏ món ăn của phiên làm việc.
/// 2. Tương ứng với: ShoppingCart bên dự án eShop.
/// 3. Trả lời GV: "Đây là một dịch vụ Scoped trong Blazor, dùng để lưu trữ tạm thời danh sách các món ăn mà khách hàng chọn trước khi tiến hành Gọi món (PlaceOrder)."
/// </summary>
public class ScopedOrderState
{
    public List<TableOrderLine> SelectedItems { get; set; } = new();

    public void AddItem(MenuItem item, int quantity = 1)
    {
        var existing = SelectedItems.Find(x => x.ItemId == item.ItemId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            SelectedItems.Add(new TableOrderLine
            {
                ItemId = item.ItemId,
                Item = item,
                Price = item.Price,
                Quantity = quantity
            });
        }
    }

    public void Clear()
    {
        SelectedItems.Clear();
    }
}
