using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

public class ManageMenuItemsUseCase
{
    private readonly IMenuItemRepository _repository;

    public ManageMenuItemsUseCase(IMenuItemRepository repository) => _repository = repository;

    public Task<IEnumerable<MenuItem>> GetItemsAsync(string filter = "") => _repository.GetMenuItemsAsync(filter);
    public Task<IEnumerable<MenuCategory>> GetCategoriesAsync() => _repository.GetCategoriesAsync();
    public Task AddAsync(MenuItem item) => _repository.AddMenuItemAsync(item);
    public Task UpdateAsync(MenuItem item) => _repository.UpdateMenuItemAsync(item);
    public Task DeleteAsync(int itemId) => _repository.DeleteMenuItemAsync(itemId);
}
