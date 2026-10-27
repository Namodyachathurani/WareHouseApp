using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Services;

namespace WareHouseApp
{
    /// <summary>
    /// Login and Sign-up Form.
    /// This window authenticates users and registers new user accounts.
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly AuthenticationService _authService = new AuthenticationService();
        private bool _isSignUpMode = false;

        // Dynamic controls for Sign Up
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Label lblFullName;
        private Label lblEmail;
        private Label lblPhone;
        private Button btnToggleMode;

        public Form1()
        {
            InitializeComponent();
            SetupCustomUI();
        }

        private void SetupCustomUI()
        {
            this.Text = "Warehouse Management System - Login & Registration";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Add Toggle button for Sign Up / Login switch
            btnToggleMode = new Button
            {
                Text = "Don't have an account? Sign Up Here",
                Location = new Point(120, 360),
                Size = new Size(260, 30),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.DarkBlue,
                Cursor = Cursors.Hand
            };
            btnToggleMode.Click += BtnToggleMode_Click;
            this.Controls.Add(btnToggleMode);

            // Optional Sign-up extra fields (hidden by default)
            lblFullName = new Label { Text = "Full Name:", Location = new Point(40, 190), Size = new Size(80, 20), Visible = false };
            txtFullName = new TextBox { Location = new Point(130, 190), Size = new Size(240, 22), Visible = false };

            lblEmail = new Label { Text = "Email:", Location = new Point(40, 220), Size = new Size(80, 20), Visible = false };
            txtEmail = new TextBox { Location = new Point(130, 220), Size = new Size(240, 22), Visible = false };

            lblPhone = new Label { Text = "Phone:", Location = new Point(40, 250), Size = new Size(80, 20), Visible = false };
            txtPhone = new TextBox { Location = new Point(130, 250), Size = new Size(240, 22), Visible = false };

            this.Controls.Add(lblFullName);
            this.Controls.Add(txtFullName);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhone);
        }

        private void BtnToggleMode_Click(object sender, EventArgs e)
        {
            _isSignUpMode = !_isSignUpMode;
            if (_isSignUpMode)
            {
                butLogin.Text = "Create Account";
                btnToggleMode.Text = "Already have an account? Log In";
                lblFullName.Visible = true;
                txtFullName.Visible = true;
                lblEmail.Visible = true;
                txtEmail.Visible = true;
                lblPhone.Visible = true;
                txtPhone.Visible = true;
                this.Height = 460;
            }
            else
            {
                butLogin.Text = "Login";
                btnToggleMode.Text = "Don't have an account? Sign Up Here";
                lblFullName.Visible = false;
                txtFullName.Visible = false;
                lblEmail.Visible = false;
                txtEmail.Visible = false;
                lblPhone.Visible = false;
                txtPhone.Visible = false;
                this.Height = 420;
            }
        }

        private void butLogin_Click(object sender, EventArgs e)
        {
            string username = txtName.Text?.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please provide both Username and Password.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_isSignUpMode)
                {
                    // Execute Sign Up
                    string fullName = txtFullName.Text?.Trim();
                    string email = txtEmail.Text?.Trim();
                    string phone = txtPhone.Text?.Trim();

                    if (string.IsNullOrEmpty(fullName))
                    {
                        MessageBox.Show("Please provide your Full Name.", "Validation Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    bool success = _authService.SignUp(username, password, fullName, email, phone, "Employee");
                    if (success)
                    {
                        MessageBox.Show("Account successfully created! You can now log in.", "Registration Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BtnToggleMode_Click(sender, e);
                    }
                }
                else
                {
                    // Execute Login
                    bool authenticated = _authService.Login(username, password);
                    if (authenticated)
                    {
                        MessageBox.Show($"Welcome, {AuthenticationService.CurrentLoggedInUser.FullName}!",
                                        "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        this.Hide();
                        DashBoard dash = new DashBoard();
                        dash.FormClosed += (s, args) => this.Close();
                        dash.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password. Please try again.", "Authentication Failed",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation error: {ex.Message}", "System Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}
