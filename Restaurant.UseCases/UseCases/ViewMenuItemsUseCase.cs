using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

/// <summary>
/// 1. File này giữ vai trò là một Use Case cho phép khách hàng xem danh sách món ăn.
/// 2. Tương ứng với: SearchProductsUseCase / ViewProductsUseCase bên dự án eShop.
/// 3. Trả lời GV: "Class này nhận yêu cầu từ UI (Blazor) và gọi xuống IMenuItemRepository để lấy dữ liệu, cách ly UI khỏi DataStore."
/// </summary>
public class ViewMenuItemsUseCase
{
    private readonly IMenuItemRepository _menuItemRepository;

    public ViewMenuItemsUseCase(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<IEnumerable<MenuItem>> ExecuteAsync(string filter = "")
    {
        return await _menuItemRepository.GetMenuItemsAsync(filter);
    }
}
