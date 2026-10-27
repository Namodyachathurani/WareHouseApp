using System;

namespace WareHouseApp_Desktop
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        protected Person() { }

        protected Person(int id, string name, string email, string phone)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
            RegisteredDate = DateTime.UtcNow;
        }

        public abstract string GetRoleDescription();

        public virtual string GetSummary()
        {
            return $"{Name} (Email: {Email}, Phone: {Phone})";
        }
    }

    public class Employee : Person
    {
        public string Username { get; set; } = string.Empty;
        public string Department { get; set; } = "Operations";
        public decimal Salary { get; set; }
        public bool IsAdminRole { get; set; }

        public Employee() { }

        public Employee(int id, string name, string email, string phone, string username, string department, decimal salary, bool isAdminRole = false)
            : base(id, name, email, phone)
        {
            Username = username;
            Department = department;
            Salary = salary;
            IsAdminRole = isAdminRole;
        }

        public override string GetRoleDescription()
        {
            return IsAdminRole ? "Warehouse Administrator" : "Warehouse Staff Member";
        }

        public virtual bool CanModifySystemSettings()
        {
            return IsAdminRole;
        }
    }

    public class Admin : Employee
    {
        public int SecurityLevel { get; set; } = 3;

        public Admin()
        {
            IsAdminRole = true;
        }

        public Admin(int id, string name, string email, string phone, string username, string department, decimal salary, int securityLevel = 3)
            : base(id, name, email, phone, username, department, salary, true)
        {
            SecurityLevel = securityLevel;
        }

        public override string GetRoleDescription()
        {
            return $"Warehouse Administrator (Level {SecurityLevel})";
        }

        public override bool CanModifySystemSettings()
        {
            return true;
        }
    }

    public class Customer : Person
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AnnualTurnover { get; set; }

        public Customer() { }

        public Customer(int id, string name, string email, string phone, string companyName, string address, decimal annualTurnover)
            : base(id, name, email, phone)
        {
            CompanyName = companyName;
            Address = address;
            AnnualTurnover = annualTurnover;
            CreditLimit = CalculateCreditLimit(annualTurnover);
            CurrentBalance = 0.0m;
        }

        public static decimal CalculateCreditLimit(decimal annualTurnover)
        {
            if (annualTurnover >= 1000000m)
                return 150000m;
            if (annualTurnover >= 500000m)
                return 75000m;
            if (annualTurnover >= 100000m)
                return 25000m;
            return 10000m;
        }

        public override string GetRoleDescription()
        {
            return "Commercial Customer";
        }

        public bool CanPlaceOrder(decimal orderValue)
        {
            return (CurrentBalance + orderValue) <= CreditLimit;
        }

        public decimal AvailableCredit => Math.Max(0.0m, CreditLimit - CurrentBalance);
    }

    public class InventoryItem
    {
        public int ItemCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public int QuantityOnHand { get; set; }
        public int SafetyThreshold { get; set; } = 15;
        public decimal UnitPrice { get; set; }
        public string WarehouseLocation { get; set; } = "A-01";

        public bool IsLowStock => QuantityOnHand <= SafetyThreshold;
        public decimal TotalValuation => QuantityOnHand * UnitPrice;
    }
}
