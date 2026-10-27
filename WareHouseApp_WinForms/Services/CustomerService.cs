using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WareHouseApp.Data;
using WareHouseApp.Models;

namespace WareHouseApp.Services
{
    /// <summary>
    /// Service for Customer Management functions.
    /// This service executes CRUD operations on customers.
    /// </summary>
    public class CustomerService
    {
        public List<Customer> GetAllCustomers()
        {
            var list = new List<Customer>();
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id, CustomerCode, Name, ContactPerson, Phone, Email, Address, CreditLimit, CurrentBalance FROM Customers ORDER BY CustomerCode;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var c = new Customer(
                                Convert.ToInt32(reader["Id"]),
                                reader["CustomerCode"].ToString(),
                                reader["Name"].ToString(),
                                reader["ContactPerson"].ToString(),
                                reader["Phone"].ToString(),
                                reader["Email"].ToString(),
                                reader["Address"].ToString(),
                                Convert.ToDecimal(reader["CreditLimit"])
                            );
                            c.CurrentBalance = Convert.ToDecimal(reader["CurrentBalance"]);
                            list.Add(c);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load customers: {ex.Message}", ex);
            }
        }

        public void AddCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerCode)) throw new ArgumentException("Customer code cannot be empty.");
            if (string.IsNullOrWhiteSpace(customer.Name)) throw new ArgumentException("Customer name cannot be empty.");

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Customers (CustomerCode, Name, ContactPerson, Phone, Email, Address, CreditLimit, CurrentBalance)
                        VALUES (@code, @name, @cp, @ph, @em, @addr, @cl, @cb);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", customer.CustomerCode.Trim());
                        cmd.Parameters.AddWithValue("@name", customer.Name.Trim());
                        cmd.Parameters.AddWithValue("@cp", customer.ContactPerson?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@ph", customer.Phone?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@em", customer.Email?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@addr", customer.Address?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@cl", customer.CreditLimit);
                        cmd.Parameters.AddWithValue("@cb", customer.CurrentBalance);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add customer: {ex.Message}", ex);
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Customers 
                        SET Name = @name, ContactPerson = @cp, Phone = @ph, Email = @em, 
                            Address = @addr, CreditLimit = @cl, CurrentBalance = @cb
                        WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", customer.Id);
                        cmd.Parameters.AddWithValue("@name", customer.Name.Trim());
                        cmd.Parameters.AddWithValue("@cp", customer.ContactPerson?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@ph", customer.Phone?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@em", customer.Email?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@addr", customer.Address?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@cl", customer.CreditLimit);
                        cmd.Parameters.AddWithValue("@cb", customer.CurrentBalance);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update customer: {ex.Message}", ex);
            }
        }

        public void DeleteCustomer(int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Customers WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete customer: {ex.Message}", ex);
            }
        }
    }
}
