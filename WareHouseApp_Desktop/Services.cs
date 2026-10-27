using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace WareHouseApp_Desktop
{
    public class AuthenticationService
    {
        public Employee? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password must not be empty.");

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string query = "SELECT Id, Username, Name, Email, Phone, Department, Salary, IsAdmin, SecurityLevel FROM Users WHERE Username = @u AND Password = @p;";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@u", username.Trim());
            cmd.Parameters.AddWithValue("@p", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                bool isAdmin = reader.GetInt32(7) == 1;
                if (isAdmin)
                {
                    return new Admin(
                        reader.GetInt32(0),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(1),
                        reader.GetString(5),
                        Convert.ToDecimal(reader.GetDouble(6)),
                        reader.GetInt32(8)
                    );
                }
                else
                {
                    return new Employee(
                        reader.GetInt32(0),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(1),
                        reader.GetString(5),
                        Convert.ToDecimal(reader.GetDouble(6)),
                        false
                    );
                }
            }

            return null;
        }

        public bool RegisterUser(string username, string password, string name, string email, string phone, string department, decimal salary, bool isAdmin, int securityLevel)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string insertSql = @"
                INSERT INTO Users (Username, Password, Name, Email, Phone, Department, Salary, IsAdmin, SecurityLevel)
                VALUES (@u, @p, @n, @e, @ph, @d, @s, @a, @sl);
            ";
            using var cmd = new SqliteCommand(insertSql, connection);
            cmd.Parameters.AddWithValue("@u", username.Trim());
            cmd.Parameters.AddWithValue("@p", password);
            cmd.Parameters.AddWithValue("@n", name.Trim());
            cmd.Parameters.AddWithValue("@e", email.Trim());
            cmd.Parameters.AddWithValue("@ph", phone.Trim());
            cmd.Parameters.AddWithValue("@d", department.Trim());
            cmd.Parameters.AddWithValue("@s", Convert.ToDouble(salary));
            cmd.Parameters.AddWithValue("@a", isAdmin ? 1 : 0);
            cmd.Parameters.AddWithValue("@sl", securityLevel);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public class InventoryService
    {
        public List<InventoryItem> GetAllItems()
        {
            var list = new List<InventoryItem>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string query = "SELECT ItemCode, Description, Category, QuantityOnHand, SafetyThreshold, UnitPrice, WarehouseLocation FROM Inventory ORDER BY ItemCode;";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new InventoryItem
                {
                    ItemCode = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    Category = reader.GetString(2),
                    QuantityOnHand = reader.GetInt32(3),
                    SafetyThreshold = reader.GetInt32(4),
                    UnitPrice = Convert.ToDecimal(reader.GetDouble(5)),
                    WarehouseLocation = reader.GetString(6)
                });
            }

            return list;
        }

        public bool AddItem(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.ItemCode <= 0) throw new ArgumentException("Item code must be positive integer.");
            if (string.IsNullOrWhiteSpace(item.Description)) throw new ArgumentException("Description cannot be empty.");

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string sql = @"
                INSERT INTO Inventory (ItemCode, Description, Category, QuantityOnHand, SafetyThreshold, UnitPrice, WarehouseLocation)
                VALUES (@c, @d, @cat, @q, @st, @p, @loc);
            ";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@c", item.ItemCode);
            cmd.Parameters.AddWithValue("@d", item.Description.Trim());
            cmd.Parameters.AddWithValue("@cat", item.Category.Trim());
            cmd.Parameters.AddWithValue("@q", item.QuantityOnHand);
            cmd.Parameters.AddWithValue("@st", item.SafetyThreshold);
            cmd.Parameters.AddWithValue("@p", Convert.ToDouble(item.UnitPrice));
            cmd.Parameters.AddWithValue("@loc", item.WarehouseLocation.Trim());

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateStockQuantity(int itemCode, int newQuantity)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string sql = "UPDATE Inventory SET QuantityOnHand = @q WHERE ItemCode = @c;";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@q", newQuantity);
            cmd.Parameters.AddWithValue("@c", itemCode);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteItem(int itemCode)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string sql = "DELETE FROM Inventory WHERE ItemCode = @c;";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@c", itemCode);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public class CustomerService
    {
        public List<Customer> GetAllCustomers()
        {
            var list = new List<Customer>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string query = "SELECT Id, Name, Email, Phone, CompanyName, Address, AnnualTurnover, CreditLimit, CurrentBalance FROM Customers ORDER BY Id;";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Customer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Phone = reader.GetString(3),
                    CompanyName = reader.GetString(4),
                    Address = reader.GetString(5),
                    AnnualTurnover = Convert.ToDecimal(reader.GetDouble(6)),
                    CreditLimit = Convert.ToDecimal(reader.GetDouble(7)),
                    CurrentBalance = Convert.ToDecimal(reader.GetDouble(8))
                });
            }

            return list;
        }

        public bool AddCustomer(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            if (string.IsNullOrWhiteSpace(customer.Name)) throw new ArgumentException("Customer name required.");

            customer.CreditLimit = Customer.CalculateCreditLimit(customer.AnnualTurnover);

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string sql = @"
                INSERT INTO Customers (Name, Email, Phone, CompanyName, Address, AnnualTurnover, CreditLimit, CurrentBalance)
                VALUES (@n, @e, @ph, @co, @a, @t, @cl, @cb);
            ";
            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@n", customer.Name.Trim());
            cmd.Parameters.AddWithValue("@e", customer.Email.Trim());
            cmd.Parameters.AddWithValue("@ph", customer.Phone.Trim());
            cmd.Parameters.AddWithValue("@co", customer.CompanyName.Trim());
            cmd.Parameters.AddWithValue("@a", customer.Address.Trim());
            cmd.Parameters.AddWithValue("@t", Convert.ToDouble(customer.AnnualTurnover));
            cmd.Parameters.AddWithValue("@cl", Convert.ToDouble(customer.CreditLimit));
            cmd.Parameters.AddWithValue("@cb", Convert.ToDouble(customer.CurrentBalance));

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public class EmployeeService
    {
        public List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            string query = "SELECT Id, Username, Name, Email, Phone, Department, Salary, IsAdmin, SecurityLevel FROM Users ORDER BY Id;";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                bool isAdmin = reader.GetInt32(7) == 1;
                if (isAdmin)
                {
                    list.Add(new Admin(
                        reader.GetInt32(0),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(1),
                        reader.GetString(5),
                        Convert.ToDecimal(reader.GetDouble(6)),
                        reader.GetInt32(8)
                    ));
                }
                else
                {
                    list.Add(new Employee(
                        reader.GetInt32(0),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(1),
                        reader.GetString(5),
                        Convert.ToDecimal(reader.GetDouble(6)),
                        false
                    ));
                }
            }

            return list;
        }
    }
}
