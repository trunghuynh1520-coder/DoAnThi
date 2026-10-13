namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là một Entity bổ sung trong lớp CoreBusiness.
/// 2. Tương ứng với: Không có trong eShop (Entity mới cho Restaurant).
/// 3. Trả lời GV: "Class này đại diện cho Bàn ăn trong nhà hàng, chứa thông tin sức chứa và trạng thái có người ngồi hay không."
/// </summary>
public class DiningTable
{
    public int TableId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsOccupied { get; set; }
}
