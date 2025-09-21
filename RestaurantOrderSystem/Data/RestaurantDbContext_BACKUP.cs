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

        #region Category Methods
        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
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
            return categories;
        }

        public Category GetCategory(int id)
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
            return null;
        }

        public void AddCategory(Category category)
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

        public void UpdateCategory(Category category)
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

        public void DeleteCategory(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Categories WHERE CategoryID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region MenuItem Methods
        public List<MenuItem> GetMenuItems()
        {
            var menuItems = new List<MenuItem>();
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
            return menuItems;
        }

        public List<MenuItem> GetMenuItemsByCategory(int categoryId)
        {
            var menuItems = new List<MenuItem>();
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
            return menuItems;
        }

        public List<MenuItem> GetFeaturedMenuItems()
        {
            var menuItems = new List<MenuItem>();
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
            return menuItems;
        }

        public MenuItem GetMenuItem(int id)
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
            return null;
        }

        public void AddMenuItem(MenuItem item)
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

        public void UpdateMenuItem(MenuItem item)
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

        public void DeleteMenuItem(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM MenuItems WHERE ItemID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Order Methods
        public List<Order> GetOrders()
        {
            var orders = new List<Order>();
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
            return orders;
        }

        public List<Order> GetOrdersByCustomerEmail(string email)
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                           TotalAmount, OrderDate, Status 
                    WHERE CustomerEmail = @email
                    ORDER BY OrderDate DESC", connection);
                command.Parameters.AddWithValue("@email", email);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            OrderID = reader.GetInt32(0),
                            CustomerName = reader.GetString(1),
                            CustomerEmail = reader.GetString(2),
                        };
                        
                        // Load order details
                        order.OrderDetails = GetOrderDetails(order.OrderID);
                        orders.Add(order);
                    }
                }
            }
            {
                connection.Open();
                var command = new SqlCommand(@"
                           TotalAmount, OrderDate, Status 
                command.Parameters.AddWithValue("@id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        {
                            OrderID = reader.GetInt32(0),
                            TotalAmount = reader.GetDecimal(5),
                            OrderDate = reader.GetDateTime(6),
                            Status = (OrderStatus)reader.GetInt32(7)
                        };
                        
                        return order;
                    }
                }
            }
            return null;
        }
                connection.Open();
                var command = new SqlCommand(@"
                
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

        public void UpdateOrderStatus(int orderId, OrderStatus status)
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

        public void AddOrderDetail(OrderDetail orderDetail)
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

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            var orderDetails = new List<OrderDetail>();
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
            return orderDetails;
        }
        #endregion

        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
            {
                connection.Open();
                var command = new SqlCommand("SELECT COUNT(*) FROM MenuItems", connection);
                return (int)command.ExecuteScalar();
            }
        }

        public int GetTotalOrders()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT COUNT(*) FROM Orders", connection);
                return (int)command.ExecuteScalar();
            }
        }

        public int GetPendingOrdersCount()
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
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE Status IN (0, 1, 2)", connection);
                return (int)command.ExecuteScalar();
            }
        }

        public decimal GetTodaysRevenue()
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
        #endregion

        #region Database Management
        public void UpdateDatabaseSchema()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Check if MenuItems table exists and update schema if needed
                var checkTableCommand = new SqlCommand(@"
                    IF EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
                    BEGIN
                        -- Check and add missing columns
                        IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('MenuItems') AND name = 'Description')
                            ALTER TABLE MenuItems ADD Description nvarchar(500)
                        
                        IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('MenuItems') AND name = 'ImageUrl')
                            ALTER TABLE MenuItems ADD ImageUrl nvarchar(255)
                        
                        IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('MenuItems') AND name = 'CategoryID')
                            ALTER TABLE MenuItems ADD CategoryID int NOT NULL DEFAULT 1
                        
                        IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('MenuItems') AND name = 'IsAvailable')
                            ALTER TABLE MenuItems ADD IsAvailable bit NOT NULL DEFAULT 1
                        
                        IF NOT EXISTS (SELECT * FROM syscolumns WHERE id = OBJECT_ID('MenuItems') AND name = 'IsFeatured')
                            ALTER TABLE MenuItems ADD IsFeatured bit NOT NULL DEFAULT 0
                    END", connection);
                
                checkTableCommand.ExecuteNonQuery();
                
                // Update existing menu items to have proper CategoryID if they don't
                var updateCategoryCommand = new SqlCommand(@"
                    IF EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
                    BEGIN
                        -- Ensure all items have a valid CategoryID
                        UPDATE MenuItems SET CategoryID = 1 WHERE CategoryID IS NULL OR CategoryID = 0
                    END", connection);
                
                updateCategoryCommand.ExecuteNonQuery();
            }
        }

        public void CreateTables()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Create Categories table
                var createCategoriesTable = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
                    CREATE TABLE Categories (
                        CategoryID int IDENTITY(1,1) PRIMARY KEY,
                        Name nvarchar(100) NOT NULL,
                        Description nvarchar(500),
                        ImageUrl nvarchar(255)
                    )";
                
                // Create MenuItems table
                var createMenuItemsTable = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
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
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
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
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderDetails' AND xtype='U')
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

        public void SeedData()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Check if categories exist
                var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Categories", connection);
                var categoryCount = (int)checkCommand.ExecuteScalar();
                
                if (categoryCount == 0)
                {
                    // Add sample categories
                    var categories = new[]
                    {
                        ("Appetizers", "Start your meal with our delicious appetizers", "/images/categories/appetizers.jpg"),
                        ("Main Courses", "Hearty and satisfying main dishes", "/images/categories/mains.jpg"),
                        ("Beverages", "Refreshing drinks and beverages", "/images/categories/beverages.jpg"),
                        ("Desserts", "Sweet treats to end your meal", "/images/categories/desserts.jpg"),
                        ("Salads", "Fresh and healthy salad options", "/images/categories/salads.jpg")
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
                    // Add sample menu items
                    var menuItems = new[]
                    {
                        ("Caesar Salad", 12.99m, "Fresh romaine lettuce with caesar dressing", "/images/menu/caesar-salad.jpg", 5, true, true),
                        ("Grilled Chicken", 18.99m, "Perfectly grilled chicken breast with herbs", "/images/menu/grilled-chicken.jpg", 2, true, true),
                        ("Margherita Pizza", 15.99m, "Classic pizza with tomato, mozzarella and basil", "/images/menu/margherita-pizza.jpg", 2, true, false),
                        ("Chocolate Cake", 8.99m, "Rich chocolate cake with chocolate frosting", "/images/menu/chocolate-cake.jpg", 4, true, true),
                        ("Coca Cola", 2.99m, "Refreshing soft drink", "/images/menu/coca-cola.jpg", 3, true, false),
                        ("Spring Rolls", 7.99m, "Crispy spring rolls with sweet and sour sauce", "/images/menu/spring-rolls.jpg", 1, true, false),
                        ("Beef Steak", 25.99m, "Tender beef steak cooked to perfection", "/images/menu/beef-steak.jpg", 2, true, true),
                        ("Greek Salad", 11.99m, "Fresh vegetables with feta cheese and olives", "/images/menu/greek-salad.jpg", 5, true, false),
                        ("Tiramisu", 9.99m, "Classic Italian dessert", "/images/menu/tiramisu.jpg", 4, true, false),
                        ("Fresh Orange Juice", 4.99m, "Freshly squeezed orange juice", "/images/menu/orange-juice.jpg", 3, true, false)
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
        #endregion

        public void Dispose()
        {
            // Nothing to dispose in this implementation
        }
    }
}