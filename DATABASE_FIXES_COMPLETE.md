# Database and Image Setup Guide

## Database Issues Fixed ?

1. **Table Creation Error**: Fixed by ensuring `CreateTables()` is called before `SeedData()`
2. **Connection String**: Updated to use LocalDB instead of specific SQL Server instance
3. **Error Handling**: Added comprehensive try-catch blocks for better error reporting
4. **Image Paths**: Corrected all image paths to use `/Content/images/` format

## Image Upload Instructions

Your images should be placed in these exact locations:

### Menu Item Images
Place in: `RestaurantOrderSystem\Content\images\menu\`

**Required files** (based on your database):
- paneer-tikka.png
- veg-biryani.png
- dal-makhani.png
- gulab-jamun.png
- lime-soda.png
- spring-rolls.png
- palak-paneer.png
- garden-salad.png
- ras-malai.png
- mango-lassi.png
- aloo-gobi.png
- butter-naan.png
- jeera-rice.png
- masala-chai.png
- rajma.png

### Category Images  
Place in: `RestaurantOrderSystem\Content\images\categories\`

**Required files** (based on your database):
- appetizers.jpg
- mains.jpg
- beverages.jpg
- desserts.jpg
- salads.jpg
- rice.jpg
- breads.jpg

## How to Test

1. **Build the project**: Press Ctrl+Shift+B
2. **Run the application**: Press F5
3. **Check browser console** for any JavaScript errors
4. **Navigate to different pages** to test functionality

## Database Connection Test

The application will automatically:
1. Create LocalDB database on first run
2. Create all required tables
3. Seed with sample vegetarian data
4. Handle missing images gracefully with placeholders

## Troubleshooting

If you still get errors:

1. **Check Output Window**: View ? Output ? Show output from: Build
2. **Check Browser Console**: F12 ? Console tab
3. **Verify LocalDB**: Ensure SQL Server LocalDB is installed
4. **Database Permissions**: Make sure application has write access to create database

## Next Steps

1. ? Project builds successfully
2. ? Database connection updated
3. ? Error handling added
4. ?? Add your actual image files to the specified directories
5. ?? Run and test the application

All major errors have been fixed!