# Authentication Demo Guide

## Overview
This project demonstrates a **mock authentication system** with permission-based UI in a Blazor WebAssembly application.

## Mock Users

### User 1: Admin (Full Permissions)
- **Username:** `Admin`
- **Password:** `password`
- **Role:** Administrator
- **Permissions:**
  - ✅ character.view
  - ✅ character.create
  - ✅ character.edit
  - ✅ character.delete

### User 2: User (Limited Permissions)
- **Username:** `User`
- **Password:** `password`
- **Role:** Regular User
- **Permissions:**
  - ✅ character.view
  - ❌ character.create
  - ❌ character.edit
  - ❌ character.delete

## How It Works

### Backend (Back-EndAPI/Services/AuthService.cs)
The `LoginSimple` method checks credentials and generates different JWT tokens based on the user:
- **Admin** gets a token with all 4 permissions
- **User** gets a token with only view permission

The permissions are embedded in the JWT token as **claims**.

### Frontend (Front-EndAPI)

#### 1. Login Page (`/login`)
- Shows available users with their permission levels
- Authenticates users and stores JWT token
- Redirects to home page on success

#### 2. Home Page (`/`)
- **Displays User Info:**
  - Username
  - Role
  - List of permissions (extracted from JWT token)

- **Permission-Based Buttons:**
  - 🔵 **View Characters** - Enabled if user has `character.view` permission
  - 🟢 **Create Character** - Enabled if user has `character.create` permission
  - 🟡 **Edit Character** - Enabled if user has `character.edit` permission
  - 🔴 **Delete Character** - Enabled if user has `character.delete` permission

- **Button Behavior:**
  - ✅ Enabled (clickable) if user has the required permission
  - ❌ Disabled (grayed out) if user lacks the permission

#### 3. Authorization Check
- Uses **JWT token claims** (NOT a list passed separately)
- Blazor's `context.User.HasClaim("permission", "character.view")` checks the token
- Buttons use `disabled="@(!context.User.HasClaim(...))"` to gray out

## Testing Instructions

### Test as Admin (Full Access)
1. Navigate to `/login`
2. Enter credentials: `Admin` / `password`
3. Click Login
4. **Expected Result:**
   - All 4 buttons are enabled (blue, green, yellow, red)
   - Permissions list shows 4 items
   - Can click "Create Character" to show the form

### Test as User (Limited Access)
1. Click "Logout" in the top-right
2. Navigate to `/login`
3. Enter credentials: `User` / `password`
4. Click Login
5. **Expected Result:**
   - Only "View Characters" button is enabled (blue)
   - Other 3 buttons are grayed out and unclickable
   - Permissions list shows only 1 item: `character.view`
   - Cannot access create form

## Key Files Modified

### Backend
- `Back-EndAPI/Services/AuthService.cs` - Added two mock users with different permissions

### Frontend
- `Front-EndAPI/Pages/Login.razor` - Updated to show available users
- `Front-EndAPI/Pages/Index.razor` - Added permission display and permission-based buttons
- `Front-EndAPI/Pages/Index.razor.cs` - Added ShowCreateForm property

## Security Note
⚠️ This is a **mock system for demonstration purposes**:
- No database connection
- Passwords are hardcoded
- No password hashing
- Token secret key is hardcoded

For production use, implement:
- Database-backed user storage
- Password hashing (bcrypt/Argon2)
- Secure token generation
- Environment variables for secrets
