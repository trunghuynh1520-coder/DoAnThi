namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là Entity chi tiết của Phiếu gọi món trong lớp CoreBusiness.
/// 2. Tương ứng với class: OrderLineItem bên dự án eShop.
/// 3. Trả lời GV: "Class này lưu trữ chi tiết số lượng và giá của một món ăn cụ thể được gọi trong một Phiếu gọi món."
/// </summary>
public class TableOrderLine
{
    public int OrderLineId { get; set; }
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public MenuItem? Item { get; set; }
    public TableOrder? Order { get; set; }
}
