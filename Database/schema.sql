/* =========================================================
   GameHub - Game Shop Management System
   Database schema + seed data  (Microsoft SQL Server / T-SQL)
   Run this whole script once in SSMS before starting the app.
   ========================================================= */
IF DB_ID('GameHubDB') IS NULL
    CREATE DATABASE GameHubDB;
GO
USE GameHubDB;
GO

/* ---------- drop existing (safe re-run) ---------- */
IF OBJECT_ID('vw_RevenueByGenre','V') IS NOT NULL DROP VIEW vw_RevenueByGenre;
IF OBJECT_ID('GameKeys','U')           IS NOT NULL DROP TABLE GameKeys;
IF OBJECT_ID('Reviews','U')            IS NOT NULL DROP TABLE Reviews;
IF OBJECT_ID('Wishlist','U')           IS NOT NULL DROP TABLE Wishlist;
IF OBJECT_ID('WalletTransactions','U') IS NOT NULL DROP TABLE WalletTransactions;
IF OBJECT_ID('Wallet','U')             IS NOT NULL DROP TABLE Wallet;
IF OBJECT_ID('Subscriptions','U')      IS NOT NULL DROP TABLE Subscriptions;
IF OBJECT_ID('SubscriptionPlans','U')  IS NOT NULL DROP TABLE SubscriptionPlans;
IF OBJECT_ID('Payments','U')           IS NOT NULL DROP TABLE Payments;
IF OBJECT_ID('OrderDetails','U')       IS NOT NULL DROP TABLE OrderDetails;
IF OBJECT_ID('Orders','U')             IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Coupons','U')            IS NOT NULL DROP TABLE Coupons;
IF OBJECT_ID('Games','U')              IS NOT NULL DROP TABLE Games;
IF OBJECT_ID('Publishers','U')         IS NOT NULL DROP TABLE Publishers;
IF OBJECT_ID('Genres','U')             IS NOT NULL DROP TABLE Genres;
IF OBJECT_ID('Users','U')              IS NOT NULL DROP TABLE Users;
GO

/* ---------- core tables ---------- */
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     VARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    Role         VARCHAR(20)  NOT NULL,
    FullName     VARCHAR(100) NOT NULL,
    Email        VARCHAR(100),
    Phone        VARCHAR(20),
    IsActive     BIT          NOT NULL DEFAULT 1,
    CreatedAt    DATETIME     NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Genres (
    GenreID   INT IDENTITY(1,1) PRIMARY KEY,
    GenreName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Publishers (
    PublisherID   INT IDENTITY(1,1) PRIMARY KEY,
    PublisherName VARCHAR(100) NOT NULL,
    Country       VARCHAR(50)
);

CREATE TABLE Games (
    GameID        INT IDENTITY(1,1) PRIMARY KEY,
    Title         VARCHAR(150) NOT NULL,
    Description   VARCHAR(MAX),
    GenreID       INT NOT NULL FOREIGN KEY REFERENCES Genres(GenreID),
    PublisherID   INT NOT NULL FOREIGN KEY REFERENCES Publishers(PublisherID),
    Price         DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    ReleaseDate   DATE,
    IsActive      BIT NOT NULL DEFAULT 1
);

CREATE TABLE Orders (
    OrderID            INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID         INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    ProcessedByStaffID INT NULL     FOREIGN KEY REFERENCES Users(UserID),
    OrderDate          DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount        DECIMAL(10,2) NOT NULL,
    Status             VARCHAR(20) NOT NULL DEFAULT 'Pending'
);

CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID   INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    GameID    INT NOT NULL FOREIGN KEY REFERENCES Games(GameID),
    Quantity  INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    Subtotal  DECIMAL(10,2) NOT NULL
);

CREATE TABLE Payments (
    PaymentID     INT IDENTITY(1,1) PRIMARY KEY,
    OrderID       INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Orders(OrderID),
    Amount        DECIMAL(10,2) NOT NULL,
    PaymentMethod VARCHAR(30),
    PaymentDate   DATETIME NOT NULL DEFAULT GETDATE(),
    Status        VARCHAR(20) NOT NULL DEFAULT 'Completed'
);

