using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WareHouseApp.Data;
using WareHouseApp.Models;

namespace WareHouseApp.Services
{
    /// <summary>
    /// Service for Employee Management functions.
    /// This service executes CRUD operations on staff records.
    /// </summary>
    public class EmployeeService
    {
        public List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id, Username, Password, FullName, Email, Phone, Position, Department, Salary, HireDate, IsActive FROM Employees ORDER BY Id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var emp = new Employee(
                                Convert.ToInt32(reader["Id"]),
                                reader["Username"].ToString(),
                                reader["Password"].ToString(),
                                reader["FullName"].ToString(),
                                reader["Email"].ToString(),
                                reader["Phone"].ToString(),
                                reader["Position"].ToString(),
                                reader["Department"].ToString(),
                                Convert.ToDecimal(reader["Salary"])
                            );
                            emp.IsActive = Convert.ToInt32(reader["IsActive"]) == 1;
                            list.Add(emp);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load employees: {ex.Message}", ex);
            }
        }

        public void AddEmployee(Employee emp)
        {
            if (string.IsNullOrWhiteSpace(emp.Username)) throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(emp.FullName)) throw new ArgumentException("Full name cannot be empty.");

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Employees (Username, Password, FullName, Email, Phone, Position, Department, Salary, HireDate, IsActive)
                        VALUES (@u, @p, @fn, @em, @ph, @pos, @dept, @sal, @hd, @act);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", emp.Username.Trim());
                        cmd.Parameters.AddWithValue("@p", emp.Password);
                        cmd.Parameters.AddWithValue("@fn", emp.FullName.Trim());
                        cmd.Parameters.AddWithValue("@em", emp.Email?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@ph", emp.Phone?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@pos", emp.Position.Trim());
                        cmd.Parameters.AddWithValue("@dept", emp.Department.Trim());
                        cmd.Parameters.AddWithValue("@sal", emp.Salary);
                        cmd.Parameters.AddWithValue("@hd", emp.HireDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@act", emp.IsActive ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }

                    // Also mirror into Users table for system login
                    string sqlUser = @"
                        INSERT OR IGNORE INTO Users (Username, Password, FullName, Email, Phone, Role)
                        VALUES (@u, @p, @fn, @em, @ph, 'Employee');";
                    using (var cmdUser = new SQLiteCommand(sqlUser, conn))
                    {
                        cmdUser.Parameters.AddWithValue("@u", emp.Username.Trim());
                        cmdUser.Parameters.AddWithValue("@p", emp.Password);
                        cmdUser.Parameters.AddWithValue("@fn", emp.FullName.Trim());
                        cmdUser.Parameters.AddWithValue("@em", emp.Email?.Trim() ?? "");
                        cmdUser.Parameters.AddWithValue("@ph", emp.Phone?.Trim() ?? "");
                        cmdUser.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add employee: {ex.Message}", ex);
            }
        }

        public void UpdateEmployee(Employee emp)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Employees 
                        SET FullName = @fn, Email = @em, Phone = @ph, Position = @pos, 
                            Department = @dept, Salary = @sal, IsActive = @act
                        WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", emp.Id);
                        cmd.Parameters.AddWithValue("@fn", emp.FullName.Trim());
                        cmd.Parameters.AddWithValue("@em", emp.Email?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@ph", emp.Phone?.Trim() ?? "");
                        cmd.Parameters.AddWithValue("@pos", emp.Position.Trim());
                        cmd.Parameters.AddWithValue("@dept", emp.Department.Trim());
                        cmd.Parameters.AddWithValue("@sal", emp.Salary);
                        cmd.Parameters.AddWithValue("@act", emp.IsActive ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update employee: {ex.Message}", ex);
            }
        }

        public void DeleteEmployee(int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Employees WHERE Id = @id;";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete employee: {ex.Message}", ex);
            }
        }
    }
}
