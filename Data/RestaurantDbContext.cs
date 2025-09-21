using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using RestaurantOrderSystem.Models;

namespace RestaurantOrderSystem.Data
{
    public class RestaurantDbContext : IDisposable
    {
        private readonly string connectionString;

        public RestaurantDbContext()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region Table Management
        public void CreateTables()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Create Categories table
                    var createCategoriesTable = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
                        CREATE TABLE Categories (
                            CategoryID int IDENTITY(1,1) PRIMARY KEY,
                            Name nvarchar(100) NOT NULL,
                            Description nvarchar(500),
                            ImageUrl nvarchar(255)
                        )";
                    
                    // Create MenuItems table
                    var createMenuItemsTable = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MenuItems')
                        CREATE TABLE MenuItems (
                            ItemID int IDENTITY(1,1) PRIMARY KEY,
                            Name nvarchar(100) NOT NULL,
                            Price decimal(10,2) NOT NULL,
                            Description nvarchar(500),
                            ImageUrl nvarchar(255),
                            CategoryID int NOT NULL,
                            IsAvailable bit NOT NULL DEFAULT 1,
                            IsFeatured bit NOT NULL DEFAULT 0,
                            FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
                        )";
                    
                    // Create Orders table
                    var createOrdersTable = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
                        CREATE TABLE Orders (
                            OrderID int IDENTITY(1,1) PRIMARY KEY,
                            CustomerName nvarchar(100) NOT NULL,
                            CustomerEmail nvarchar(100) NOT NULL,
                            CustomerPhone nvarchar(20) NOT NULL,
                            SpecialInstructions nvarchar(500),
                            TotalAmount decimal(10,2) NOT NULL,
                            OrderDate datetime NOT NULL DEFAULT GETDATE(),
                            Status int NOT NULL DEFAULT 0
                        )";
                    
                    // Create OrderDetails table
                    var createOrderDetailsTable = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderDetails')
                        CREATE TABLE OrderDetails (
                            OrderDetailID int IDENTITY(1,1) PRIMARY KEY,
                            OrderID int NOT NULL,
                            ItemID int NOT NULL,
                            Quantity int NOT NULL,
                            UnitPrice decimal(10,2) NOT NULL,
                            Subtotal decimal(10,2) NOT NULL,
                            FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
                            FOREIGN KEY (ItemID) REFERENCES MenuItems(ItemID)
                        )";

                    var command = new SqlCommand(createCategoriesTable, connection);
                    command.ExecuteNonQuery();
                    
                    command = new SqlCommand(createMenuItemsTable, connection);
                    command.ExecuteNonQuery();
                    
                    command = new SqlCommand(createOrdersTable, connection);
                    command.ExecuteNonQuery();
                    
                    command = new SqlCommand(createOrderDetailsTable, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating database tables: {ex.Message}", ex);
            }
        }

        public void SeedData()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Check if categories exist
                    var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Categories", connection);
                    var categoryCount = (int)checkCommand.ExecuteScalar();
                    
