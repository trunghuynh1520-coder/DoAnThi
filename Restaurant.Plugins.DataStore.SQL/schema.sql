-- 1. File này giữ vai trò là Script khởi tạo CSDL cho DataStore Plugin.
-- 2. Tương ứng với: Script tạo bảng bên dự án eShop.
-- 3. Trả lời GV: "Script này tạo 6 bảng chuẩn 3NF có khóa ngoại đầy đủ, phục vụ việc lưu trữ dữ liệu cho ứng dụng Nhà hàng món Huế."

CREATE TABLE MenuCategory (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

CREATE TABLE MenuItem (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price FLOAT NOT NULL,
    ImageUrl NVARCHAR(500),
    FOREIGN KEY (CategoryId) REFERENCES MenuCategory(CategoryId)
);

CREATE TABLE DiningTable (
    TableId INT IDENTITY(1,1) PRIMARY KEY,
    TableNumber NVARCHAR(20) NOT NULL,
    Capacity INT NOT NULL,
    IsOccupied BIT NOT NULL DEFAULT 0
);

CREATE TABLE Reservation (
    ReservationId INT IDENTITY(1,1) PRIMARY KEY,
    TableId INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    ReservationTime DATETIME NOT NULL,
    GuestCount INT NOT NULL,
    FOREIGN KEY (TableId) REFERENCES DiningTable(TableId)
);

CREATE TABLE TableOrder (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    TableId INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    IsClosed BIT NOT NULL DEFAULT 0,
    TotalAmount FLOAT NOT NULL DEFAULT 0,
    FOREIGN KEY (TableId) REFERENCES DiningTable(TableId)
);

CREATE TABLE TableOrderLine (
    OrderLineId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ItemId INT NOT NULL,
    Price FLOAT NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES TableOrder(OrderId),
    FOREIGN KEY (ItemId) REFERENCES MenuItem(ItemId)
);

-- Dữ liệu mẫu cho Menu
INSERT INTO MenuCategory (Name, Description) VALUES (N'Món Nước', N'Các loại bún, phở đặc sản Huế');
INSERT INTO MenuCategory (Name, Description) VALUES (N'Món Ăn Chơi', N'Bánh bèo, nậm, lọc');

INSERT INTO MenuItem (CategoryId, Name, Description, Price, ImageUrl) VALUES (1, N'Bún Bò Huế', N'Bún bò chuẩn vị Huế', 55000, '');
INSERT INTO MenuItem (CategoryId, Name, Description, Price, ImageUrl) VALUES (2, N'Bánh Bèo Chén', N'Bánh bèo tôm cháy', 35000, '');

-- Dữ liệu mẫu cho Bàn
INSERT INTO DiningTable (TableNumber, Capacity, IsOccupied) VALUES ('T1', 4, 0);
INSERT INTO DiningTable (TableNumber, Capacity, IsOccupied) VALUES ('T2', 2, 0);
INSERT INTO DiningTable (TableNumber, Capacity, IsOccupied) VALUES ('T3', 6, 0);
