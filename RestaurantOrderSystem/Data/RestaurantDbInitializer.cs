using RestaurantOrderSystem.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace RestaurantOrderSystem.Data
{
    public static class RestaurantDbInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();
        private static readonly string _initFlagFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "db_initialized.flag");

        public static void Initialize()
        {
            // Prevent multiple initializations during the same application run
            if (_isInitialized)
            {
                Debug.WriteLine("Database already initialized in this session, skipping...");
                return;
            }

            lock (_lockObject)
            {
                if (_isInitialized)
                    return;

                try
                {
                    // Ensure App_Data directory exists
                    string appDataDir = Path.GetDirectoryName(_initFlagFile);
                    if (!Directory.Exists(appDataDir))
                    {
                        Directory.CreateDirectory(appDataDir);
                    }

                    using (var db = new RestaurantDbContext())
                    {
                        Debug.WriteLine("Starting database initialization...");
                        
                        // Create tables first (this is safe to call multiple times)
                        db.CreateTables();
                        Debug.WriteLine("Tables created/verified successfully.");
                        
                        // Check if database already has data before seeding
                        bool hasData = CheckIfDatabaseHasData(db);
                        bool isFirstRun = !File.Exists(_initFlagFile);
                        
                        if (!hasData || isFirstRun)
                        {
                            if (isFirstRun)
                            {
                                Debug.WriteLine("First application run detected, initializing database...");
                            }
                            else
                            {
                                Debug.WriteLine("Database is empty, seeding with initial data...");
                            }
                            
                            db.SeedData();
                            
                            // Create initialization flag file
                            File.WriteAllText(_initFlagFile, $"Database initialized on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                            
                            Debug.WriteLine("Database seeded successfully with vegetarian menu data.");
                        }
                        else
                        {
                            Debug.WriteLine("Database already contains data, skipping seed operation.");
                        }
                    }
                    
                    _isInitialized = true;
                    Debug.WriteLine("Database initialization completed successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Database initialization error: {ex.Message}");
                    // Log the error but don't throw to prevent application startup failure
                    // In production, you might want to use a proper logging framework
                    throw new Exception($"Failed to initialize database: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Check if the database already contains data
        /// </summary>
        private static bool CheckIfDatabaseHasData(RestaurantDbContext db)
        {
            try
            {
                int categoryCount = db.GetTotalCategories();
                int menuItemCount = db.GetTotalMenuItems();
                
                Debug.WriteLine($"Found {categoryCount} categories and {menuItemCount} menu items in database.");
                
                return categoryCount > 0 && menuItemCount > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking database data: {ex.Message}");
                // If we can't check, assume empty and try to seed
                return false;
            }
        }

        /// <summary>
        /// Force refresh with fresh vegetarian data (removes existing data)
        /// This method is for manual database refresh - USE WITH CAUTION
        /// </summary>
        public static void RefreshWithVegetarianData()
        {
            try
            {
                using (var db = new RestaurantDbContext())
                {
                    Debug.WriteLine("Starting forced database refresh...");
                    
                    // Create tables first
                    db.CreateTables();
                    
                    // Just reseed the data (SeedData method already checks for existing data)
                    // If you want to force refresh, you would need to clear data first
                    db.SeedData();
                    
                    // Update initialization flag file
                    if (File.Exists(_initFlagFile))
                    {
                        File.WriteAllText(_initFlagFile, $"Database refreshed on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    }
                    
                    Debug.WriteLine("Database refresh completed successfully.");
                }
                
                // Reset the initialization flag
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database refresh error: {ex.Message}");
                throw new Exception($"Failed to refresh database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reset initialization (deletes flag file and allows fresh initialization)
        /// </summary>
        public static void ResetInitialization()
        {
            try
            {
                if (File.Exists(_initFlagFile))
                {
                    File.Delete(_initFlagFile);
                    Debug.WriteLine("Initialization flag file deleted.");
                }
                _isInitialized = false;
                Debug.WriteLine("Database initialization reset completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resetting initialization: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if database has been initialized
        /// </summary>
        public static bool IsInitialized => _isInitialized || File.Exists(_initFlagFile);

        /// <summary>
        /// Get initialization status information
        /// </summary>
        public static string GetInitializationStatus()
        {
            if (File.Exists(_initFlagFile))
            {
                try
                {
                    string content = File.ReadAllText(_initFlagFile);
                    return content;
                }
                catch
                {
                    return "Initialization flag file exists but cannot be read.";
                }
            }
            return "Database not yet initialized.";
        }
    }
}