using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Manages the portable local SQLite database.
    /// This class creates the schema and manages database connections.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WarehouseLocal.db");
        public static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public static void InitializeDatabase()
        {
            try
            {
                bool isNew = !File.Exists(DbPath);
                if (isNew)
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // 1. Users Table
                    string createUsersTable = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT UNIQUE NOT NULL,
                            Password TEXT NOT NULL,
                            FullName TEXT NOT NULL,
                            Email TEXT,
                            Phone TEXT,
                            Role TEXT NOT NULL
                        );";

                    // 2. Employees Table
                    string createEmployeesTable = @"
                        CREATE TABLE IF NOT EXISTS Employees (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT UNIQUE NOT NULL,
                            Password TEXT NOT NULL,
                            FullName TEXT NOT NULL,
                            Email TEXT,
                            Phone TEXT,
                            Position TEXT NOT NULL,
                            Department TEXT NOT NULL,
                            Salary REAL NOT NULL,
                            HireDate TEXT NOT NULL,
                            IsActive INTEGER NOT NULL DEFAULT 1
                        );";

                    // 3. Customers Table
                    string createCustomersTable = @"
                        CREATE TABLE IF NOT EXISTS Customers (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            CustomerCode TEXT UNIQUE NOT NULL,
                            Name TEXT NOT NULL,
                            ContactPerson TEXT,
                            Phone TEXT,
                            Email TEXT,
                            Address TEXT,
                            CreditLimit REAL NOT NULL,
                            CurrentBalance REAL NOT NULL DEFAULT 0
                        );";

                    // 4. Inventory Table
                    string createInventoryTable = @"
                        CREATE TABLE IF NOT EXISTS Inventory (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ItemCode TEXT UNIQUE NOT NULL,
                            ItemName TEXT NOT NULL,
                            Category TEXT NOT NULL,
                            Quantity INTEGER NOT NULL,
                            UnitPrice REAL NOT NULL,
                            WarehouseLocation TEXT,
                            MinimumThreshold INTEGER NOT NULL DEFAULT 10
                        );";

                    ExecuteNonQuery(conn, createUsersTable);
                    ExecuteNonQuery(conn, createEmployeesTable);
                    ExecuteNonQuery(conn, createCustomersTable);
                    ExecuteNonQuery(conn, createInventoryTable);

                    // Seed Default Administrator Account if missing
                    SeedDefaultData(conn);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection conn, string query)
        {
            using (var cmd = new SQLiteCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static void SeedDefaultData(SQLiteConnection conn)
        {
            // Seed Admin User
            string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username = 'Admin';";
            using (var cmd = new SQLiteCommand(checkAdmin, conn))
            {
                long count = (long)cmd.ExecuteScalar();
                if (count == 0)
                {
                    string insertAdmin = @"
                        INSERT INTO Users (Username, Password, FullName, Email, Phone, Role)
                        VALUES ('Admin', 'admin123', 'System Administrator', 'admin@warehouse.local', '0771234567', 'Admin');";
                    ExecuteNonQuery(conn, insertAdmin);

                    string insertAdminEmp = @"
                        INSERT INTO Employees (Username, Password, FullName, Email, Phone, Position, Department, Salary, HireDate)
                        VALUES ('Admin', 'admin123', 'System Administrator', 'admin@warehouse.local', '0771234567', 'IT Administrator', 'Administration', 150000.0, datetime('now'));";
                    ExecuteNonQuery(conn, insertAdminEmp);
                }
            }

            // Seed Sample Inventory
            string checkInv = "SELECT COUNT(*) FROM Inventory;";
            using (var cmd = new SQLiteCommand(checkInv, conn))
            {
                long count = (long)cmd.ExecuteScalar();
                if (count == 0)
                {
                    string insertInv = @"
                        INSERT INTO Inventory (ItemCode, ItemName, Category, Quantity, UnitPrice, WarehouseLocation, MinimumThreshold)
                        VALUES 
                        ('SKU-1001', 'Steel Fastener M8', 'Hardware', 500, 15.50, 'Aisle 1 - Bay B', 50),
                        ('SKU-1002', 'Copper Pipe 15mm', 'Plumbing', 120, 185.00, 'Aisle 2 - Bay A', 20),
                        ('SKU-1003', 'Circuit Breaker 16A', 'Electrical', 8, 450.00, 'Aisle 3 - Bay C', 15),
                        ('SKU-1004', 'Safety Gloves L', 'Safety Gear', 75, 85.00, 'Aisle 4 - Bay D', 25);";
                    ExecuteNonQuery(conn, insertInv);
                }
            }

            // Seed Sample Customers
            string checkCust = "SELECT COUNT(*) FROM Customers;";
            using (var cmd = new SQLiteCommand(checkCust, conn))
            {
                long count = (long)cmd.ExecuteScalar();
                if (count == 0)
                {
                    string insertCust = @"
                        INSERT INTO Customers (CustomerCode, Name, ContactPerson, Phone, Email, Address, CreditLimit, CurrentBalance)
                        VALUES 
                        ('CUST-001', 'Lanka Engineering Ltd', 'Kamal Perera', '0112345678', 'kamal@lankaeng.lk', '124 Colombo Road', 250000.0, 15000.0),
                        ('CUST-002', 'Apex Builders Consortium', 'Sunil Silva', '0339876543', 'sunil@apexbuilders.lk', '45 Kandy Road', 500000.0, 48000.0);";
                    ExecuteNonQuery(conn, insertCust);
                }
            }
        }
    }
}
