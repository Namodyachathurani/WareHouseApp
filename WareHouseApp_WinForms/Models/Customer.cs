using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Represents a commercial customer for warehouse shipments.
    /// Demonstrates Encapsulation and Business Rule Validation.
    /// This class stores customer account and credit details.
    /// </summary>
    public class Customer : IEntity
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }

        public Customer()
        {
            CreditLimit = 100000m;
            CurrentBalance = 0m;
        }

        public Customer(int id, string code, string name, string contact, string phone, string email, string address, decimal creditLimit)
        {
            Id = id;
            CustomerCode = code;
            Name = name;
            ContactPerson = contact;
            Phone = phone;
            Email = email;
            Address = address;
            CreditLimit = creditLimit;
            CurrentBalance = 0m;
        }

        /// <summary>
        /// Validates if customer can place a new order.
        /// </summary>
        public bool CanPlaceOrder(decimal orderAmount)
        {
            return (CurrentBalance + orderAmount) <= CreditLimit;
        }

        public void ChargeAccount(decimal amount)
        {
            if (!CanPlaceOrder(amount))
            {
                throw new InvalidOperationException("Transaction exceeds customer credit limit.");
            }
            CurrentBalance += amount;
        }
    }
}
