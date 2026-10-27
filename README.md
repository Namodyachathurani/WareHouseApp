# Warehouse Inventory and Management System (WareHouseApp)

A modular, four-tier desktop application built in C# and Microsoft .NET for inventory control, customer relations, employee management, and role-based authentication.

---

## 1. System Overview

WareHouseApp is an enterprise desktop solution developed for warehouse logistics and stock control. The application demonstrates object-oriented programming (OOP) principles, defensive exception handling, data persistence, and graphical user interface architecture.

### Key Capabilities
- **Authentication and Authorization**: Secure user registration, credential verification, and role-based access control (Administrator vs Warehouse Operator).
- **Inventory and Stock Management**: Product cataloging with SKU, category classification, purchase and selling price records, stock quantity tracking, and automated low-stock threshold alerts.
- **Customer Account Management**: Client profiles with address records, credit limit assignments, current balance tracking, and transaction credit validation.
- **Employee Directory and Payroll Administration**: Staff directory with department assignments, contact information, role classification, base salary tracking, and role-specific bonus computations.

---

## 2. Solution Architecture

The solution follows a structured four-tier architecture ensuring high cohesion and low coupling:

```
+-------------------------------------------------------------+
|                     Presentation Layer                      |
|   Avalonia UI Desktop (Cross-Platform) / Windows Forms UI   |
+-------------------------------------------------------------+
                              |
                              v
+-------------------------------------------------------------+
|                       Service Layer                         |
|   AuthenticationService, InventoryService, CustomerService  |
+-------------------------------------------------------------+
                              |
                              v
+-------------------------------------------------------------+
|                     Data Access Layer                       |
|   DatabaseHelper (SQLite Integration, SQL Schema & CRUD)   |
+-------------------------------------------------------------+
                              |
                              v
+-------------------------------------------------------------+
|                     Domain Models Layer                     |
|   User, Product, Customer, Employee (OOP Inheritance)       |
+-------------------------------------------------------------+
```

### Object-Oriented Principles Applied
1. **Encapsulation**: Private field storage with public getter/setter properties, parameter validation, and controlled state mutations.
2. **Inheritance**: Base entity definitions (`IEntity`, `BaseEntity`) and hierarchical class models (`Employee` base class extended by `WarehouseManager` and `WarehouseStaff`).
3. **Polymorphism**: Abstract methods and virtual overrides for calculating net payroll allowances, and interface-based repository contracts (`IRepository<T>`).
4. **Abstraction**: Distinct separation of UI presentation from service business logic and persistent storage operations.

---

## 3. Project Structure

```
WareHouseApp/
|-- WareHouseApp.sln                     # Master Visual Studio Solution
|-- WareHouseApp_Desktop/                # Modern Cross-Platform Desktop Project (.NET 10)
|   |-- App.axaml                        # Application configuration and themes
|   |-- App.axaml.cs                     # Application lifecycle entrypoint
|   |-- MainWindow.axaml                 # Main desktop UI layout (Avalonia UI XAML)
|   |-- MainWindow.axaml.cs              # UI event handlers and view logic
|   |-- Models.cs                        # Domain entities and OOP inheritance hierarchy
|   |-- Services.cs                      # Business service operations and validations
|   |-- DatabaseHelper.cs                # SQLite database management and automated schema setup
|   |-- Program.cs                       # Application entrypoint
|   |-- WareHouseApp_Desktop.csproj      # Project build file
|   `-- app.manifest                     # Execution manifest
|-- WareHouseApp_WinForms/               # Windows Forms Project (.NET Framework)
|   |-- DashBoard.cs                     # Tabular dashboard interface
|   |-- Form1.cs                         # Authentication and sign-up form
|   |-- MainDash.cs                      # Administrative operations screen
|   |-- Models/                          # Domain entity classes
|   |-- Services/                        # Business logic services
|   |-- Data/                            # Database connection helpers
|   `-- WareHouseApp.csproj              # Windows Forms build configuration
|-- .gitignore                           # Git ignore rules for build artifacts
`-- README.md                            # Technical documentation
```

---

## 4. Prerequisites and Build Instructions

### Prerequisites
- .NET SDK 8.0, 9.0, or 10.0
- macOS, Linux, or Windows

### Building the Desktop Application
Clone the repository:
```bash
git clone https://github.com/Namodyachathurani/WareHouseApp.git
cd WareHouseApp
```

Restore dependencies and build:
```bash
dotnet build WareHouseApp_Desktop/WareHouseApp_Desktop.csproj
```

Run the application:
```bash
dotnet run --project WareHouseApp_Desktop/WareHouseApp_Desktop.csproj
```

---

## 5. Demonstration and Verification

- The application automatically initializes an embedded SQLite database upon first execution.
- Default seeded data includes initial administrators, inventory items, customer accounts, and staff records.
- Input validation safeguards all numeric parameters, stock counts, and credit limits against invalid states.
