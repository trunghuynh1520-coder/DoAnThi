# HỒ SƠ CHẤM ĐIỂM BẢO VỆ CAPSTONE (GRADING.MD)
**Đề tài 11:** Nhà hàng món Huế - Đặt bàn & Gọi món
**Công nghệ:** .NET 8 Blazor Web App (InteractiveServer), Dapper, SQL Server, Clean Architecture

Dưới đây là bảng ánh xạ mã kiểm tra (Rubric) với vị trí File và Dòng code thực tế trong dự án để Giảng viên tiện theo dõi và chấm điểm:

| Mã Rubric | Yêu cầu kiểm tra | File Path | Dòng Code | Mô tả nhanh |
| :--- | :--- | :--- | :--- | :--- |
| **K1.1** | Khởi tạo Entity chuẩn và Navigation Properties | `Restaurant.CoreBusiness/Models/MenuItem.cs` | Dòng 14-16 | Thiết lập thuộc tính và liên kết Category. |
| **K1.2** | Cài đặt Luật nghiệp vụ 1 (Độc lập UI) | `Restaurant.CoreBusiness/Models/Reservation.cs` | Dòng 21-36 | Hàm `IsValidReservation`: Kiểm tra sức chứa bàn và chống trùng giờ đặt trong 2 tiếng. |
| **K1.3** | Cài đặt Luật nghiệp vụ 2 (Độc lập UI) | `Restaurant.CoreBusiness/Models/TableOrder.cs` | Dòng 24-41 | Hàm `AddLineItem`: Chỉ cho gọi món khi bàn đang Mở (IsOccupied = true) và phiếu chưa đóng. |
| **K2.1** | Xây dựng Use Cases | `Restaurant.UseCases/UseCases/CreateReservationUseCase.cs` | Dòng 27-41 | UseCase Đặt bàn, gọi Luật nghiệp vụ từ CoreBusiness trước khi Save. |
| **K2.2** | Áp dụng Dependency Inversion (Interfaces) | `Restaurant.UseCases/PluginInterfaces/ITableOrderRepository.cs` | Dòng 13-17 | Tách biệt tầng logic và DataStore thông qua Interfaces. |
| **K3.1** | Thiết kế CSDL chuẩn 3NF có Khóa Ngoại | `Restaurant.Plugins.DataStore.SQL/schema.sql` | Toàn bộ file | 6 bảng CSDL có khóa chính, khóa ngoại liên kết đầy đủ. |
| **K3.2** | Sử dụng Dapper & Chống SQL Injection | `Restaurant.Plugins.DataStore.SQL/MenuItemRepository.cs` | Dòng 23-28 | Parameterized queries `new { filter }` chống SQL Injection. |
| **K3.3** | Xử lý Transaction khi lưu dữ liệu phức tạp | `Restaurant.Plugins.DataStore.SQL/TableOrderRepository.cs` | Dòng 35-64 | Dùng `IDbTransaction` để lưu cả `TableOrder` và nhiều `TableOrderLine` cùng lúc. |
| **K4.1** | Sử dụng Scoped State (Giỏ chọn món) | `Restaurant.WebApp/State/ScopedOrderState.cs` | Dòng 12-32 | Quản lý tạm thời danh sách món được chọn trong phiên làm việc. |
| **K4.2** | UI Blazor & DataAnnotationsValidator | `Restaurant.WebApp/Components/Pages/ReservationPage.razor` | Dòng 28-50, 71-86 | Dùng `EditForm` để validate dữ liệu đầu vào (Tên, SDT, số khách) trước khi gọi UseCase. |
| **K5.1** | Phân quyền (Authentication/Authorization) | `Restaurant.WebApp/Program.cs` | Dòng 13-20 | Thiết lập Cookie Authentication bảo vệ trang của Admin. |
| **K5.2** | Phân quyền Component | `Restaurant.WebApp/Components/Pages/Admin/Tables.razor` | Dòng 3 | Gắn thẻ `[Authorize(Roles = "Admin")]` cho trang Sơ đồ bàn. |
| **K5.3** | Đăng ký Dependency Injection (DI) | `Restaurant.WebApp/Program.cs` | Dòng 27-36 | Đăng ký Repositories và UseCases vào container của Blazor. |
