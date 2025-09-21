# ?? RESTAURANT ORDER SYSTEM - STARTUP GUIDE

## **?? WHAT TO EXECUTE FIRST - COMPLETE STARTUP INSTRUCTIONS**

### **? PROJECT STATUS:**
- ? **Build Status:** SUCCESS - All errors fixed
- ? **Project Type:** ASP.NET MVC Web Application (.NET Framework 4.8)
- ? **Database:** Auto-initialization configured
- ? **All Components:** Ready to run

---

## **?? IMMEDIATE STARTUP STEPS:**

### **1. START THE APPLICATION (Choose One Method):**

#### **?? Method A: Debug Mode (Recommended for Development)**
```
Press F5 in Visual Studio
```
- **What this does:** Starts with debugging enabled
- **Browser opens at:** `https://localhost:44300/`
- **Benefits:** Can set breakpoints, see detailed errors

#### **?? Method B: Run Without Debugging**
```
Press Ctrl + F5 in Visual Studio
```
- **What this does:** Starts faster without debugger
- **Browser opens at:** `https://localhost:44300/`
- **Benefits:** Faster startup, production-like experience

#### **?? Method C: From Visual Studio Menu**
```
Debug ? Start Debugging (F5)
OR
Debug ? Start Without Debugging (Ctrl+F5)
```

---

## **? WHAT HAPPENS WHEN YOU START:**

### **?? Automatic Initialization Sequence:**
1. **IIS Express starts** (hosts your web application)
2. **Global.asax.cs Application_Start() executes:**
   - `RestaurantDbInitializer.Initialize()` - Creates database & sample data
   - MVC routes registration
   - Bundle configuration for CSS/JS
3. **Database Creation** (if first run):
   - Creates `ROS` database on your SQL Server
   - Seeds with sample categories and menu items
4. **Homepage loads** at `https://localhost:44300/`

---

## **?? FIRST-TIME SETUP VERIFICATION:**

### **Before Starting - Quick Checklist:**

#### **? 1. SQL Server Running:**
```powershell
# Check if SQL Server is running
services.msc
# Look for: SQL Server (SQLEXPRESS) - should be "Running"
```

#### **? 2. Connection String Check:**
Your current connection: `Data Source=HEMIL\SQLEXPRESS;Initial Catalog=ROS;Integrated Security=True;Encrypt=False`

**If you have different SQL Server instance, update Web.config:**
```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ROS;Integrated Security=True;Encrypt=False"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

#### **? 3. Build Verification:**
```
Build ? Rebuild Solution
```
- Should show: "Build succeeded"

---

## **?? WHAT YOU'LL SEE AFTER STARTUP:**

### **?? Homepage Features:**
- **Welcome message** with restaurant branding
- **Quick statistics** (Total menu items, orders, categories)
- **Featured menu items** with images and prices
- **Navigation menu** (Home, Menu, Cart, Admin)
- **Responsive design** that works on mobile

### **??? Available Pages:**
- **`/`** - Homepage with featured items
- **`/Menu`** - Browse full menu with categories
- **`/Cart`** - Shopping cart management
- **`/Order/Track`** - Track orders by email
- **`/Admin/Dashboard`** - Admin panel with statistics

---

## **?? STARTUP TROUBLESHOOTING:**

### **?? If Application Doesn't Start:**

#### **Issue 1: Port Already in Use**
```
Error: "Port 44300 is already in use"
Solution: 
1. Stop IIS Express from system tray
2. Or change port in project properties
```

#### **Issue 2: SQL Server Connection Failed**
```
Error: "Cannot open database 'ROS'"
Solutions:
1. Start SQL Server service
2. Update connection string in Web.config
3. Use (localdb)\MSSQLLocalDB instead
```

#### **Issue 3: Build Errors**
```
Error: CS0234 or similar compilation errors
Solution: The project is already fixed, but if issues persist:
1. Clean Solution (Build ? Clean Solution)
2. Rebuild Solution (Build ? Rebuild Solution)
```

---

## **?? TESTING YOUR APPLICATION:**

### **?? Step-by-Step Test Plan:**

#### **1. Homepage Test (/):**
- ? Page loads without errors
- ? Statistics show numbers
- ? Featured items display
- ? Navigation menu works

#### **2. Menu Test (/Menu):**
- ? All menu items display
- ? Categories filter works
- ? Search functionality works
- ? "Add to Cart" buttons work

#### **3. Cart Test (/Cart):**
- ? Items appear in cart
- ? Quantity can be updated
- ? Items can be removed
- ? Total calculates correctly

#### **4. Admin Test (/Admin/Dashboard):**
- ? Dashboard loads with statistics
- ? Can navigate to menu management
- ? Can add/edit menu items
- ? Can view orders

---

## **?? DATABASE SAMPLE DATA:**

### **?? Pre-loaded Content:**
After first startup, your database will contain:

#### **Categories:**
- Appetizers
- Main Courses  
- Desserts
- Beverages

#### **Sample Menu Items:**
- Chicken Wings ($12.99)
- Caesar Salad ($8.99)
- Grilled Salmon ($18.99)
- Chocolate Cake ($6.99)
- And more...

---

## **?? SUCCESS INDICATORS:**

### **? Application Started Successfully When:**
1. **Browser opens** to `https://localhost:44300/`
2. **Homepage displays** with restaurant content
3. **Menu page works** (`/Menu`) showing food items
4. **No error messages** in browser or Visual Studio
5. **Admin panel accessible** (`/Admin/Dashboard`)

---

## **?? NEXT STEPS AFTER STARTUP:**

### **?? Explore Features:**
1. **Browse the menu** - See categories and items
2. **Add items to cart** - Test shopping functionality
3. **Place a test order** - Go through checkout process
4. **Access admin panel** - Manage menu items and orders
5. **Track orders** - Test order tracking by email

### **??? Customization Options:**
1. **Add your own menu items** via Admin panel
2. **Upload restaurant logo** and branding
3. **Modify categories** to match your restaurant
4. **Update contact information** in views
5. **Customize styling** in Content/Site.css

---

## **?? YOU'RE READY TO GO!**

**Your Restaurant Order System is now fully functional and ready for customers!**

**Just press F5 in Visual Studio and start exploring your complete restaurant management platform!** ????

---

### **?? Need Help?**
- **Check:** Error List window in Visual Studio for any issues
- **Check:** Output window for detailed startup logs
- **Check:** Browser Developer Tools (F12) for client-side errors