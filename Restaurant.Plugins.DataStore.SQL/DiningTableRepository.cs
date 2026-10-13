using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.Plugins.DataStore.SQL;

/// <summary>
/// 1. File này giữ vai trò Repository thao tác với Bàn ăn.
/// 2. Tương ứng với: Không có (Bảng mới).
/// 3. Trả lời GV: "Thực hiện truy vấn lấy danh sách bàn và cập nhật trạng thái bàn (có người hay trống), dùng Dapper chống SQL Injection."
/// </summary>
public class DiningTableRepository : IDiningTableRepository
{
    private readonly string _connectionString;

    public DiningTableRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<DiningTable>> GetTablesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM DiningTable";
        return await connection.QueryAsync<DiningTable>(sql);
    }

    public async Task<DiningTable?> GetTableByIdAsync(int tableId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM DiningTable WHERE TableId = @tableId";
        return await connection.QueryFirstOrDefaultAsync<DiningTable>(sql, new { tableId });
    }

    public async Task UpdateTableStatusAsync(int tableId, bool isOccupied)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "UPDATE DiningTable SET IsOccupied = @isOccupied WHERE TableId = @tableId";
        await connection.ExecuteAsync(sql, new { isOccupied, tableId });
    }

    public async Task OpenTableAsync(int tableId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "UPDATE DiningTable SET IsOccupied = 1 WHERE TableId = @tableId";
        await connection.ExecuteAsync(sql, new { tableId });
    }
}