                    if (categoryCount == 0)
                    {
                        // Add sample vegetarian categories with corrected paths
                        var categories = new[]
                        {
                            ("Appetizers", "Fresh and delicious vegetarian starters", "/Content/images/categories/appetizers.jpg"),
                            ("Main Courses", "Hearty vegetarian main dishes", "/Content/images/categories/mains.jpg"),
                            ("Beverages", "Refreshing drinks and fresh juices", "/Content/images/categories/beverages.jpg"),
                            ("Desserts", "Sweet vegetarian treats", "/Content/images/categories/desserts.jpg"),
                            ("Salads", "Fresh and healthy salad options", "/Content/images/categories/salads.jpg"),
                            ("Rice & Biryanis", "Aromatic rice dishes and vegetarian biryanis", "/Content/images/categories/rice.jpg"),
                            ("Indian Breads", "Fresh rotis, naans and parathas", "/Content/images/categories/breads.jpg")
                        };

                        foreach (var (name, description, imageUrl) in categories)
                        {
                            var command = new SqlCommand("INSERT INTO Categories (Name, Description, ImageUrl) VALUES (@name, @description, @imageUrl)", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@description", description);
                            command.Parameters.AddWithValue("@imageUrl", imageUrl);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Check if menu items exist
                    checkCommand = new SqlCommand("SELECT COUNT(*) FROM MenuItems", connection);
                    var menuCount = (int)checkCommand.ExecuteScalar();
                    
                    if (menuCount == 0)
                    {
                        // Add sample vegetarian menu items with corrected paths
                        var menuItems = new[]
                        {
                            ("Paneer Tikka", 280.00m, "Grilled cottage cheese marinated with aromatic spices and herbs", "/Content/images/menu/paneer-tikka.png", 1, true, true),
                            ("Vegetable Biryani", 320.00m, "Fragrant basmati rice layered with mixed vegetables and traditional spices", "/Content/images/menu/veg-biryani.png", 6, true, true),
                            ("Dal Makhani", 240.00m, "Creamy black lentils slow-cooked with butter, tomatoes and aromatic spices", "/Content/images/menu/dal-makhani.png", 2, true, true),
                            ("Gulab Jamun (2 pcs)", 120.00m, "Soft milk dumplings soaked in rose-flavored sugar syrup", "/Content/images/menu/gulab-jamun.png", 4, true, true),
                            ("Fresh Lime Soda", 80.00m, "Refreshing lime juice with sparkling soda water and fresh mint", "/Content/images/menu/lime-soda.png", 3, true, false),
                            ("Vegetable Spring Rolls (4 pcs)", 180.00m, "Crispy golden spring rolls filled with fresh seasonal vegetables", "/Content/images/menu/spring-rolls.png", 1, true, false),
                            ("Palak Paneer", 260.00m, "Fresh cottage cheese cubes in creamy spinach gravy with garlic and ginger", "/Content/images/menu/palak-paneer.png", 2, true, true),
                            ("Garden Fresh Salad", 150.00m, "Mixed seasonal greens with cucumber, tomatoes and house special dressing", "/Content/images/menu/garden-salad.png", 5, true, false),
                            ("Ras Malai (2 pcs)", 140.00m, "Soft cottage cheese dumplings in sweet cardamom-flavored milk", "/Content/images/menu/ras-malai.png", 4, true, false),
                            ("Mango Lassi", 100.00m, "Traditional thick yogurt drink blended with fresh mango pulp", "/Content/images/menu/mango-lassi.png", 3, true, false),
                            ("Aloo Gobi", 220.00m, "Classic spiced potato and cauliflower curry with onions and tomatoes", "/Content/images/menu/aloo-gobi.png", 2, true, false),
                            ("Butter Naan", 60.00m, "Soft leavened bread baked in tandoor and brushed with butter", "/Content/images/menu/butter-naan.png", 7, true, false),
                            ("Jeera Rice", 160.00m, "Fragrant basmati rice tempered with cumin seeds and ghee", "/Content/images/menu/jeera-rice.png", 6, true, false),
                            ("Masala Chai", 40.00m, "Traditional Indian tea brewed with cardamom, ginger and aromatic spices", "/Content/images/menu/masala-chai.png", 3, true, false),
                            ("Rajma Masala", 230.00m, "Red kidney beans slow-cooked in rich tomato and onion gravy with Indian spices", "/Content/images/menu/rajma.png", 2, true, false)
                        };

                        foreach (var (name, price, description, imageUrl, categoryId, isAvailable, isFeatured) in menuItems)
                        {
                            var command = new SqlCommand(@"
                                INSERT INTO MenuItems (Name, Price, Description, ImageUrl, CategoryID, IsAvailable, IsFeatured) 
                                VALUES (@name, @price, @description, @imageUrl, @categoryId, @isAvailable, @isFeatured)", connection);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@price", price);
                            command.Parameters.AddWithValue("@description", description);
                            command.Parameters.AddWithValue("@imageUrl", imageUrl);
                            command.Parameters.AddWithValue("@categoryId", categoryId);
                            command.Parameters.AddWithValue("@isAvailable", isAvailable);
                            command.Parameters.AddWithValue("@isFeatured", isFeatured);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Clear existing data and reseed with updated image paths
        /// </summary>
        public void RefreshDataWithCorrectImagePaths()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Clear existing data (in correct order due to foreign keys)
                    var commands = new[]
                    {
                        "DELETE FROM OrderDetails",
                        "DELETE FROM Orders", 
                        "DELETE FROM MenuItems",
                        "DELETE FROM Categories"
                    };

                    foreach (var sql in commands)
                    {
                        var command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }

                    // Reset identity seeds
                    var resetCommands = new[]
                    {
                        "DBCC CHECKIDENT ('Categories', RESEED, 0)",
                        "DBCC CHECKIDENT ('MenuItems', RESEED, 0)",
                        "DBCC CHECKIDENT ('Orders', RESEED, 0)",
                        "DBCC CHECKIDENT ('OrderDetails', RESEED, 0)"
                    };

                    foreach (var sql in resetCommands)
                    {
                        var command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }

                // Now reseed with correct data
                SeedData();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error refreshing database: {ex.Message}", ex);
            }
        }
        #endregion

        #region Category Methods
        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT CategoryID, Name, Description, ImageUrl FROM Categories ORDER BY Name", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                CategoryID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting categories: {ex.Message}", ex);
            }
            return categories;
        }

