using System;

namespace WareHouseApp.Models
{
    /// <summary>
    /// Abstract base class representing a human actor in the system.
    /// Demonstrates Abstraction and Encapsulation.
    /// This class stores common demographic and authentication properties.
    /// </summary>
    public abstract class Person : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        
        // Encapsulated password field with validation
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Password cannot be empty.");
                }
                _password = value;
            }
        }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Person() { }

        public Person(int id, string username, string password, string fullName, string email, string phone)
        {
            Id = id;
            Username = username;
            Password = password;
            FullName = fullName;
            Email = email;
            Phone = phone;
        }

        /// <summary>
        /// Polymorphic method to get the specific system role.
        /// Child classes override this method to return their role title.
        /// </summary>
        public abstract string GetRoleDescription();

        /// <summary>
        /// Authenticates the user credentials.
        /// </summary>
        public virtual bool Authenticate(string passwordInput)
        {
            return !string.IsNullOrEmpty(_password) && _password == passwordInput;
        }
    }
}
