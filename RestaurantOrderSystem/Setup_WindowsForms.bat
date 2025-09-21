@echo off
echo ==================================================
echo Restaurant Order System - Windows Forms Setup
echo ==================================================
echo.
echo This script will help you set up Windows Forms support
echo for the Restaurant Order System project.
echo.
echo Manual Steps Required:
echo.
echo 1. Open Visual Studio
echo 2. Right-click on the RestaurantOrderSystem project
echo 3. Select "Properties"
echo 4. In Application tab, change "Output type" from "Library" to "Windows Application"
echo 5. Right-click on "References" in Solution Explorer
echo 6. Select "Add Reference..."
echo 7. In .NET tab, check:
echo    - System.Windows.Forms
echo    - System.Data
echo.
echo 8. To run Windows Forms version:
echo    - Comment out the console menu code in Program.cs
echo    - Uncomment the Windows Forms code:
echo      Application.EnableVisualStyles();
echo      Application.SetCompatibleTextRenderingDefault(false);
echo      Application.Run(new MenuForm());
echo.
echo Files ready for Windows Forms:
echo - Forms\MenuForm.cs
echo - Forms\MenuForm.Designer.cs
echo - Program.cs (contains both console and forms code)
echo.
echo ==================================================
echo Console version is currently active and ready to run!
echo ==================================================
pause