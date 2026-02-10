# Task Management System - Architecture Summary

## 3-Tier Architecture Overview

This project follows a clean 3-tier architecture with the following structure:

### 1. Data Access Layer (TaskManager.DataAccess)
**Purpose:** Manages all database operations and entity definitions

#### Folders:
- **Models/** - Entity classes
  - `ApplicationUser.cs` - User entity (extends IdentityUser)
  - `UserTask.cs` - Task entity (renamed from Task to avoid conflicts)
  - `Category.cs` - Category entity
  
- **Enums/** - Enumeration types
  - `TaskPriority.cs` - Low, Medium, High, Critical
  - `TaskStatus.cs` - ToDo, Pending, Postponed, Completed
  
- **Data/** - Database context and initialization
  - `ApplicationDbContext.cs` - EF Core DbContext
  - `DbInitializer.cs` - Database seeding and user category initialization
  
- **Repositories/** - Data access repositories
  - `IRepository.cs` / `Repository.cs` - Generic repository pattern
  - `ITaskRepository.cs` / `TaskRepository.cs` - Task-specific operations
  - `ICategoryRepository.cs` / `CategoryRepository.cs` - Category-specific operations

### 2. Business Layer (TaskManager.Business)
**Purpose:** Contains business logic and data transfer objects

#### Folders:
- **DTOs/** - Data Transfer Objects (currently placeholders)
  - `TaskDto.cs`
  - `CategoryDto.cs`
  - `DashboardDto.cs`
  - `RecommendedTaskDto.cs`
  
- **Services/** - Business logic services (currently placeholders)
  - `ITaskService.cs` / `TaskService.cs`
  - `ICategoryService.cs` / `CategoryService.cs`
  - `IDashboardService.cs` / `DashboardService.cs`
  - `IRecommendationService.cs` / `RecommendationService.cs`

### 3. Presentation Layer (TaskManager)
**Purpose:** ASP.NET Core MVC web application

#### Folders:
- **Controllers/** - MVC Controllers (currently placeholders)
  - `HomeController.cs` - Default home controller
  - `AccountController.cs` - Authentication (Register/Login/Logout)
  - `TaskController.cs` - Task CRUD operations
  - `CategoryController.cs` - Category CRUD operations
  - `DashboardController.cs` - Dashboard and analytics
  
- **ViewModels/** - View-specific models (currently placeholders)
  - `LoginViewModel.cs` / `RegisterViewModel.cs`
  - `TaskViewModel.cs` / `CreateTaskViewModel.cs` / `EditTaskViewModel.cs`
  - `CategoryViewModel.cs`
  - `DashboardViewModel.cs`
  
- **Views/** - Razor views (currently placeholders)
  - `Account/` - Login, Register, AccessDenied
  - `Task/` - Index, Create, Edit, Delete, Details
  - `Category/` - Index, Create, Edit, Delete
  - `Dashboard/` - Index
  - `Home/` - Index, Privacy
  - `Shared/` - _Layout, Error, etc.

## Key Design Decision: User-Specific Default Categories

### Modified Approach
Instead of having shared default categories, **each user gets their own copy** of default categories upon registration.

### Implementation:
1. **Category Model** - All categories now REQUIRE a UserId (removed IsDefault flag)
2. **ApplicationDbContext** - Removed seed data (no shared categories)
3. **CategoryRepository** - Added `CreateDefaultCategoriesForUserAsync()` method
4. **DbInitializer** - Provides template for default categories and initialization method

### Default Categories Created Per User:
- Work
- Personal
- Shopping
- Health
- Education
- Finance

### How It Works:
When a new user registers, the system will call:
```csharp
await categoryRepository.CreateDefaultCategoriesForUserAsync(userId);
```

This creates 6 personal categories for that user, allowing:
- Each user to have independent categories
- Users can modify or delete their default categories without affecting others
- Complete data isolation between users

## Next Steps for Implementation:
1. Implement DTOs in Business Layer
2. Implement Services with business logic
3. Implement ViewModels in Presentation Layer
4. Implement Controllers with actions
5. Implement Razor Views
6. Add database migrations
7. Configure dependency injection in Program.cs
8. Hook up category initialization in registration process
9. Implement authentication and authorization
10. Implement dashboard with statistics and recommendations

## NuGet Packages Installed:
- **TaskManager.DataAccess:**
  - Microsoft.EntityFrameworkCore (8.0.11)
  - Microsoft.EntityFrameworkCore.SqlServer (8.0.11)
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.11)
  
- **TaskManager (Web):**
  - Microsoft.EntityFrameworkCore.Tools (8.0.11)
  - Microsoft.AspNetCore.Identity.UI (8.0.11)

## Important Naming Note:
The Task entity is named **`UserTask`** (not `Task`) to avoid conflicts with `System.Threading.Tasks.Task`.