/* ---------- extended / feature tables ---------- */
CREATE TABLE SubscriptionPlans (
    PlanID            INT IDENTITY(1,1) PRIMARY KEY,
    PlanName          VARCHAR(30) NOT NULL,
    MonthlyPrice      DECIMAL(10,2) NOT NULL,
    DiscountPct       INT NOT NULL DEFAULT 0,
    FreeGamesPerMonth INT NOT NULL DEFAULT 0
);

CREATE TABLE Subscriptions (
    SubscriptionID INT IDENTITY(1,1) PRIMARY KEY,
    UserID    INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    PlanID    INT NOT NULL FOREIGN KEY REFERENCES SubscriptionPlans(PlanID),
    StartDate DATE NOT NULL DEFAULT GETDATE(),
    EndDate   DATE NOT NULL,
    Status    VARCHAR(20) NOT NULL DEFAULT 'Active'
);

CREATE TABLE Wallet (
    WalletID      INT IDENTITY(1,1) PRIMARY KEY,
    UserID        INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserID),
    Balance       DECIMAL(10,2) NOT NULL DEFAULT 0,
    LoyaltyPoints INT NOT NULL DEFAULT 0
);

CREATE TABLE WalletTransactions (
    TxID     INT IDENTITY(1,1) PRIMARY KEY,
    WalletID INT NOT NULL FOREIGN KEY REFERENCES Wallet(WalletID),
    TxType   VARCHAR(20) NOT NULL,
    Amount   DECIMAL(10,2) NOT NULL,
    TxDate   DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Wishlist (
    WishlistID  INT IDENTITY(1,1) PRIMARY KEY,
    UserID      INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    GameID      INT NOT NULL FOREIGN KEY REFERENCES Games(GameID),
    AlertOnSale BIT NOT NULL DEFAULT 1
);

CREATE TABLE Reviews (
    ReviewID  INT IDENTITY(1,1) PRIMARY KEY,
    UserID    INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    GameID    INT NOT NULL FOREIGN KEY REFERENCES Games(GameID),
    Rating    INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment   VARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE GameKeys (
    KeyID          INT IDENTITY(1,1) PRIMARY KEY,
    OrderID        INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    GameID         INT NOT NULL FOREIGN KEY REFERENCES Games(GameID),
    ActivationCode VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Coupons (
    CouponID    INT IDENTITY(1,1) PRIMARY KEY,
    Code        VARCHAR(30) NOT NULL UNIQUE,
    DiscountPct INT NOT NULL,
    ExpiryDate  DATE,
    IsActive    BIT NOT NULL DEFAULT 1
);
GO

/* ---------- reporting view ---------- */
CREATE VIEW vw_RevenueByGenre AS
SELECT ge.GenreName,
       SUM(od.Subtotal)          AS Revenue,
       COUNT(DISTINCT o.OrderID) AS Orders
FROM   OrderDetails od
JOIN   Games  g  ON od.GameID  = g.GameID
JOIN   Genres ge ON g.GenreID  = ge.GenreID
JOIN   Orders o  ON od.OrderID = o.OrderID
WHERE  o.Status = 'Paid'
GROUP BY ge.GenreName;
GO

/* =========================================================
   SEED DATA  --  login accounts (username / password):
     admin / admin123    staff / staff123
     customer / cust123   nabila / nabila123
   Passwords are stored as SHA-256 (lower-case hex).
   ========================================================= */
INSERT INTO Users (Username, PasswordHash, Role, FullName, Email, Phone) VALUES
('superadmin','4e4c56e4a15f89f05c2f4c72613da2a18c9665d4f0d6acce16415eb06f9be776', 'SuperAdmin', 'GameHub Owner',        'owner@gamehub.com',  '01700000009'),
('admin',   '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin',    'System Administrator', 'admin@gamehub.com',  '01700000000'),
('staff',   '10176e7b7b24d317acfcf8d2064cfd2f24e154f7b5a96603077d5ef813d6a6b6', 'Staff',    'Store Staff',          'staff@gamehub.com',  '01700000001'),
('customer','4f21b18a4c743a5da01bb3a4955dea0a0294a0b4f7977b454c7259e37b2e6c19', 'Customer', 'Rakib Hasan',          'rakib@gamehub.com',  '01700000002'),
('nabila',  'edd99d048235c3c0cfc4ad913bb3ee81587314f9e93e85b6fb32c76069156f9d', 'Customer', 'Nabila Rahman',        'nabila@gamehub.com', '01700000003');

INSERT INTO Genres (GenreName) VALUES
('Action RPG'),('Roguelike'),('Sports'),('Metroidvania'),('RPG'),('Simulation'),('Action');

INSERT INTO Publishers (PublisherName, Country) VALUES
('FromSoftware','Japan'),('Supergiant Games','USA'),('EA Sports','USA'),
('Team Cherry','Australia'),('CD Projekt Red','Poland'),('ConcernedApe','USA'),('Sony','Japan');

INSERT INTO Games (Title, Description, GenreID, PublisherID, Price, StockQuantity, ReleaseDate) VALUES
('Elden Ring',     'Open-world action RPG from FromSoftware.',       1, 1, 3999.00, 42, '2022-02-25'),
('Hades II',       'Rogue-like dungeon crawler by Supergiant.',      2, 2, 2499.00, 30, '2024-05-06'),
('EA FC 25',       'The latest football simulation from EA Sports.', 3, 3, 5499.00, 18, '2024-09-27'),
('Hollow Knight',  'Award-winning metroidvania adventure.',          4, 4, 1299.00, 55, '2017-02-24'),
('Cyberpunk 2077', 'Open-world RPG set in Night City.',              5, 5, 3299.00, 12, '2020-12-10'),
('Stardew Valley', 'Relaxing farming and life simulation.',          6, 6,  999.00, 80, '2016-02-26'),
('God of War',     'Cinematic action-adventure epic.',               7, 7, 4499.00, 25, '2018-04-20'),
('Baldurs Gate 3', 'Story-rich party-based RPG.',                    5, 5, 4999.00, 20, '2023-08-03');

INSERT INTO SubscriptionPlans (PlanName, MonthlyPrice, DiscountPct, FreeGamesPerMonth) VALUES
('Basic', 199.00, 5, 0),('Plus', 399.00, 10, 1),('Ultimate', 699.00, 15, 2);

INSERT INTO Coupons (Code, DiscountPct, ExpiryDate, IsActive) VALUES
('WELCOME10', 10, '2026-12-31', 1),('EIDSALE20', 20, '2026-12-31', 1);

INSERT INTO Wallet (UserID, Balance, LoyaltyPoints)
SELECT UserID, 2450.00, 1280 FROM Users WHERE Role = 'Customer';
GO

PRINT 'GameHubDB created and seeded successfully.';

/* =========================================================
   SUPER ADMIN FEATURES: system log + owner settings
   ========================================================= */
IF OBJECT_ID('SystemLog','U') IS NOT NULL DROP TABLE SystemLog;
CREATE TABLE SystemLog (
    LogID     INT IDENTITY(1,1) PRIMARY KEY,
    UserID    INT NULL,
    Username  VARCHAR(100),
    Action    VARCHAR(100) NOT NULL,
    Details   VARCHAR(400),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

IF OBJECT_ID('AppSettings','U') IS NOT NULL DROP TABLE AppSettings;
CREATE TABLE AppSettings (
    SettingKey   VARCHAR(50) PRIMARY KEY,
    SettingValue VARCHAR(200)
);

INSERT INTO AppSettings (SettingKey, SettingValue) VALUES
('ShopName', 'GameHub'),
('Currency', 'Tk'),
('VatPercent', '5'),
('DefaultDiscount', '0'),
('MaintenanceMode', 'false');
GO
