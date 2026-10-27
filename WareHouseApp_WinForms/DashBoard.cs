using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Models;
using WareHouseApp.Services;

namespace WareHouseApp
{
    /// <summary>
    /// Comprehensive Dashboard Form for WareHouseApp.
    /// This window provides interactive navigation for Inventory,
    /// Customers, Employees, and user session logout.
    /// </summary>
    public partial class DashBoard : Form
    {
        private readonly InventoryService _inventoryService = new InventoryService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly EmployeeService _employeeService = new EmployeeService();

        // UI Tabs & Views
        private TabControl tabControl;
        private TabPage tabInventory;
        private TabPage tabCustomers;
        private TabPage tabEmployees;

        // Inventory Controls
        private DataGridView dgvInventory;
        private TextBox txtInvCode, txtInvName, txtInvCat, txtInvQty, txtInvPrice, txtInvLoc, txtInvMin;
        private Button btnInvAdd, btnInvUpdate, btnInvDelete, btnInvRefresh;

        // Customer Controls
        private DataGridView dgvCustomers;
        private TextBox txtCustCode, txtCustName, txtCustContact, txtCustPhone, txtCustEmail, txtCustAddress, txtCustCredit;
        private Button btnCustAdd, btnCustUpdate, btnCustDelete, btnCustRefresh;

        // Employee Controls
        private DataGridView dgvEmployees;
        private TextBox txtEmpUser, txtEmpPass, txtEmpName, txtEmpEmail, txtEmpPhone, txtEmpPos, txtEmpDept, txtEmpSalary;
        private Button btnEmpAdd, btnEmpUpdate, btnEmpDelete, btnEmpRefresh;

        public DashBoard()
        {
            InitializeComponent();
            SetupDashboardUI();
            LoadAllData();
        }

        private void SetupDashboardUI()
        {
            this.Text = "WareHouseApp - Integrated Operations Hub";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Header User Info Banner
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(30, 58, 138)
            };

            var lblTitle = new Label
            {
                Text = "WAREHOUSE MANAGEMENT SYSTEM (C# .NET)",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            string userInfo = AuthenticationService.CurrentLoggedInUser != null
                ? $"Logged in as: {AuthenticationService.CurrentLoggedInUser.FullName} ({AuthenticationService.CurrentLoggedInUser.GetRoleDescription()})"
                : "Active User Session: Administrator";

            var lblUser = new Label
            {
                Text = userInfo,
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.LightSkyBlue,
                Location = new Point(450, 20),
                AutoSize = true
            };

            var btnLogout = new Button
            {
                Text = "Log Out",
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(960, 14),
                Size = new Size(90, 32),
                Cursor = Cursors.Hand
            };
            btnLogout.Click += (s, e) => {
                AuthenticationService.Logout();
                this.Hide();
                new Form1().Show();
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblUser);
            pnlHeader.Controls.Add(btnLogout);
            this.Controls.Add(pnlHeader);

            // Tab Control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            tabInventory = new TabPage("Inventory Management");
            tabCustomers = new TabPage("Customer Management");
            tabEmployees = new TabPage("Employee Management");

            BuildInventoryTab();
            BuildCustomerTab();
            BuildEmployeeTab();

            tabControl.TabPages.Add(tabInventory);
            tabControl.TabPages.Add(tabCustomers);
            tabControl.TabPages.Add(tabEmployees);

            this.Controls.Add(tabControl);
            pnlHeader.BringToFront();
        }

        #region INVENTORY TAB
        private void BuildInventoryTab()
        {
            var pnlForm = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.FromArgb(243, 244, 246) };
            
            pnlForm.Controls.Add(new Label { Text = "SKU Code:", Location = new Point(20, 15), AutoSize = true });
            txtInvCode = new TextBox { Location = new Point(100, 12), Width = 110 };
            pnlForm.Controls.Add(txtInvCode);

