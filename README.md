# Nhà hàng món Huế – Đặt bàn và gọi món

Ứng dụng Blazor Web App cho khách đặt bàn, xem thực đơn và gọi món tại bàn; nhân viên quản lý bàn, thực đơn và hóa đơn. Dự án sử dụng Clean Architecture, Dapper và SQL Server.

## Công nghệ

- .NET theo `TargetFramework` trong các project
- Blazor Web App – Interactive Server
- SQL Server và Dapper
- 4 project: `Restaurant.CoreBusiness`, `Restaurant.UseCases`, `Restaurant.Plugins.DataStore.SQL`, `Restaurant.WebApp`

## Chuẩn bị cơ sở dữ liệu

1. Tạo database `RestaurantDB` trên SQL Server.
2. Mở và chạy [`Restaurant.Plugins.DataStore.SQL/schema.sql`](Restaurant.Plugins.DataStore.SQL/schema.sql) trong database đó. Script tạo 8 bảng cùng dữ liệu mẫu, bao gồm thực đơn, bàn, đặt chỗ, hóa đơn và nhật ký kho.
3. Kiểm tra connection string trong `Restaurant.WebApp/Program.cs`, hoặc đặt biến môi trường `ConnectionStrings__DefaultConnection` trước khi chạy. Ví dụ PowerShell:

   ```powershell
   $env:ConnectionStrings__DefaultConnection = 'Data Source=.\SQLEXPRESS;Initial Catalog=RestaurantDB;Integrated Security=True;TrustServerCertificate=True'
   ```

   Khi khởi động, ứng dụng bổ sung các cột mới cần thiết cho database đã có.

## Chạy ứng dụng

Tại thư mục chứa `Restaurant.sln`:

```powershell
dotnet restore
dotnet build Restaurant.sln
dotnet run --project Restaurant.WebApp
```

Hoặc mở `Restaurant.sln` bằng Visual Studio và chạy project `Restaurant.WebApp`.

## Tài khoản demo

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Quản trị viên (Admin) | `admin` | `admin` |
| Người dùng (User) | `user` | `user` |

Đây là tài khoản demo phục vụ học tập; không dùng cơ chế đăng nhập cố định này cho môi trường thật.

## Cấu hình phí phục vụ

Trong `Restaurant.WebApp/appsettings.json`, chỉnh `Restaurant:ServiceFeeRate`. Ví dụ `0.10` tương đương 10%. Tỷ lệ này được dùng khi đóng hóa đơn và xuất workbook Excel.

## Chức năng chính

- Đặt bàn và kiểm tra sức chứa / trùng lịch trong khung 2 giờ.
- Xem menu, chỉ chọn món đang phục vụ và gọi món cho bàn đang mở.
- Quản trị bàn, mở bàn, đóng hóa đơn và xuất hóa đơn `.xlsx` từ dữ liệu SQL Server.
- Dashboard quản trị: doanh thu 7 ngày, bill trung bình, công suất bàn, món bán chạy, lượt khách check-in và tỷ lệ khách quay lại.
- Lịch sử bill đã thanh toán: lọc theo ngày, xem chi tiết, hình thức thanh toán và in phiếu nội bộ.
- Quản trị thực đơn: thêm, sửa, xóa và bật/tắt món.
- Các sơ đồ PlantUML nằm trong thư mục `Docs/`.
