using System;
using System.Windows.Forms;
using WareHouseApp.Data;

namespace WareHouseApp
{
    static class Program
    {
        /// <summary>
        /// Main application entrypoint.
        /// Initializes local database and starts the Login Form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Initialize local database tables and seed data
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database startup error: {ex.Message}\nApplication will run with in-memory fallback.",
                                "Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Launch initial Login Form
            Application.Run(new Form1());
        }
    }
}
