namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là một Entity trong lớp CoreBusiness (thuộc Clean Architecture).
/// 2. Tương ứng với class: Product bên dự án eShop.
/// 3. Trả lời GV: "Class này định nghĩa cấu trúc dữ liệu cho Món ăn, chứa thông tin tên, giá, hình ảnh và thuộc về danh mục nào."
/// </summary>
public class MenuItem
{
    public int ItemId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

    // Navigation property
    public MenuCategory? Category { get; set; }
}
