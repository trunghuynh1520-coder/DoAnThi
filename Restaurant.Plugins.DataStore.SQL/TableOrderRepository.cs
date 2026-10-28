using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.Plugins.DataStore.SQL;

/// <summary>
/// 1. File này giữ vai trò Repository thao tác với Order.
/// 2. Tương ứng với: OrderRepository bên dự án eShop.
/// 3. Trả lời GV: "Sử dụng Dapper với IDbTransaction để đảm bảo tính toàn vẹn dữ liệu (ACID) khi lưu đồng thời thông tin Order (bảng TableOrder) và chi tiết các món (bảng TableOrderLine)."
/// </summary>
public class TableOrderRepository : ITableOrderRepository
{
    private readonly string _connectionString;

    public TableOrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<TableOrder?> GetActiveOrderForTableAsync(int tableId)
    {
        using var connection = new SqlConnection(_connectionString);
        string orderSql = "SELECT * FROM TableOrder WHERE TableId = @tableId AND IsClosed = 0";
        var order = await connection.QueryFirstOrDefaultAsync<TableOrder>(orderSql, new { tableId });

        if (order != null)
        {
            string lineSql = "SELECT * FROM TableOrderLine WHERE OrderId = @orderId";
            var lines = await connection.QueryAsync<TableOrderLine>(lineSql, new { orderId = order.OrderId });
            order.OrderLines = lines.ToList();
        }

        return order;
    }

    public async Task SaveOrderAsync(TableOrder order)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string orderSql = @"
                INSERT INTO TableOrder (TableId, OrderDate, IsClosed, TotalAmount)
                OUTPUT INSERTED.OrderId
                VALUES (@TableId, @OrderDate, @IsClosed, @TotalAmount)";
            
            order.OrderId = await connection.ExecuteScalarAsync<int>(orderSql, order, transaction);

            string lineSql = @"
                INSERT INTO TableOrderLine (OrderId, ItemId, Price, Quantity)
                VALUES (@OrderId, @ItemId, @Price, @Quantity)";
                
            foreach (var line in order.OrderLines)
            {
                line.OrderId = order.OrderId;
                await connection.ExecuteAsync(lineSql, line, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateOrderAsync(TableOrder order)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string orderSql = @"
                UPDATE TableOrder
                SET IsClosed = @IsClosed, TotalAmount = @TotalAmount, ClosedAt = @ClosedAt,
                    PaymentMethod = @PaymentMethod
                WHERE OrderId = @OrderId";
            await connection.ExecuteAsync(orderSql, order, transaction);

            // Cập nhật lại OrderLines (Cách đơn giản: Xóa cũ, thêm mới)
            string deleteLinesSql = "DELETE FROM TableOrderLine WHERE OrderId = @OrderId";
            await connection.ExecuteAsync(deleteLinesSql, new { OrderId = order.OrderId }, transaction);

            string lineSql = @"
                INSERT INTO TableOrderLine (OrderId, ItemId, Price, Quantity)
                VALUES (@OrderId, @ItemId, @Price, @Quantity)";
            foreach (var line in order.OrderLines)
            {
                await connection.ExecuteAsync(lineSql, line, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
