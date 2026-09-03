# GameHub — Game Shop Management System

**GameHub** is a Windows desktop game-shop management application developed in **C# WinForms** with **Microsoft SQL Server**. The system supports role-based access for **Super Admin, Admin, Staff, and Customer** users and combines store management, digital game purchasing, wallet and subscription features, reporting, and system administration in one desktop application.

> **Course:** CSC 2210 — Object Oriented Programming 2  
> **Platform:** Windows Desktop  
> **Framework:** .NET Framework 4.7.2

---

## Project Overview

GameHub is designed to digitize the main activities of a game shop. Customers can create accounts, browse games, purchase digital copies, receive activation keys, manage a wishlist and library, use an in-app wallet, subscribe to GameHub Pass, and review purchased games.

Management users are provided with separate role-based dashboards for inventory, orders, payments, users, reports, customer support, settings, and system activity.

The project demonstrates core **Object-Oriented Programming (OOP)** concepts together with a normalized relational database and layered application architecture.

---

## Main Features

### Customer Features

- Secure login and customer registration
- Username/email-based authentication
- Password recovery and password reset
- Steam-style customer dashboard
- Browse and search the game catalogue
- Filter games by genre
- View game details
- Add games to cart and checkout
- Multiple payment methods
- In-app wallet and wallet top-up
- Loyalty points
- GameHub Pass subscriptions
- Automatic subscription discounts
- Digital activation-key generation after purchase
- Personal game library
- Wishlist management
- Game ratings and reviews
- Customer profile management

### Staff Features

- Staff dashboard
- Inventory/game management
- Order management
- Customer support tools
- Search and operational access required for day-to-day shop activities

### Admin Features

- Admin dashboard with shop statistics
- Game catalogue and inventory CRUD operations
- Order management
- Payment management
- User and staff account management
- Sales reports and analytics
- Revenue-by-genre reporting

### Super Admin Features

The **Super Admin** is the highest-level system role and has access to all administrator functionality plus owner-level controls.

- Full system access
- Create and manage Admin/Staff/Customer accounts
- Manage privileged accounts
- Owner settings
- View system activity/audit log
- Configure shop name
- Configure currency
- Configure VAT percentage
- Configure default discount
- Configure maintenance mode

---

## User Roles

| Role | Main Access |
|---|---|
| **Super Admin** | Dashboard, Games, Orders, Payments, Users & Staff, Reports, Settings, System Log |
| **Admin** | Dashboard, Games, Orders, Payments, Users & Staff, Reports |
| **Staff** | Dashboard, Inventory, Orders, Customer Support |
| **Customer** | Home, Store, Library, Wishlist, Wallet, Subscription, Profile |

---

## OOP Concepts Used

The project was designed to demonstrate the major OOP principles required for the course.

### Encapsulation

Application data and database operations are organized inside model, repository, service, and form classes. Database access is handled through dedicated data-access classes rather than being directly exposed throughout the UI.

### Inheritance

All system roles inherit from the abstract `User` base class:

```text
User
├── SuperAdmin
├── Admin
├── Staff
└── Customer
```

### Abstraction

`User` is an abstract class that defines common user information and role-specific behavior. `DatabaseConnection` also abstracts ADO.NET database connection and command handling.

### Polymorphism

Each user type overrides role-specific members such as:

```csharp
Role
GetHomeForm()
```

After login, the system can therefore route different user types to the correct dashboard using the same base-class reference.

---

## Technology Stack

| Area | Technology |
|---|---|
| Programming Language | C# |
| Framework | .NET Framework 4.7.2 |
| Desktop UI | Windows Forms (WinForms) |
| Database | Microsoft SQL Server |
| Database Tool | SQL Server Management Studio (SSMS) |
| Data Access | ADO.NET / `System.Data.SqlClient` |
| IDE | Visual Studio 2022 |
| Password Storage | SHA-256 hashing |
| Architecture | Models + Data/Repositories + Forms/Views + Services |

No external NuGet package is required by the current project.

---

## Project Architecture

```text
GameHub/
│
├── Assets/
│   └── Games/                 # Game covers, banners and screenshots
│
├── Data/
│   ├── AuditRepository.cs
│   ├── CustomerRepository.cs
│   ├── DatabaseConnection.cs
│   ├── GameRepository.cs
│   ├── OrderRepository.cs
│   ├── ReportRepository.cs
│   ├── Security.cs
│   ├── SettingsRepository.cs
│   ├── SubscriptionRepository.cs
│   ├── UserRepository.cs
│   └── WalletRepository.cs
│
├── Database/
│   └── schema.sql             # Database creation and seed script
│
├── Forms/
│   ├── Controls/              # Reusable WinForms controls
│   ├── Views/                 # Customer dashboard views
│   ├── LoginForm.cs
│   ├── RegisterForm.cs
│   ├── CustomerDashboardForm.cs
│   ├── DashboardForm.cs
│   ├── StoreForm.cs
│   ├── CheckoutForm.cs
│   ├── ManageGamesForm.cs
│   ├── OrdersForm.cs
│   ├── PaymentsForm.cs
│   ├── UsersForm.cs
│   ├── ReportsForm.cs
│   ├── AuditLogForm.cs
│   ├── SettingsForm.cs
│   └── ...
│
├── Models/
│   ├── User.cs
│   ├── SuperAdmin.cs
│   ├── Admin.cs
│   ├── Staff.cs
│   ├── Customer.cs
│   ├── Game.cs
│   ├── CartItem.cs
│   └── SubscriptionPlan.cs
│
├── Services/
│   ├── GameAssetService.cs
│   └── ImageCacheService.cs
│
├── App.config
├── Program.cs
└── GameHub.csproj
```

