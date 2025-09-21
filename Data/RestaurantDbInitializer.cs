using RestaurantOrderSystem.Data;
using System;

namespace RestaurantOrderSystem.Data
{
    public static class RestaurantDbInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var db = new RestaurantDbContext())
                {
                    // Create tables first
                    db.CreateTables();
                    
                    // Then seed with vegetarian data
                    db.SeedData();
                }
            }
            catch (Exception ex)
            {
                // Log the error (in a real application, you would use a logging framework)
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                throw new Exception($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Initialize with fresh vegetarian data (removes existing data)
        /// This method is for manual database refresh with vegetarian items
        /// </summary>
        public static void RefreshWithVegetarianData()
        {
            try
            {
                using (var db = new RestaurantDbContext())
                {
                    // Create tables first
                    db.CreateTables();
                    
                    // Clear existing data and reseed with corrected image paths
                    db.RefreshDataWithCorrectImagePaths();
                }
            }
            catch (Exception ex)
            {
                // Log the error (in a real application, you would use a logging framework)
                System.Diagnostics.Debug.WriteLine($"Database refresh error: {ex.Message}");
                throw new Exception($"Failed to refresh database: {ex.Message}", ex);
            }
        }
    }
}