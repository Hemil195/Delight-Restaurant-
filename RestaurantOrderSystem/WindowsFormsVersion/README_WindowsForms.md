# Windows Forms Setup Instructions

## To Create a Windows Forms Project:

1. **Create New Windows Forms Application:**
   - In Visual Studio, create a new project
   - Select "Windows Forms App (.NET Framework)"
   - Choose .NET Framework 4.8
   - Name it "RestaurantMenuManager"

2. **Copy Required Files:**
   - Copy all files from the `Data` folder (RestaurantDbContext.cs, RestaurantDbInitializer.cs)
   - Copy all files from the `Models` folder (MenuItem.cs, Order.cs, OrderDetail.cs)
   - Copy MenuForm.cs and MenuForm.Designer.cs from this WindowsFormsVersion folder
   - Replace Program.cs with ProgramWinForms.cs content

3. **Add App.config:**
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <connectionStrings>
       <add name="DefaultConnection"
            connectionString="Data Source=HEMIL\SQLEXPRESS;Initial Catalog=ROS;Integrated Security=True;Encrypt=False"
            providerName="System.Data.SqlClient" />
     </connectionStrings>
     
     <startup>
       <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
     </startup>
   </configuration>
   ```

4. **Add References:**
   - System.Windows.Forms (should be included by default)
   - System.Data
   - System.Configuration

5. **Build and Run:**
   - Build the project
   - Run the application

## Features of the Windows Forms Application:

- **DataGridView** displays all menu items with formatted pricing
- **Text boxes** for entering/editing item name and price
- **Add button** (green) - adds new menu items
- **Update button** (blue) - updates selected menu item
- **Delete button** (red) - deletes selected menu item with confirmation
- **Clear button** - clears input fields
- **Refresh button** - reloads data from database
- **Input validation** ensures data integrity
- **Error handling** with user-friendly messages
- **Responsive design** with grouped controls

## Using the Application:

1. **View Items:** All menu items are displayed in the grid
2. **Select Item:** Click on any row to select it and populate the text boxes
3. **Add New Item:** Enter name and price, click Add
4. **Update Item:** Select item, modify name/price, click Update
5. **Delete Item:** Select item, click Delete, confirm deletion
6. **Clear Fields:** Click Clear to reset input fields

The application automatically handles database connections and updates the display after each operation.