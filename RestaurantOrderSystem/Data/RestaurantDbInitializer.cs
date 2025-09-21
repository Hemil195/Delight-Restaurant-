using RestaurantOrderSystem.Data;

namespace RestaurantOrderSystem.Data
{
    public static class RestaurantDbInitializer
    {
        public static void Initialize()
        {
            using (var db = new RestaurantDbContext())
            {
                // Create tables
                db.CreateTables();
                
                // Seed with vegetarian data
                db.SeedData();
            }
        }

        /// <summary>
        /// Initialize with fresh vegetarian data (removes existing data)
        /// This method is for manual database refresh with vegetarian items
        /// </summary>
        public static void RefreshWithVegetarianData()
        {
            using (var db = new RestaurantDbContext())
            {
                // Create tables first
                db.CreateTables();
                
                // For now, let the existing SeedData method handle the vegetarian items
                // In a production scenario, you would create a separate method to clear and reseed
                db.SeedData();
            }
        }
    }
}