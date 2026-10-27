using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Represents a warehouse inventory SKU / material.
    /// Demonstrates Encapsulation and Domain Logic.
    /// This class tracks material stock levels and pricing.
    /// </summary>
    public class InventoryItem : IEntity
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public string WarehouseLocation { get; set; }
        public int MinimumThreshold { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity cannot be negative.");
                }
                _quantity = value;
            }
        }

        public InventoryItem()
        {
            MinimumThreshold = 10;
        }

        public InventoryItem(int id, string code, string name, string category, int qty, decimal price, string location, int minThreshold = 10)
        {
            Id = id;
            ItemCode = code;
            ItemName = name;
            Category = category;
            Quantity = qty;
            UnitPrice = price;
            WarehouseLocation = location;
            MinimumThreshold = minThreshold;
        }

        public bool IsLowStock() => Quantity <= MinimumThreshold;

        public void AddStock(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Addition amount must be positive.");
            Quantity += amount;
        }

        public void DeductStock(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Deduction amount must be positive.");
            if (amount > Quantity) throw new InvalidOperationException($"Insufficient stock. Available: {Quantity}, Requested: {amount}");
            Quantity -= amount;
        }
    }
}
