using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Represents an administrative user with elevated privileges.
    /// Demonstrates Multi-level Inheritance and Polymorphism.
    /// This class grants full administrative rights.
    /// </summary>
    public class Admin : Employee
    {
        public int SecurityLevel { get; set; }

        public Admin() : base()
        {
            Position = "System Administrator";
            Department = "IT & Operations";
            SecurityLevel = 1;
        }

        public Admin(int id, string username, string password, string fullName, string email, string phone, decimal salary)
            : base(id, username, password, fullName, email, phone, "System Administrator", "IT & Operations", salary)
        {
            SecurityLevel = 1;
        }

        public override string GetRoleDescription()
        {
            return $"Executive Administrator - Full System Privileges (Level {SecurityLevel})";
        }
    }
}
