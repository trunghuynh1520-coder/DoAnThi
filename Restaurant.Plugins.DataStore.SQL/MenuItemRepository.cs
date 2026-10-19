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
        string sql = @"SELECT m.*, c.Name AS CategoryName
                       FROM MenuItem m
                       INNER JOIN MenuCategory c ON c.CategoryId = m.CategoryId
                       WHERE m.Name LIKE '%' + @filter + '%'
                       ORDER BY c.Name, m.Name";
        return await connection.QueryAsync<MenuItem>(sql, new { filter });
    }

    public async Task<MenuItem?> GetMenuItemByIdAsync(int itemId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM MenuItem WHERE ItemId = @itemId";
        return await connection.QueryFirstOrDefaultAsync<MenuItem>(sql, new { itemId });
    }

    public async Task<IEnumerable<MenuCategory>> GetCategoriesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<MenuCategory>("SELECT * FROM MenuCategory ORDER BY Name");
    }

    public async Task AddMenuItemAsync(MenuItem item)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"INSERT INTO MenuItem (CategoryId, Name, Description, Price, ImageUrl, IsAvailable)
                             VALUES (@CategoryId, @Name, @Description, @Price, @ImageUrl, @IsAvailable)";
        await connection.ExecuteAsync(sql, item);
    }

    public async Task UpdateMenuItemAsync(MenuItem item)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = @"UPDATE MenuItem
                             SET CategoryId = @CategoryId, Name = @Name, Description = @Description,
                                 Price = @Price, ImageUrl = @ImageUrl, IsAvailable = @IsAvailable
                             WHERE ItemId = @ItemId";
        await connection.ExecuteAsync(sql, item);
    }

    public async Task DeleteMenuItemAsync(int itemId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("DELETE FROM MenuItem WHERE ItemId = @itemId", new { itemId });
    }
}
