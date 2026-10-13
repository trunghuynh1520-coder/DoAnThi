using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.Plugins.DataStore.SQL;

/// <summary>
/// 1. File này giữ vai trò Repository thao tác với Phiếu Đặt Bàn.
/// 2. Tương ứng với: Không có.
/// 3. Trả lời GV: "Repository này dùng để thêm mới lịch đặt và lấy lịch hiện tại của bàn trong ngày để CoreBusiness kiểm tra Luật nghiệp vụ."
/// </summary>
public class ReservationRepository : IReservationRepository
{
    private readonly string _connectionString;

    public ReservationRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = @"
            INSERT INTO Reservation (TableId, CustomerName, PhoneNumber, ReservationTime, GuestCount)
            VALUES (@TableId, @CustomerName, @PhoneNumber, @ReservationTime, @GuestCount)";
        await connection.ExecuteAsync(sql, reservation);
    }

    public async Task<IEnumerable<Reservation>> GetReservationsForTableAsync(int tableId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Reservation WHERE TableId = @tableId AND CAST(ReservationTime as DATE) = CAST(GETDATE() as DATE)";
        return await connection.QueryAsync<Reservation>(sql, new { tableId });
    }
}