---

## Database Design

The application uses the **GameHubDB** SQL Server database.

### Database Tables

The current database contains **17 tables**:

1. `Users`
2. `Genres`
3. `Publishers`
4. `Games`
5. `Orders`
6. `OrderDetails`
7. `Payments`
8. `SubscriptionPlans`
9. `Subscriptions`
10. `Wallet`
11. `WalletTransactions`
12. `Wishlist`
13. `Reviews`
14. `GameKeys`
15. `Coupons`
16. `SystemLog`
17. `AppSettings`

The database also contains the reporting view:

```text
vw_RevenueByGenre
```

The schema uses primary keys and foreign-key relationships to connect users, games, purchases, payments, subscriptions, wallets, reviews, and other system data.

---

## Important Database Relationships

```text
Users ─────< Orders
Users ─────< Subscriptions >───── SubscriptionPlans
Users ────── Wallet ─────< WalletTransactions
Users ─────< Wishlist >────────── Games
Users ─────< Reviews >─────────── Games

Genres ────< Games >───────────── Publishers
Games ─────< OrderDetails >────── Orders
Orders ───── Payments
Orders ────< GameKeys >────────── Games
```

`SystemLog` stores system activity information, while `AppSettings` stores owner-configurable application settings.

---

## Checkout and Order Processing

The checkout process uses a SQL transaction so related operations remain consistent. During a successful purchase the application can:

1. Validate game stock.
2. Validate and charge the wallet when Wallet payment is selected.
3. Create the order.
4. Create order-detail records.
5. Reduce game stock.
6. Generate unique activation keys.
7. Record the payment.
8. Award loyalty points.
9. Commit all operations together.

If a database operation fails, the transaction is rolled back.

---

## Installation and Setup

### Prerequisites

Before running GameHub, install:

- Windows 10/11
- Visual Studio 2022
- .NET Framework 4.7.2 Developer/Targeting Pack
- Microsoft SQL Server
- SQL Server Management Studio (SSMS)

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd GameHub
```

You can also download the repository as a ZIP file and extract it.

### 2. Create the Database

Open **SQL Server Management Studio (SSMS)**.

Open:

```text
Database/schema.sql
```

Run the complete script.

The script will:

- Create `GameHubDB`
- Create all required tables
- Create the reporting view
- Insert sample users
- Insert sample games
- Insert subscription plans
- Insert coupons
- Insert initial application settings

### 3. Configure the SQL Server Connection

Open:

```text
App.config
```

Find the `GameHubDB` connection string:

```xml
<connectionStrings>
    <add name="GameHubDB"
         connectionString="Data Source=YOUR_SQL_SERVER;Initial Catalog=GameHubDB;Integrated Security=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

Replace:

```text
YOUR_SQL_SERVER
```

with your SQL Server instance name.

Examples:

```text
.
localhost
DESKTOP-NAME\SQLEXPRESS
```

### 4. Open the Project

Open:

```text
GameHub.csproj
```

in **Visual Studio 2022**.

### 5. Build and Run

In Visual Studio:

```text
Build → Build Solution
```

Then press:

```text
F5
```

or select **Start**.

---

## Demo Accounts

The database seed script creates the following accounts for testing.

| Role | Username | Password |
|---|---|---|
| Super Admin | `superadmin` | `super123` |
| Admin | `admin` | `admin123` |
| Staff | `staff` | `staff123` |
| Customer | `customer` | `cust123` |
| Customer | `nabila` | `nabila123` |

> These accounts are included only as demonstration/academic seed data.

---

## GameHub Pass

GameHub includes three subscription plans stored in the database.

| Plan | Monthly Price | Discount | Free Games / Month |
|---|---:|---:|---:|
| Basic | Tk 199 | 5% | 0 |
| Plus | Tk 399 | 10% | 1 |
| Ultimate | Tk 699 | 15% | 2 |

Active subscription discounts can be applied during checkout.

---

## Security and Validation

The project includes:

- Password hashing using SHA-256
- Parameterized SQL queries
- Role-based access control
- Active/inactive user checking
- Required-field validation
- Email/account validation
- Numeric input validation
- Exception handling around database operations
- SQL transactions for checkout operations
- System activity logging

> **Academic-project note:** The current implementation uses SHA-256 password hashing as required/implemented in this project. A production application should use a dedicated salted password-hashing algorithm such as Argon2, bcrypt, or PBKDF2.

---

## Reporting

Admin and Super Admin users can access reporting features including:

- Total games
- Total orders
- Total revenue
- Active users
- Revenue by genre

The database view `vw_RevenueByGenre` is used for genre-based revenue reporting.

---

## Repository Notes

For a clean GitHub submission, generated Visual Studio files should not be committed. The repository should normally exclude:

```text
.vs/
bin/
obj/
*.user
*.suo
```

Keep the source code, database script, assets, documentation, and project file in the repository.

---

## Academic Purpose

GameHub was developed as an **Object Oriented Programming 2** academic project. Its purpose is to demonstrate practical use of:

- Object-oriented design
- WinForms GUI development
- Role-based systems
- CRUD operations
- SQL Server relational database design
- ADO.NET
- Authentication
- Transaction processing
- Reporting and analytics

---

## License

This repository is intended for **academic and educational use**. If a separate license is required for the GitHub repository, add a `LICENSE` file before publication.

---

## Project Status

**Completed academic project / submission version**

