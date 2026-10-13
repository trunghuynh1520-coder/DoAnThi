using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.Plugins.DataStore.SQL;

/// <summary>
/// 1. File này giữ vai trò là một DataStore Plugin (cụ thể là Dapper/SQL Server) thực thi IMenuItemRepository.
/// 2. Tương ứng với: ProductRepository bên dự án eShop.
/// 3. Trả lời GV: "Class này sử dụng Dapper để truy vấn CSDL lấy danh sách món ăn, 100% sử dụng tham số (@filter) để chống SQL Injection."
/// </summary>
public class MenuItemRepository : IMenuItemRepository
{
    private readonly string _connectionString;

    public MenuItemRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string filter = "")
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM MenuItem WHERE Name LIKE '%' + @filter + '%'";
        return await connection.QueryAsync<MenuItem>(sql, new { filter });
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int itemId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM MenuItem WHERE ItemId = @itemId";
        return await connection.QueryFirstOrDefaultAsync<MenuItem>(sql, new { itemId });
    }
}
