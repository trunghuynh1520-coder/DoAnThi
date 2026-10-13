using System;
using System.Collections.Generic;
using System.Linq;

namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là Entity chính quản lý 1 Phiếu gọi món trong lớp CoreBusiness.
/// 2. Tương ứng với class: Order bên dự án eShop.
/// 3. Trả lời GV: "Class này đại diện cho một Phiếu gọi món của Bàn, bao gồm các dòng món (TableOrderLine) và tính tổng tiền cộng phụ phí. Nó cũng đảm bảo không cho phép sửa khi bàn đã đóng."
/// </summary>
public class TableOrder
{
    public int OrderId { get; set; }
    public int TableId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public bool IsClosed { get; set; }
    
    // Thuộc tính để lưu tổng tiền sau khi đóng
    public double TotalAmount { get; set; }

    public List<TableOrderLine> OrderLines { get; set; } = new();

    /// <summary>
    /// Luật Gọi món & Đóng hóa đơn
    /// 1. Chỉ gọi món khi bàn đang Mở (IsOccupied = true)
    /// 2. Hóa đơn = SUM(Quantity * UnitPrice) + Phí phục vụ (giả sử phí phục vụ là 10%)
    /// 3. Cấm sửa khi bàn đã Đóng (IsClosed = true)
    /// </summary>
    public void AddLineItem(int itemId, double price, int quantity, DiningTable table)
    {
        if (IsClosed)
            throw new InvalidOperationException("Không thể gọi thêm món, hóa đơn đã đóng.");
            
        if (!table.IsOccupied)
            throw new InvalidOperationException("Chỉ được gọi món khi bàn đang được sử dụng (IsOccupied = true).");

        var existingLine = OrderLines.FirstOrDefault(x => x.ItemId == itemId);
        if (existingLine != null)
        {
            existingLine.Quantity += quantity;
        }
        else
        {
            OrderLines.Add(new TableOrderLine
            {
                OrderId = this.OrderId,
                ItemId = itemId,
                Price = price,
                Quantity = quantity
            });
        }
    }

    public void CloseOrder()
    {
        if (IsClosed)
            throw new InvalidOperationException("Hóa đơn đã được đóng từ trước.");

        double subTotal = OrderLines.Sum(x => x.Price * x.Quantity);
        double serviceFee = subTotal * 0.1; // Phí phục vụ 10%
        TotalAmount = subTotal + serviceFee;
        IsClosed = true;
    }
}
