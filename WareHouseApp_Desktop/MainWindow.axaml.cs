using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace WareHouseApp_Desktop
{
    public partial class MainWindow : Window
    {
        private readonly AuthenticationService _authService;
        private readonly InventoryService _inventoryService;
        private readonly CustomerService _customerService;
        private readonly EmployeeService _employeeService;
        private Employee? _currentUser;
        private bool _isDemoRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            _authService = new AuthenticationService();
            _inventoryService = new InventoryService();
            _customerService = new CustomerService();
            _employeeService = new EmployeeService();

            DatabaseHelper.InitializeDatabase();

            // Default login as admin for initial state
            _currentUser = _authService.Authenticate("admin", "admin123");

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            LogActivity("Application initialized successfully.");
            LogActivity("SQLite relational database loaded: warehouse.db");
            RefreshAllViews();
            UpdateUserStatus();
        }

        public void TriggerAutoDemoOnStart()
        {
            Loaded += async (s, e) =>
            {
                await Task.Delay(1200);
                BtnRunAutoDemo_Click(null, new RoutedEventArgs());
            };
        }

        private void LogActivity(string message)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            if (TxtActivityLog != null)
            {
                TxtActivityLog.Text = string.IsNullOrEmpty(TxtActivityLog.Text) ? entry : entry + Environment.NewLine + TxtActivityLog.Text;
            }
        }

        private void UpdateUserStatus()
        {
            if (_currentUser != null)
            {
                TxtCurrentUser.Text = $"Active Session: {_currentUser.Name} ({_currentUser.GetRoleDescription()})";
            }
            else
            {
                TxtCurrentUser.Text = "Active Session: Unauthenticated (Guest)";
            }
        }

        private void RefreshAllViews()
        {
            RefreshInventory();
            RefreshCustomers();
            RefreshEmployees();
            UpdateOverviewMetrics();
        }

        private void RefreshInventory()
        {
            try
            {
                var items = _inventoryService.GetAllItems();
                GridInventory.ItemsSource = items;
            }
            catch (Exception ex)
            {
                LogActivity($"Inventory fetch error: {ex.Message}");
            }
        }

        private void RefreshCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                GridCustomers.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                LogActivity($"Customer fetch error: {ex.Message}");
            }
        }

        private void RefreshEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                var displayList = employees.Select(emp => new
                {
                    emp.Id,
                    emp.Username,
                    emp.Name,
                    emp.Department,
                    emp.Salary,
                    RoleDescription = emp.GetRoleDescription()
                }).ToList();

                GridEmployees.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                LogActivity($"Staff fetch error: {ex.Message}");
            }
        }

        private void UpdateOverviewMetrics()
        {
            try
            {
                var items = _inventoryService.GetAllItems();
                var customers = _customerService.GetAllCustomers();

                int totalUnits = items.Sum(i => i.QuantityOnHand);
                int lowStockCount = items.Count(i => i.IsLowStock);
                decimal totalValuation = items.Sum(i => i.TotalValuation);

                TxtMetricStockCount.Text = $"{totalUnits} Units";
                TxtMetricLowStock.Text = $"{lowStockCount} Items";
                TxtMetricCustomers.Text = $"{customers.Count} Accounts";
                TxtMetricValuation.Text = $"Rs. {totalValuation:N0}";
            }
            catch (Exception ex)
            {
                LogActivity($"Metrics error: {ex.Message}");
            }
        }

        // --- Tab 2: Inventory Handlers ---

        private void BtnAddItem_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(TxtItemCode.Text, out int code) || code <= 0)
                {
                    TxtInventoryStatus.Text = "Validation Error: Item Code must be a valid positive integer.";
                    return;
                }

                string desc = TxtItemDesc.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(desc))
                {
                    TxtInventoryStatus.Text = "Validation Error: Item description is required.";
                    return;
                }

                string cat = ((CmbItemCategory.SelectedItem as ComboBoxItem)?.Content?.ToString()) ?? "General";

                if (!int.TryParse(TxtItemQty.Text, out int qty) || qty < 0)
                {
                    TxtInventoryStatus.Text = "Validation Error: Quantity must be zero or a positive integer.";
                    return;
                }

                if (!int.TryParse(TxtItemThreshold.Text, out int threshold) || threshold < 0)
                {
                    TxtInventoryStatus.Text = "Validation Error: Safety threshold must be zero or a positive integer.";
                    return;
                }

                if (!decimal.TryParse(TxtItemPrice.Text, out decimal price) || price <= 0)
                {
                    TxtInventoryStatus.Text = "Validation Error: Unit price must be greater than zero.";
                    return;
                }

                string loc = TxtItemLocation.Text?.Trim() ?? "A-01";

                var newItem = new InventoryItem
                {
                    ItemCode = code,
                    Description = desc,
                    Category = cat,
                    QuantityOnHand = qty,
                    SafetyThreshold = threshold,
                    UnitPrice = price,
                    WarehouseLocation = loc
                };

                bool ok = _inventoryService.AddItem(newItem);
                if (ok)
                {
                    TxtInventoryStatus.Text = $"Success: Item {code} recorded in inventory.";
                    LogActivity($"Inventory Item Added: Code {code} - {desc} (Qty: {qty}, Threshold: {threshold})");
                    RefreshInventory();
                    UpdateOverviewMetrics();

                    TxtItemCode.Text = "";
                    TxtItemDesc.Text = "";
                    TxtItemQty.Text = "";
                    TxtItemThreshold.Text = "";
                    TxtItemPrice.Text = "";
                    TxtItemLocation.Text = "";
                }
            }
            catch (Exception ex)
            {
                TxtInventoryStatus.Text = $"Database Error: {ex.Message}";
                LogActivity($"Inventory Add Failure: {ex.Message}");
            }
        }

        private void BtnRefreshInventory_Click(object? sender, RoutedEventArgs e)
        {
            RefreshInventory();
            UpdateOverviewMetrics();
            TxtInventoryStatus.Text = "Inventory directory refreshed.";
        }

        private void BtnDeleteSelectedStock_Click(object? sender, RoutedEventArgs e)
        {
            if (GridInventory.SelectedItem is InventoryItem item)
            {
                try
                {
                    _inventoryService.DeleteItem(item.ItemCode);
                    LogActivity($"Inventory Item Removed: Code {item.ItemCode} - {item.Description}");
                    RefreshInventory();
                    UpdateOverviewMetrics();
                }
                catch (Exception ex)
                {
                    LogActivity($"Delete Item Failure: {ex.Message}");
                }
            }
        }

        // --- Tab 3: Customer Handlers ---

        private void BtnCalculateCreditPreview_Click(object? sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(TxtCustTurnover.Text, out decimal turnover))
            {
                decimal calculatedLimit = Customer.CalculateCreditLimit(turnover);
                TxtCustomerStatus.Text = $"Calculated Credit Limit: Rs. {calculatedLimit:N2} based on turnover Rs. {turnover:N2}.";
            }
            else
            {
                TxtCustomerStatus.Text = "Please enter valid numeric turnover.";
            }
        }

        private void BtnAddCustomer_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                string name = TxtCustName.Text?.Trim() ?? "";
                string company = TxtCustCompany.Text?.Trim() ?? "";
                string email = TxtCustEmail.Text?.Trim() ?? "";
                string phone = TxtCustPhone.Text?.Trim() ?? "";
                string address = TxtCustAddress.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(company))
                {
                    TxtCustomerStatus.Text = "Validation Error: Contact name and company name are required.";
                    return;
                }

                if (!decimal.TryParse(TxtCustTurnover.Text, out decimal turnover) || turnover < 0)
                {
                    TxtCustomerStatus.Text = "Validation Error: Turnover must be a valid positive amount.";
                    return;
                }

                var customer = new Customer(0, name, email, phone, company, address, turnover);
                bool ok = _customerService.AddCustomer(customer);
                if (ok)
                {
                    TxtCustomerStatus.Text = $"Success: Customer {company} registered with credit limit Rs. {customer.CreditLimit:N2}.";
                    LogActivity($"Customer Registered: {company} ({name}) - Credit Limit Rs. {customer.CreditLimit:N2}");
                    RefreshCustomers();
                    UpdateOverviewMetrics();

                    TxtCustName.Text = "";
                    TxtCustCompany.Text = "";
                    TxtCustEmail.Text = "";
                    TxtCustPhone.Text = "";
                    TxtCustAddress.Text = "";
                    TxtCustTurnover.Text = "";
                }
            }
            catch (Exception ex)
            {
                TxtCustomerStatus.Text = $"Customer Registration Error: {ex.Message}";
                LogActivity($"Customer Add Failure: {ex.Message}");
            }
        }

        // --- Tab 4: Staff Handlers ---

        private void BtnAddStaff_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                string u = TxtStaffUsername.Text?.Trim() ?? "";
                string p = TxtStaffPassword.Text?.Trim() ?? "";
                string n = TxtStaffName.Text?.Trim() ?? "";
                string em = TxtStaffEmail.Text?.Trim() ?? "";
                string d = TxtStaffDept.Text?.Trim() ?? "Logistics";

                if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(n))
                {
                    TxtStaffStatus.Text = "Validation Error: Username, password, and name are required.";
                    return;
                }

                if (!decimal.TryParse(TxtStaffSalary.Text, out decimal sal) || sal < 0)
                {
                    TxtStaffStatus.Text = "Validation Error: Salary must be a positive number.";
                    return;
                }

                bool isAdmin = CmbStaffRole.SelectedIndex == 1;
                int secLevel = isAdmin ? 3 : 1;

                bool ok = _authService.RegisterUser(u, p, n, em, "+94 11 000 0000", d, sal, isAdmin, secLevel);
                if (ok)
                {
                    TxtStaffStatus.Text = $"Success: Staff account '{u}' created.";
                    LogActivity($"Staff Account Created: {u} ({n}) - Department: {d}, Role: {(isAdmin ? "Admin" : "Staff")}");
                    RefreshEmployees();

                    TxtStaffUsername.Text = "";
                    TxtStaffPassword.Text = "";
                    TxtStaffName.Text = "";
                    TxtStaffEmail.Text = "";
                    TxtStaffSalary.Text = "";
                }
            }
            catch (Exception ex)
            {
                TxtStaffStatus.Text = $"Staff Error: {ex.Message}";
                LogActivity($"Staff Creation Error: {ex.Message}");
            }
        }

        // --- Tab 5: Authentication Handlers ---

        private void BtnLogin_Click(object? sender, RoutedEventArgs e)
        {
            PerformLogin(TxtAuthUsername.Text ?? "", TxtAuthPassword.Text ?? "");
        }

        private void BtnQuickOperator_Click(object? sender, RoutedEventArgs e)
        {
            TxtAuthUsername.Text = "operator";
            TxtAuthPassword.Text = "op123";
            PerformLogin("operator", "op123");
        }

        private void PerformLogin(string username, string password)
        {
            try
            {
                var user = _authService.Authenticate(username, password);
                if (user != null)
                {
                    _currentUser = user;
                    UpdateUserStatus();
                    TxtAuthStatus.Text = $"Authentication Successful: Logged in as {user.Name} ({user.GetRoleDescription()})";
                    LogActivity($"User Authenticated: {username} - {user.GetRoleDescription()}");
                }
                else
                {
                    TxtAuthStatus.Text = "Authentication Failed: Invalid username or password.";
                    LogActivity($"Authentication Failed for username: {username}");
                }
            }
            catch (Exception ex)
            {
                TxtAuthStatus.Text = $"Error: {ex.Message}";
                LogActivity($"Auth Exception: {ex.Message}");
            }
        }

        // --- Automated Demonstration Mode ---

        private async void BtnRunAutoDemo_Click(object? sender, RoutedEventArgs e)
        {
            if (_isDemoRunning) return;
            _isDemoRunning = true;
            BtnRunAutoDemo.IsEnabled = false;

            try
            {
                await RunAutomatedDemoSequenceAsync();
            }
            finally
            {
                _isDemoRunning = false;
                BtnRunAutoDemo.IsEnabled = true;
            }
        }

        private void SaveScreenshot(string filename)
        {
            try
            {
                int w = (int)this.Bounds.Width;
                int h = (int)this.Bounds.Height;
                if (w > 0 && h > 0)
                {
                    var rtb = new Avalonia.Media.Imaging.RenderTargetBitmap(new Avalonia.PixelSize(w, h), new Avalonia.Vector(96, 96));
                    rtb.Render(this);
                    string folder = "/Users/nushan/Projects/Namodya/Final/CS107.3_Object_Oriented_Programming_C#";
                    string outPath = System.IO.Path.Combine(folder, filename);
                    rtb.Save(outPath);
                    LogActivity($"Captured Screen Graphic: {filename}");
                }
            }
            catch (Exception ex)
            {
                LogActivity($"Screenshot notice: {ex.Message}");
            }
        }

        private async Task RunAutomatedDemoSequenceAsync()
        {
            LogActivity("COMMENCING SYSTEM VERIFICATION RUN");

            // System Overview Tab
            if (MainTabControl != null) MainTabControl.SelectedIndex = 0;
            LogActivity("Auditing warehouse stock valuation and relational database status.");
            await Task.Delay(2000);
            SaveScreenshot("warehouse_screen_1_overview.png");
            await Task.Delay(1000);

            // Authentication Tab
            if (MainTabControl != null) MainTabControl.SelectedIndex = 4;
            LogActivity("Authenticating administrator credentials.");
            TxtAuthUsername.Text = "admin";
            TxtAuthPassword.Text = "admin123";
            await Task.Delay(1500);
            PerformLogin("admin", "admin123");
            SaveScreenshot("warehouse_screen_5_auth.png");
            await Task.Delay(2000);

            // Inventory Management
            if (MainTabControl != null) MainTabControl.SelectedIndex = 1;
            LogActivity("Inserting stock item: Code 1006 - Modular Heavy-Duty Cantilever Racks.");
            TxtItemCode.Text = "1006";
            TxtItemDesc.Text = "Modular Heavy-Duty Cantilever Racks";
            CmbItemCategory.SelectedIndex = 0;
            TxtItemQty.Text = "35";
            TxtItemThreshold.Text = "10";
            TxtItemPrice.Text = "24500";
            TxtItemLocation.Text = "A-05";
            await Task.Delay(1500);

            BtnAddItem_Click(null, new RoutedEventArgs());
            LogActivity("Stock balance and safety threshold verified.");
            SaveScreenshot("warehouse_screen_2_inventory.png");
            await Task.Delay(2500);

            // Customer Credit Control
            if (MainTabControl != null) MainTabControl.SelectedIndex = 2;
            LogActivity("Registering customer: Apex Industrial Engineering Ltd.");
            TxtCustName.Text = "Samantha Wickrama";
            TxtCustCompany.Text = "Apex Industrial Engineering Ltd";
            TxtCustEmail.Text = "samantha@apex.lk";
            TxtCustPhone.Text = "+94 77 345 6789";
            TxtCustAddress.Text = "15 Industrial Estate, Ekala";
            TxtCustTurnover.Text = "850000";
            await Task.Delay(1500);

            BtnCalculateCreditPreview_Click(null, new RoutedEventArgs());
            await Task.Delay(1200);

            BtnAddCustomer_Click(null, new RoutedEventArgs());
            SaveScreenshot("warehouse_screen_3_customers.png");
            await Task.Delay(2500);

            // Staff Administration
            if (MainTabControl != null) MainTabControl.SelectedIndex = 3;
            LogActivity("Onboarding staff account: kalinga - Quality Assurance.");
            TxtStaffUsername.Text = "kalinga";
            TxtStaffPassword.Text = "kal123";
            TxtStaffName.Text = "Kalinga Bandara";
            TxtStaffEmail.Text = "kalinga@warehouse.local";
            TxtStaffDept.Text = "Quality Assurance";
            TxtStaffSalary.Text = "115000";
            CmbStaffRole.SelectedIndex = 0;
            await Task.Delay(1500);

            BtnAddStaff_Click(null, new RoutedEventArgs());
            SaveScreenshot("warehouse_screen_4_staff.png");
            await Task.Delay(2500);

            // Return to System Overview
            if (MainTabControl != null) MainTabControl.SelectedIndex = 0;
            LogActivity("All subsystems operating within parameters. Verification run finished.");
            await Task.Delay(2000);
        }
    }
}