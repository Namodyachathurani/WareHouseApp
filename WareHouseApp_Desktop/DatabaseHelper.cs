using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace WareHouseApp_Desktop
{
    public static class DatabaseHelper
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "warehouse.db");
        public static string ConnectionString => $"Data Source={DbPath}";

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string createTablesSql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    Password TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    Department TEXT NOT NULL,
                    Salary REAL NOT NULL,
                    IsAdmin INTEGER NOT NULL DEFAULT 0,
                    SecurityLevel INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Inventory (
                    ItemCode INTEGER PRIMARY KEY,
                    Description TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    QuantityOnHand INTEGER NOT NULL,
                    SafetyThreshold INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    WarehouseLocation TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    CompanyName TEXT NOT NULL,
                    Address TEXT NOT NULL,
                    AnnualTurnover REAL NOT NULL,
                    CreditLimit REAL NOT NULL,
                    CurrentBalance REAL NOT NULL DEFAULT 0
                );
            ";

            using (var cmd = new SqliteCommand(createTablesSql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            // Seed Users
            string checkUsers = "SELECT COUNT(*) FROM Users;";
            using (var cmd = new SqliteCommand(checkUsers, connection))
            {
                long count = (long)(cmd.ExecuteScalar() ?? 0L);
                if (count == 0)
                {
                    string seedUsers = @"
                        INSERT INTO Users (Username, Password, Name, Email, Phone, Department, Salary, IsAdmin, SecurityLevel)
                        VALUES 
                        ('admin', 'admin123', 'System Administrator', 'admin@warehouse.local', '+94 11 234 5678', 'Executive', 185000, 1, 3),
                        ('operator', 'op123', 'Warehouse Operator', 'operator@warehouse.local', '+94 11 876 5432', 'Logistics', 95000, 0, 1);
                    ";
                    using var seedCmd = new SqliteCommand(seedUsers, connection);
                    seedCmd.ExecuteNonQuery();
                }
            }

            // Seed Inventory
            string checkInv = "SELECT COUNT(*) FROM Inventory;";
            using (var cmd = new SqliteCommand(checkInv, connection))
            {
                long count = (long)(cmd.ExecuteScalar() ?? 0L);
                if (count == 0)
                {
                    string seedInv = @"
                        INSERT INTO Inventory (ItemCode, Description, Category, QuantityOnHand, SafetyThreshold, UnitPrice, WarehouseLocation)
                        VALUES 
                        (1001, 'Industrial Steel Storage Shelving', 'Hardware', 45, 10, 18500, 'A-01'),
                        (1002, 'Heavy-Duty Hydraulic Pallet Jack 2.5T', 'Machinery', 8, 12, 145000, 'B-04'),
                        (1003, 'Polypropylene Packing Strapping Coil', 'Packaging', 150, 30, 4200, 'C-12'),
                        (1004, 'Direct Thermal Barcode Shipping Labels', 'Stationery', 6, 25, 2100, 'D-02'),
                        (1005, 'Warehouse Safety Bump Caps (Pack of 10)', 'Safety', 75, 15, 8900, 'E-08');
                    ";
                    using var seedCmd = new SqliteCommand(seedInv, connection);
                    seedCmd.ExecuteNonQuery();
                }
            }

            // Seed Customers
            string checkCust = "SELECT COUNT(*) FROM Customers;";
            using (var cmd = new SqliteCommand(checkCust, connection))
            {
                long count = (long)(cmd.ExecuteScalar() ?? 0L);
                if (count == 0)
                {
                    string seedCust = @"
                        INSERT INTO Customers (Name, Email, Phone, CompanyName, Address, AnnualTurnover, CreditLimit, CurrentBalance)
                        VALUES 
                        ('Sunil Perera', 'sunil@lankalogistics.lk', '+94 77 123 4567', 'Lanka Logistics PLC', '45 Nawam Mawatha, Colombo 02', 1500000, 150000, 42000),
                        ('Nimal Fernando', 'nimal@ceylonretail.com', '+94 71 456 7890', 'Ceylon Retail Distributors', '128 Galle Road, Colombo 03', 650000, 75000, 18500),
                        ('Anura Silva', 'anura@metrohardware.lk', '+94 76 987 6543', 'Metro Hardware Enterprises', '88 Kandy Road, Kelaniya', 220000, 25000, 24000);
                    ";
                    using var seedCmd = new SqliteCommand(seedCust, connection);
                    seedCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
