using System;
using System.Windows.Forms;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Forms;

namespace RestaurantOrderSystem
{
    static class ProgramWinForms
    {
        /// <summary>
        /// The main entry point for the Windows Forms application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Initialize the database
            try
            {
                RestaurantDbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}", "Database Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Start the Windows Forms application
            Application.Run(new MenuForm());
        }
    }
}