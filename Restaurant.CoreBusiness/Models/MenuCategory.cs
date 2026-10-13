namespace Restaurant.CoreBusiness;

/// <summary>
/// 1. File này giữ vai trò là một Entity trong lớp CoreBusiness (thuộc Clean Architecture).
/// 2. Tương ứng với class: Category bên dự án eShop.
/// 3. Trả lời GV: "Class này dùng để map với bảng MenuCategory trong DB, quản lý các danh mục món ăn (ví dụ: Khai vị, Món chính, Đồ uống)."
/// </summary>
public class MenuCategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public List<MenuItem> MenuItems { get; set; } = new();
}
