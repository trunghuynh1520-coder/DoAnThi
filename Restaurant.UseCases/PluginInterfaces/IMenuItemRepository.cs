using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;

namespace Restaurant.UseCases.PluginInterfaces;

/// <summary>
/// 1. File này giữ vai trò định nghĩa Interface Repository cho MenuItem, theo Dependency Inversion của Clean Architecture.
/// 2. Tương ứng với: IProductRepository bên dự án eShop.
/// 3. Trả lời GV: "Interface này định nghĩa các phương thức lấy danh sách món ăn từ database, DataStore plugin sẽ implement nó."
/// </summary>
public interface IMenuItemRepository
{
    Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string filter = "");
    Task<MenuItem?> GetMenuItemByIdAsync(int itemId);
}