            pnlForm.Controls.Add(new Label { Text = "Item Name:", Location = new Point(230, 15), AutoSize = true });
            txtInvName = new TextBox { Location = new Point(310, 12), Width = 160 };
            pnlForm.Controls.Add(txtInvName);

            pnlForm.Controls.Add(new Label { Text = "Category:", Location = new Point(490, 15), AutoSize = true });
            txtInvCat = new TextBox { Location = new Point(560, 12), Width = 130 };
            pnlForm.Controls.Add(txtInvCat);

            pnlForm.Controls.Add(new Label { Text = "Quantity:", Location = new Point(20, 50), AutoSize = true });
            txtInvQty = new TextBox { Location = new Point(100, 47), Width = 110 };
            pnlForm.Controls.Add(txtInvQty);

            pnlForm.Controls.Add(new Label { Text = "Unit Price:", Location = new Point(230, 50), AutoSize = true });
            txtInvPrice = new TextBox { Location = new Point(310, 47), Width = 160 };
            pnlForm.Controls.Add(txtInvPrice);

            pnlForm.Controls.Add(new Label { Text = "Location:", Location = new Point(490, 50), AutoSize = true });
            txtInvLoc = new TextBox { Location = new Point(560, 47), Width = 130 };
            pnlForm.Controls.Add(txtInvLoc);

            pnlForm.Controls.Add(new Label { Text = "Min Alert:", Location = new Point(710, 50), AutoSize = true });
            txtInvMin = new TextBox { Location = new Point(780, 47), Width = 80, Text = "10" };
            pnlForm.Controls.Add(txtInvMin);

