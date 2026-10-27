using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using WareHouseApp.Data;
using WareHouseApp.Models;

namespace WareHouseApp.Services
{
    /// <summary>
    /// Service for Inventory Management functions.
    /// This service executes CRUD operations on warehouse items.
    /// </summary>
    public class InventoryService
    {
        public List<InventoryItem> GetAllItems()
        {
            var list = new List<InventoryItem>();
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, ItemCode, ItemName, Category, Quantity, UnitPrice, WarehouseLocation, MinimumThreshold FROM Inventory ORDER BY ItemCode;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new InventoryItem(
                                Convert.ToInt32(reader["Id"]),
                                reader["ItemCode"].ToString(),
                                reader["ItemName"].ToString(),
                                reader["Category"].ToString(),
                                Convert.ToInt32(reader["Quantity"]),
                                Convert.ToDecimal(reader["UnitPrice"]),
                                reader["WarehouseLocation"].ToString(),
                                Convert.ToInt32(reader["MinimumThreshold"])
                            ));
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load inventory: {ex.Message}", ex);
            }
        }

        public void AddItem(InventoryItem item)
        {
            if (string.IsNullOrWhiteSpace(item.ItemCode)) throw new ArgumentException("Item code cannot be empty.");
            if (string.IsNullOrWhiteSpace(item.ItemName)) throw new ArgumentException("Item name cannot be empty.");

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Inventory (ItemCode, ItemName, Category, Quantity, UnitPrice, WarehouseLocation, MinimumThreshold)
                        VALUES (@c, @n, @cat, @q, @p, @loc, @min);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@c", item.ItemCode.Trim());
                        cmd.Parameters.AddWithValue("@n", item.ItemName.Trim());
                        cmd.Parameters.AddWithValue("@cat", item.Category.Trim());
                        cmd.Parameters.AddWithValue("@q", item.Quantity);
                        cmd.Parameters.AddWithValue("@p", item.UnitPrice);
                        cmd.Parameters.AddWithValue("@loc", item.WarehouseLocation.Trim());
                        cmd.Parameters.AddWithValue("@min", item.MinimumThreshold);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add inventory item: {ex.Message}", ex);
            }
        }

        public void UpdateItem(InventoryItem item)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Inventory 
                        SET ItemName = @n, Category = @cat, Quantity = @q, UnitPrice = @p, 
                            WarehouseLocation = @loc, MinimumThreshold = @min
                        WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", item.Id);
                        cmd.Parameters.AddWithValue("@n", item.ItemName.Trim());
                        cmd.Parameters.AddWithValue("@cat", item.Category.Trim());
                        cmd.Parameters.AddWithValue("@q", item.Quantity);
                        cmd.Parameters.AddWithValue("@p", item.UnitPrice);
                        cmd.Parameters.AddWithValue("@loc", item.WarehouseLocation.Trim());
                        cmd.Parameters.AddWithValue("@min", item.MinimumThreshold);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update inventory item: {ex.Message}", ex);
            }
        }

        public void DeleteItem(int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Inventory WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete inventory item: {ex.Message}", ex);
            }
        }
    }
}
