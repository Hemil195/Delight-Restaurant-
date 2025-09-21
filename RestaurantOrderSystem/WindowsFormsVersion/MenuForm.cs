using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using RestaurantOrderSystem.Data;
using RestaurantOrderSystem.Models;

namespace RestaurantOrderSystem.Forms
{
    public partial class MenuForm : Form
    {
        private RestaurantDbContext dbContext;
        private BindingList<MenuItem> menuItems;
        private MenuItem selectedMenuItem;

        public MenuForm()
        {
            InitializeComponent();
            dbContext = new RestaurantDbContext();
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            try
            {
                var items = dbContext.GetMenuItems();
                menuItems = new BindingList<MenuItem>(items);
                dataGridViewMenuItems.DataSource = menuItems;
                
                // Hide the ID column if desired
                if (dataGridViewMenuItems.Columns["ItemID"] != null)
                {
                    dataGridViewMenuItems.Columns["ItemID"].Visible = false;
                }

                // Format columns
                if (dataGridViewMenuItems.Columns["Price"] != null)
                {
                    dataGridViewMenuItems.Columns["Price"].DefaultCellStyle.Format = "C2";
                    dataGridViewMenuItems.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewMenuItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewMenuItems.SelectedRows.Count > 0)
            {
                selectedMenuItem = (MenuItem)dataGridViewMenuItems.SelectedRows[0].DataBoundItem;
                if (selectedMenuItem != null)
                {
                    textBoxName.Text = selectedMenuItem.Name;
                    textBoxPrice.Text = selectedMenuItem.Price.ToString("F2");
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    var newItem = new MenuItem
                    {
                        Name = textBoxName.Text.Trim(),
                        Price = decimal.Parse(textBoxPrice.Text)
                    };

                    dbContext.AddMenuItem(newItem);
                    LoadMenuItems();
                    ClearInputs();
                    MessageBox.Show("Menu item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding menu item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (selectedMenuItem == null)
            {
                MessageBox.Show("Please select a menu item to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateInput())
            {
                try
                {
                    selectedMenuItem.Name = textBoxName.Text.Trim();
                    selectedMenuItem.Price = decimal.Parse(textBoxPrice.Text);

                    dbContext.UpdateMenuItem(selectedMenuItem);
                    LoadMenuItems();
                    ClearInputs();
                    MessageBox.Show("Menu item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating menu item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (selectedMenuItem == null)
            {
                MessageBox.Show("Please select a menu item to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{selectedMenuItem.Name}'?", 
                                       "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    dbContext.DeleteMenuItem(selectedMenuItem.ItemID);
                    LoadMenuItems();
                    ClearInputs();
                    MessageBox.Show("Menu item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting menu item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadMenuItems();
            ClearInputs();
        }

        private void ClearInputs()
        {
            textBoxName.Clear();
            textBoxPrice.Clear();
            selectedMenuItem = null;
            dataGridViewMenuItems.ClearSelection();
            textBoxName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Please enter a menu item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxName.Focus();
                return false;
            }

            if (!decimal.TryParse(textBoxPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPrice.Focus();
                return false;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}