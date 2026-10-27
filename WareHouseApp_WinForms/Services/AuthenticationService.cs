using System;
using System.Data.SQLite;
using WareHouseApp.Data;
using WareHouseApp.Models;

namespace WareHouseApp.Services
{
    /// <summary>
    /// Service for User Login and Sign Up.
    /// This service authenticates user credentials and registers new users.
    /// </summary>
    public class AuthenticationService
    {
        public static Person CurrentLoggedInUser { get; private set; }

        public bool Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Username, Password, FullName, Email, Phone, Role FROM Users WHERE Username = @u;";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username.Trim());
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbPassword = reader["Password"].ToString();
                                string role = reader["Role"].ToString();

                                if (dbPassword == password)
                                {
                                    if (role == "Admin")
                                    {
                                        CurrentLoggedInUser = new Admin(
                                            Convert.ToInt32(reader["Id"]),
                                            reader["Username"].ToString(),
                                            dbPassword,
                                            reader["FullName"].ToString(),
                                            reader["Email"].ToString(),
                                            reader["Phone"].ToString(),
                                            150000m
                                        );
                                    }
                                    else
                                    {
                                        CurrentLoggedInUser = new Employee(
                                            Convert.ToInt32(reader["Id"]),
                                            reader["Username"].ToString(),
                                            dbPassword,
                                            reader["FullName"].ToString(),
                                            reader["Email"].ToString(),
                                            reader["Phone"].ToString(),
                                            "Staff",
                                            "Warehouse Operations",
                                            65000m
                                        );
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Authentication error: {ex.Message}", ex);
            }
        }

        public bool SignUp(string username, string password, string fullName, string email, string phone, string role = "Employee")
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty.");
            if (password.Length < 4) throw new ArgumentException("Password must contain at least 4 characters.");

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    // Check duplicate
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @u;";
                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@u", username.Trim());
                        if ((long)checkCmd.ExecuteScalar() > 0)
                        {
                            throw new InvalidOperationException($"Username '{username}' is already taken.");
                        }
                    }

                    // Insert User
                    string insertUser = @"
                        INSERT INTO Users (Username, Password, FullName, Email, Phone, Role)
                        VALUES (@u, @p, @fn, @e, @ph, @r);";
                    using (var cmd = new SQLiteCommand(insertUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username.Trim());
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.Parameters.AddWithValue("@fn", fullName.Trim());
                        cmd.Parameters.AddWithValue("@e", email.Trim());
                        cmd.Parameters.AddWithValue("@ph", phone.Trim());
                        cmd.Parameters.AddWithValue("@r", role);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert matching Employee record
                    string insertEmp = @"
                        INSERT INTO Employees (Username, Password, FullName, Email, Phone, Position, Department, Salary, HireDate)
                        VALUES (@u, @p, @fn, @e, @ph, 'Staff', 'Warehouse Operations', 60000.0, datetime('now'));";
                    using (var cmdEmp = new SQLiteCommand(insertEmp, conn))
                    {
                        cmdEmp.Parameters.AddWithValue("@u", username.Trim());
                        cmdEmp.Parameters.AddWithValue("@p", password);
                        cmdEmp.Parameters.AddWithValue("@fn", fullName.Trim());
                        cmdEmp.Parameters.AddWithValue("@e", email.Trim());
                        cmdEmp.Parameters.AddWithValue("@ph", phone.Trim());
                        cmdEmp.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Sign-up error: {ex.Message}", ex);
            }
        }

        public static void Logout()
        {
            CurrentLoggedInUser = null;
        }
    }
}
