using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Represents a warehouse staff member.
    /// Demonstrates Inheritance (inherits Person).
    /// This class adds employment details to the base Person model.
    /// </summary>
    public class Employee : Person
    {
        public string Position { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }

        public Employee()
        {
            HireDate = DateTime.Now;
            IsActive = true;
        }

        public Employee(int id, string username, string password, string fullName, string email, string phone,
                        string position, string department, decimal salary)
            : base(id, username, password, fullName, email, phone)
        {
            Position = position;
            Department = department;
            Salary = salary;
            HireDate = DateTime.Now;
            IsActive = true;
        }

        /// <summary>
        /// Polymorphic override of role description.
        /// </summary>
        public override string GetRoleDescription()
        {
            return $"Warehouse Employee - Position: {Position} ({Department})";
        }
    }
}