            // Action Buttons
            btnInvAdd = new Button { Text = "Add Item", Location = new Point(100, 90), Width = 100, BackColor = Color.FromArgb(5, 150, 105), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnInvUpdate = new Button { Text = "Update Item", Location = new Point(210, 90), Width = 100, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnInvDelete = new Button { Text = "Delete Item", Location = new Point(320, 90), Width = 100, BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnInvRefresh = new Button { Text = "Refresh", Location = new Point(430, 90), Width = 100, BackColor = Color.FromArgb(75, 85, 99), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnInvAdd.Click += BtnInvAdd_Click;
            btnInvUpdate.Click += BtnInvUpdate_Click;
            btnInvDelete.Click += BtnInvDelete_Click;
            btnInvRefresh.Click += (s, e) => LoadInventoryGrid();

            pnlForm.Controls.Add(btnInvAdd);
            pnlForm.Controls.Add(btnInvUpdate);
            pnlForm.Controls.Add(btnInvDelete);
            pnlForm.Controls.Add(btnInvRefresh);

            dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            dgvInventory.SelectionChanged += DgvInventory_SelectionChanged;

            tabInventory.Controls.Add(dgvInventory);
            tabInventory.Controls.Add(pnlForm);
        }

        private void LoadInventoryGrid()
        {
            try
            {
                var items = _inventoryService.GetAllItems();
                dgvInventory.DataSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvInventory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow?.DataBoundItem is InventoryItem item)
            {
                txtInvCode.Text = item.ItemCode;
                txtInvName.Text = item.ItemName;
                txtInvCat.Text = item.Category;
                txtInvQty.Text = item.Quantity.ToString();
                txtInvPrice.Text = item.UnitPrice.ToString("F2");
                txtInvLoc.Text = item.WarehouseLocation;
                txtInvMin.Text = item.MinimumThreshold.ToString();
            }
        }

        private void BtnInvAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var item = new InventoryItem(
                    0,
                    txtInvCode.Text.Trim(),
                    txtInvName.Text.Trim(),
                    txtInvCat.Text.Trim(),
                    int.Parse(txtInvQty.Text.Trim()),
                    decimal.Parse(txtInvPrice.Text.Trim()),
                    txtInvLoc.Text.Trim(),
                    int.Parse(txtInvMin.Text.Trim())
                );
                _inventoryService.AddItem(item);
                MessageBox.Show("Inventory SKU added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadInventoryGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding SKU: {ex.Message}", "Validation / Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnInvUpdate_Click(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow?.DataBoundItem is InventoryItem selected)
            {
                try
                {
                    selected.ItemName = txtInvName.Text.Trim();
                    selected.Category = txtInvCat.Text.Trim();
                    selected.Quantity = int.Parse(txtInvQty.Text.Trim());
                    selected.UnitPrice = decimal.Parse(txtInvPrice.Text.Trim());
                    selected.WarehouseLocation = txtInvLoc.Text.Trim();
                    selected.MinimumThreshold = int.Parse(txtInvMin.Text.Trim());

                    _inventoryService.UpdateItem(selected);
                    MessageBox.Show("Inventory SKU updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventoryGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating SKU: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnInvDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow?.DataBoundItem is InventoryItem selected)
            {
                if (MessageBox.Show($"Are you sure you want to delete {selected.ItemName}?", "Confirm Delete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _inventoryService.DeleteItem(selected.Id);
                        LoadInventoryGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting SKU: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region CUSTOMER TAB
        private void BuildCustomerTab()
        {
            var pnlForm = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.FromArgb(243, 244, 246) };

            pnlForm.Controls.Add(new Label { Text = "Cust Code:", Location = new Point(20, 15), AutoSize = true });
            txtCustCode = new TextBox { Location = new Point(100, 12), Width = 110 };
            pnlForm.Controls.Add(txtCustCode);

            pnlForm.Controls.Add(new Label { Text = "Client Name:", Location = new Point(230, 15), AutoSize = true });
            txtCustName = new TextBox { Location = new Point(310, 12), Width = 160 };
            pnlForm.Controls.Add(txtCustName);

            pnlForm.Controls.Add(new Label { Text = "Contact Person:", Location = new Point(490, 15), AutoSize = true });
            txtCustContact = new TextBox { Location = new Point(590, 12), Width = 130 };
            pnlForm.Controls.Add(txtCustContact);

            pnlForm.Controls.Add(new Label { Text = "Phone:", Location = new Point(20, 50), AutoSize = true });
            txtCustPhone = new TextBox { Location = new Point(100, 47), Width = 110 };
            pnlForm.Controls.Add(txtCustPhone);

            pnlForm.Controls.Add(new Label { Text = "Email:", Location = new Point(230, 50), AutoSize = true });
            txtCustEmail = new TextBox { Location = new Point(310, 47), Width = 160 };
            pnlForm.Controls.Add(txtCustEmail);

            pnlForm.Controls.Add(new Label { Text = "Address:", Location = new Point(490, 50), AutoSize = true });
            txtCustAddress = new TextBox { Location = new Point(590, 47), Width = 130 };
            pnlForm.Controls.Add(txtCustAddress);

            pnlForm.Controls.Add(new Label { Text = "Credit Limit:", Location = new Point(740, 50), AutoSize = true });
            txtCustCredit = new TextBox { Location = new Point(830, 47), Width = 90, Text = "100000" };
            pnlForm.Controls.Add(txtCustCredit);

            btnCustAdd = new Button { Text = "Add Customer", Location = new Point(100, 90), Width = 120, BackColor = Color.FromArgb(5, 150, 105), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCustUpdate = new Button { Text = "Update Customer", Location = new Point(230, 90), Width = 130, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCustDelete = new Button { Text = "Delete Customer", Location = new Point(370, 90), Width = 120, BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCustRefresh = new Button { Text = "Refresh", Location = new Point(500, 90), Width = 100, BackColor = Color.FromArgb(75, 85, 99), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnCustAdd.Click += BtnCustAdd_Click;
            btnCustUpdate.Click += BtnCustUpdate_Click;
            btnCustDelete.Click += BtnCustDelete_Click;
            btnCustRefresh.Click += (s, e) => LoadCustomerGrid();

            pnlForm.Controls.Add(btnCustAdd);
            pnlForm.Controls.Add(btnCustUpdate);
            pnlForm.Controls.Add(btnCustDelete);
            pnlForm.Controls.Add(btnCustRefresh);

            dgvCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;

            tabCustomers.Controls.Add(dgvCustomers);
            tabCustomers.Controls.Add(pnlForm);
        }

        private void LoadCustomerGrid()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                dgvCustomers.DataSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is Customer c)
            {
                txtCustCode.Text = c.CustomerCode;
                txtCustName.Text = c.Name;
                txtCustContact.Text = c.ContactPerson;
                txtCustPhone.Text = c.Phone;
                txtCustEmail.Text = c.Email;
                txtCustAddress.Text = c.Address;
                txtCustCredit.Text = c.CreditLimit.ToString("F2");
            }
        }

        private void BtnCustAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var c = new Customer(
                    0,
                    txtCustCode.Text.Trim(),
                    txtCustName.Text.Trim(),
                    txtCustContact.Text.Trim(),
                    txtCustPhone.Text.Trim(),
                    txtCustEmail.Text.Trim(),
                    txtCustAddress.Text.Trim(),
                    decimal.Parse(txtCustCredit.Text.Trim())
                );
                _customerService.AddCustomer(c);
                MessageBox.Show("Customer account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCustomerGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCustUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is Customer c)
            {
                try
                {
                    c.Name = txtCustName.Text.Trim();
                    c.ContactPerson = txtCustContact.Text.Trim();
                    c.Phone = txtCustPhone.Text.Trim();
                    c.Email = txtCustEmail.Text.Trim();
                    c.Address = txtCustAddress.Text.Trim();
                    c.CreditLimit = decimal.Parse(txtCustCredit.Text.Trim());

                    _customerService.UpdateCustomer(c);
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomerGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnCustDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is Customer c)
            {
                if (MessageBox.Show($"Delete customer record for {c.Name}?", "Confirm Delete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _customerService.DeleteCustomer(c.Id);
                        LoadCustomerGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region EMPLOYEE TAB
        private void BuildEmployeeTab()
        {
            var pnlForm = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.FromArgb(243, 244, 246) };

            pnlForm.Controls.Add(new Label { Text = "Username:", Location = new Point(20, 15), AutoSize = true });
            txtEmpUser = new TextBox { Location = new Point(100, 12), Width = 110 };
            pnlForm.Controls.Add(txtEmpUser);

            pnlForm.Controls.Add(new Label { Text = "Password:", Location = new Point(220, 15), AutoSize = true });
            txtEmpPass = new TextBox { Location = new Point(290, 12), Width = 120, UseSystemPasswordChar = true };
            pnlForm.Controls.Add(txtEmpPass);

            pnlForm.Controls.Add(new Label { Text = "Full Name:", Location = new Point(420, 15), AutoSize = true });
            txtEmpName = new TextBox { Location = new Point(490, 12), Width = 160 };
            pnlForm.Controls.Add(txtEmpName);

            pnlForm.Controls.Add(new Label { Text = "Position:", Location = new Point(660, 15), AutoSize = true });
            txtEmpPos = new TextBox { Location = new Point(730, 12), Width = 130 };
            pnlForm.Controls.Add(txtEmpPos);

            pnlForm.Controls.Add(new Label { Text = "Department:", Location = new Point(20, 50), AutoSize = true });
            txtEmpDept = new TextBox { Location = new Point(100, 47), Width = 110 };
            pnlForm.Controls.Add(txtEmpDept);

            pnlForm.Controls.Add(new Label { Text = "Email:", Location = new Point(220, 50), AutoSize = true });
            txtEmpEmail = new TextBox { Location = new Point(290, 47), Width = 120 };
            pnlForm.Controls.Add(txtEmpEmail);

            pnlForm.Controls.Add(new Label { Text = "Phone:", Location = new Point(420, 50), AutoSize = true });
            txtEmpPhone = new TextBox { Location = new Point(490, 47), Width = 160 };
            pnlForm.Controls.Add(txtEmpPhone);

            pnlForm.Controls.Add(new Label { Text = "Salary (Rs):", Location = new Point(660, 50), AutoSize = true });
            txtEmpSalary = new TextBox { Location = new Point(730, 47), Width = 130 };
            pnlForm.Controls.Add(txtEmpSalary);

            btnEmpAdd = new Button { Text = "Add Employee", Location = new Point(100, 90), Width = 120, BackColor = Color.FromArgb(5, 150, 105), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEmpUpdate = new Button { Text = "Update Employee", Location = new Point(230, 90), Width = 130, BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEmpDelete = new Button { Text = "Delete Employee", Location = new Point(370, 90), Width = 120, BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEmpRefresh = new Button { Text = "Refresh", Location = new Point(500, 90), Width = 100, BackColor = Color.FromArgb(75, 85, 99), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnEmpAdd.Click += BtnEmpAdd_Click;
            btnEmpUpdate.Click += BtnEmpUpdate_Click;
            btnEmpDelete.Click += BtnEmpDelete_Click;
            btnEmpRefresh.Click += (s, e) => LoadEmployeeGrid();

            pnlForm.Controls.Add(btnEmpAdd);
            pnlForm.Controls.Add(btnEmpUpdate);
            pnlForm.Controls.Add(btnEmpDelete);
            pnlForm.Controls.Add(btnEmpRefresh);

            dgvEmployees = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true
            };
            dgvEmployees.SelectionChanged += DgvEmployees_SelectionChanged;

            tabEmployees.Controls.Add(dgvEmployees);
            tabEmployees.Controls.Add(pnlForm);
        }

        private void LoadEmployeeGrid()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                dgvEmployees.DataSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow?.DataBoundItem is Employee emp)
            {
                txtEmpUser.Text = emp.Username;
                txtEmpPass.Text = emp.Password;
                txtEmpName.Text = emp.FullName;
                txtEmpPos.Text = emp.Position;
                txtEmpDept.Text = emp.Department;
                txtEmpEmail.Text = emp.Email;
                txtEmpPhone.Text = emp.Phone;
                txtEmpSalary.Text = emp.Salary.ToString("F2");
            }
        }

        private void BtnEmpAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var emp = new Employee(
                    0,
                    txtEmpUser.Text.Trim(),
                    txtEmpPass.Text,
                    txtEmpName.Text.Trim(),
                    txtEmpEmail.Text.Trim(),
                    txtEmpPhone.Text.Trim(),
                    txtEmpPos.Text.Trim(),
                    txtEmpDept.Text.Trim(),
                    decimal.Parse(txtEmpSalary.Text.Trim())
                );
                _employeeService.AddEmployee(emp);
                MessageBox.Show("Staff member registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployeeGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnEmpUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow?.DataBoundItem is Employee emp)
            {
                try
                {
                    emp.FullName = txtEmpName.Text.Trim();
                    emp.Position = txtEmpPos.Text.Trim();
                    emp.Department = txtEmpDept.Text.Trim();
                    emp.Email = txtEmpEmail.Text.Trim();
                    emp.Phone = txtEmpPhone.Text.Trim();
                    emp.Salary = decimal.Parse(txtEmpSalary.Text.Trim());

                    _employeeService.UpdateEmployee(emp);
                    MessageBox.Show("Employee details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployeeGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnEmpDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow?.DataBoundItem is Employee emp)
            {
                if (MessageBox.Show($"Delete employee record for {emp.FullName}?", "Confirm Delete",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        _employeeService.DeleteEmployee(emp.Id);
                        LoadEmployeeGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        private void LoadAllData()
        {
            LoadInventoryGrid();
            LoadCustomerGrid();
            LoadEmployeeGrid();
        }
    }
}