        public Category GetCategory(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT CategoryID, Name, Description, ImageUrl FROM Categories WHERE CategoryID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Category
                            {
                                CategoryID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImageUrl = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting category: {ex.Message}", ex);
            }
            return null;
        }

        public void AddCategory(Category category)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("INSERT INTO Categories (Name, Description, ImageUrl) VALUES (@name, @description, @imageUrl)", connection);
                    command.Parameters.AddWithValue("@name", category.Name);
                    command.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imageUrl", category.ImageUrl ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding category: {ex.Message}", ex);
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("UPDATE Categories SET Name = @name, Description = @description, ImageUrl = @imageUrl WHERE CategoryID = @id", connection);
                    command.Parameters.AddWithValue("@name", category.Name);
                    command.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imageUrl", category.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", category.CategoryID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }

        public void DeleteCategory(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("DELETE FROM Categories WHERE CategoryID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}", ex);
            }
        }
        #endregion

        #region MenuItem Methods
        public List<MenuItem> GetMenuItems()
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT m.ItemID, m.Name, m.Price, m.Description, m.ImageUrl, m.CategoryID, m.IsAvailable, m.IsFeatured, c.Name as CategoryName
                        FROM MenuItems m 
                        LEFT JOIN Categories c ON m.CategoryID = c.CategoryID 
                        ORDER BY m.Name", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var menuItem = new MenuItem
                            {
                                ItemID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CategoryID = reader.GetInt32(5),
                                IsAvailable = reader.GetBoolean(6),
                                IsFeatured = reader.GetBoolean(7)
                            };
                            
                            if (!reader.IsDBNull(8))
                            {
                                menuItem.Category = new Category { Name = reader.GetString(8) };
                            }
                            
                            menuItems.Add(menuItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting menu items: {ex.Message}", ex);
            }
            return menuItems;
        }

        public List<MenuItem> GetMenuItemsByCategory(int categoryId)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT m.ItemID, m.Name, m.Price, m.Description, m.ImageUrl, m.CategoryID, m.IsAvailable, m.IsFeatured
                        FROM MenuItems m 
                        WHERE m.CategoryID = @categoryId AND m.IsAvailable = 1
                        ORDER BY m.Name", connection);
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menuItems.Add(new MenuItem
                            {
                                ItemID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CategoryID = reader.GetInt32(5),
                                IsAvailable = reader.GetBoolean(6),
                                IsFeatured = reader.GetBoolean(7)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting menu items by category: {ex.Message}", ex);
            }
            return menuItems;
        }

        public List<MenuItem> GetFeaturedMenuItems()
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT m.ItemID, m.Name, m.Price, m.Description, m.ImageUrl, m.CategoryID, m.IsAvailable, m.IsFeatured
                        FROM MenuItems m 
                        WHERE m.IsFeatured = 1 AND m.IsAvailable = 1
                        ORDER BY m.Name", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menuItems.Add(new MenuItem
                            {
                                ItemID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CategoryID = reader.GetInt32(5),
                                IsAvailable = reader.GetBoolean(6),
                                IsFeatured = reader.GetBoolean(7)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting featured menu items: {ex.Message}", ex);
            }
            return menuItems;
        }

        public MenuItem GetMenuItem(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT m.ItemID, m.Name, m.Price, m.Description, m.ImageUrl, m.CategoryID, m.IsAvailable, m.IsFeatured, c.Name as CategoryName
                        FROM MenuItems m 
                        LEFT JOIN Categories c ON m.CategoryID = c.CategoryID 
                        WHERE m.ItemID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var menuItem = new MenuItem
                            {
                                ItemID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CategoryID = reader.GetInt32(5),
                                IsAvailable = reader.GetBoolean(6),
                                IsFeatured = reader.GetBoolean(7)
                            };
                            
                            if (!reader.IsDBNull(8))
                            {
                                menuItem.Category = new Category { Name = reader.GetString(8) };
                            }
                            
                            return menuItem;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting menu item: {ex.Message}", ex);
            }
            return null;
        }

        public void AddMenuItem(MenuItem item)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        INSERT INTO MenuItems (Name, Price, Description, ImageUrl, CategoryID, IsAvailable, IsFeatured) 
                        VALUES (@name, @price, @description, @imageUrl, @categoryId, @isAvailable, @isFeatured)", connection);
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@price", item.Price);
                    command.Parameters.AddWithValue("@description", item.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imageUrl", item.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@categoryId", item.CategoryID);
                    command.Parameters.AddWithValue("@isAvailable", item.IsAvailable);
                    command.Parameters.AddWithValue("@isFeatured", item.IsFeatured);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding menu item: {ex.Message}", ex);
            }
        }

        public void UpdateMenuItem(MenuItem item)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        UPDATE MenuItems SET Name = @name, Price = @price, Description = @description, 
                        ImageUrl = @imageUrl, CategoryID = @categoryId, IsAvailable = @isAvailable, IsFeatured = @isFeatured 
                        WHERE ItemID = @id", connection);
                    command.Parameters.AddWithValue("@name", item.Name);
                    command.Parameters.AddWithValue("@price", item.Price);
                    command.Parameters.AddWithValue("@description", item.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@imageUrl", item.ImageUrl ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@categoryId", item.CategoryID);
                    command.Parameters.AddWithValue("@isAvailable", item.IsAvailable);
                    command.Parameters.AddWithValue("@isFeatured", item.IsFeatured);
                    command.Parameters.AddWithValue("@id", item.ItemID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating menu item: {ex.Message}", ex);
            }
        }

        public void DeleteMenuItem(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("DELETE FROM MenuItems WHERE ItemID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting menu item: {ex.Message}", ex);
            }
        }
        #endregion

        #region Order Methods
        public List<Order> GetOrders()
        {
            var orders = new List<Order>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT OrderID, CustomerName, CustomerEmail, CustomerPhone, SpecialInstructions, 
                               TotalAmount, OrderDate, Status 
                        FROM Orders 
                        ORDER BY OrderDate DESC", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderID = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                CustomerEmail = reader.GetString(2),
                                CustomerPhone = reader.GetString(3),
                                SpecialInstructions = reader.IsDBNull(4) ? null : reader.GetString(4),
                                TotalAmount = reader.GetDecimal(5),
                                OrderDate = reader.GetDateTime(6),
                                Status = (OrderStatus)reader.GetInt32(7)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting orders: {ex.Message}", ex);
            }
            return orders;
        }

        public Order GetOrder(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT OrderID, CustomerName, CustomerEmail, CustomerPhone, SpecialInstructions, 
                               TotalAmount, OrderDate, Status 
                        FROM Orders 
                        WHERE OrderID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Order
                            {
                                OrderID = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                CustomerEmail = reader.GetString(2),
                                CustomerPhone = reader.GetString(3),
                                SpecialInstructions = reader.IsDBNull(4) ? null : reader.GetString(4),
                                TotalAmount = reader.GetDecimal(5),
                                OrderDate = reader.GetDateTime(6),
                                Status = (OrderStatus)reader.GetInt32(7)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order: {ex.Message}", ex);
            }
            return null;
        }

        public List<Order> GetOrdersByEmail(string email)
        {
            var orders = new List<Order>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT OrderID, CustomerName, CustomerEmail, CustomerPhone, SpecialInstructions, 
                               TotalAmount, OrderDate, Status 
                        FROM Orders 
                        WHERE CustomerEmail = @email
                        ORDER BY OrderDate DESC", connection);
                    command.Parameters.AddWithValue("@email", email);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                OrderID = reader.GetInt32(0),
                                CustomerName = reader.GetString(1),
                                CustomerEmail = reader.GetString(2),
                                CustomerPhone = reader.GetString(3),
                                SpecialInstructions = reader.IsDBNull(4) ? null : reader.GetString(4),
                                TotalAmount = reader.GetDecimal(5),
                                OrderDate = reader.GetDateTime(6),
                                Status = (OrderStatus)reader.GetInt32(7)
                            });
                        }
                    }
                }

                // Load order details for each order
                foreach (var order in orders)
                {
                    order.OrderDetails = GetOrderDetails(order.OrderID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting orders by email: {ex.Message}", ex);
            }

            return orders;
        }

        public int CreateOrder(Order order)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        INSERT INTO Orders (CustomerName, CustomerEmail, CustomerPhone, SpecialInstructions, 
                                          TotalAmount, OrderDate, Status) 
                        VALUES (@customerName, @customerEmail, @customerPhone, @specialInstructions, 
                               @totalAmount, @orderDate, @status);
                        SELECT SCOPE_IDENTITY();", connection);
                    
                    command.Parameters.AddWithValue("@customerName", order.CustomerName);
                    command.Parameters.AddWithValue("@customerEmail", order.CustomerEmail);
                    command.Parameters.AddWithValue("@customerPhone", order.CustomerPhone);
                    command.Parameters.AddWithValue("@specialInstructions", order.SpecialInstructions ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
                    command.Parameters.AddWithValue("@orderDate", order.OrderDate);
                    command.Parameters.AddWithValue("@status", (int)order.Status);
                    
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating order: {ex.Message}", ex);
            }
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("UPDATE Orders SET Status = @status WHERE OrderID = @id", connection);
                    command.Parameters.AddWithValue("@status", (int)status);
                    command.Parameters.AddWithValue("@id", orderId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating order status: {ex.Message}", ex);
            }
        }

        public void AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        INSERT INTO OrderDetails (OrderID, ItemID, Quantity, UnitPrice, Subtotal) 
                        VALUES (@orderId, @itemId, @quantity, @unitPrice, @subtotal)", connection);
                    
                    command.Parameters.AddWithValue("@orderId", orderDetail.OrderID);
                    command.Parameters.AddWithValue("@itemId", orderDetail.ItemID);
                    command.Parameters.AddWithValue("@quantity", orderDetail.Quantity);
                    command.Parameters.AddWithValue("@unitPrice", orderDetail.UnitPrice);
                    command.Parameters.AddWithValue("@subtotal", orderDetail.Subtotal);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding order detail: {ex.Message}", ex);
            }
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            var orderDetails = new List<OrderDetail>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT od.OrderDetailID, od.OrderID, od.ItemID, od.Quantity, od.UnitPrice, od.Subtotal, m.Name
                        FROM OrderDetails od
                        INNER JOIN MenuItems m ON od.ItemID = m.ItemID
                        WHERE od.OrderID = @orderId", connection);
                    command.Parameters.AddWithValue("@orderId", orderId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var orderDetail = new OrderDetail
                            {
                                OrderDetailID = reader.GetInt32(0),
                                OrderID = reader.GetInt32(1),
                                ItemID = reader.GetInt32(2),
                                Quantity = reader.GetInt32(3),
                                UnitPrice = reader.GetDecimal(4),
                                Subtotal = reader.GetDecimal(5),
                                MenuItem = new MenuItem { Name = reader.GetString(6) }
                            };
                            orderDetails.Add(orderDetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting order details: {ex.Message}", ex);
            }
            return orderDetails;
        }
        #endregion

        #region Statistics Methods
        public int GetTotalMenuItems()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT COUNT(*) FROM MenuItems", connection);
                    return (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total menu items: {ex.Message}", ex);
            }
        }

        public int GetTotalCategories()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT COUNT(*) FROM Categories", connection);
                    return (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total categories: {ex.Message}", ex);
            }
        }

        public int GetTotalOrders()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT COUNT(*) FROM Orders", connection);
                    return (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total orders: {ex.Message}", ex);
            }
        }

        public int GetPendingOrdersCount()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE Status IN (0, 1, 2)", connection);
                    return (int)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pending orders count: {ex.Message}", ex);
            }
        }

        public decimal GetTodaysRevenue()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"
                        SELECT ISNULL(SUM(TotalAmount), 0) 
                        FROM Orders 
                        WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE) 
                        AND Status = 4", connection);
                    var result = command.ExecuteScalar();
                    return result == DBNull.Value ? 0 : (decimal)result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting today's revenue: {ex.Message}", ex);
            }
        }
        #endregion

        public void Dispose()
        {
            // Nothing to dispose in this implementation
        }
    }
}