# Images Folder Structure

This document explains where to place images in your Restaurant Order System.

## Directory Structure

```
Content/
??? images/
    ??? menu/           # Menu item images
    ??? categories/     # Category images
    ??? default/        # Default/placeholder images
```

## Where to Put Images

### For Menu Items
- **Location**: `Content/images/menu/`
- **Naming**: Use descriptive names like `paneer-tikka.jpg`, `veg-biryani.jpg`
- **Formats**: .jpg, .jpeg, .png, .gif
- **Recommended Size**: 800x600 pixels or similar aspect ratio
- **Usage**: Referenced in database ImageUrl field as `/images/menu/filename.jpg`

### For Categories  
- **Location**: `Content/images/categories/`
- **Naming**: Use category names like `appetizers.jpg`, `main-courses.jpg`
- **Formats**: .jpg, .jpeg, .png, .gif
- **Recommended Size**: 800x600 pixels or similar aspect ratio
- **Usage**: Referenced in database ImageUrl field as `/images/categories/filename.jpg`

## Example URLs in Database

When adding menu items or categories, use these URL patterns:

### Menu Items
- `/images/menu/paneer-tikka.jpg`
- `/images/menu/veg-biryani.jpg` 
- `/images/menu/dal-makhani.jpg`

### Categories
- `/images/categories/appetizers.jpg`
- `/images/categories/main-courses.jpg`
- `/images/categories/beverages.jpg`

## Default Images

The system will automatically fall back to placeholder images if:
- No image URL is provided
- The specified image file doesn't exist
- There's an error loading the image

Default fallback paths:
- Menu items: `/images/menu/default-food.jpg`
- Categories: `/images/categories/default-category.jpg`

## Adding Images

1. **Place image files** in the appropriate directory (`Content/images/menu/` or `Content/images/categories/`)
2. **Use admin panel** to add/edit menu items and categories
3. **Set ImageUrl field** to the relative path (e.g., `/images/menu/your-image.jpg`)
4. **Test the image** appears correctly on the website

## Tips

- Use compressed, web-optimized images for better performance
- Maintain consistent aspect ratios for better visual consistency
- Use descriptive filenames without spaces (use hyphens instead)
- Keep file sizes reasonable (under 500KB recommended)