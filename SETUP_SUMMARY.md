# BeaTraction Database Setup Summary

## What Was Done

### 1. Domain Entities Created
Created entity classes in `BeaTraction.Domain/Entities/`:
- `User.cs` - User entity with name, email, role
- `Schedule.cs` - Schedule entity with name, start/end time
- `Attraction.cs` - Attraction entity with description, capacity, and image URL
- `Registration.cs` - Registration entity linking users to attractions

### 2. AppDbContext Created
Created `BeaTraction.Infrastructure/Persistence/AppDbContext.cs` with:
- DbSet properties for all entities
- Complete entity configurations matching your PostgreSQL schema
- Proper column mappings (snake_case)
- Foreign key relationships with cascade delete
- Unique constraints (email, user-attraction combination)
- Default values (gen_random_uuid(), NOW(), etc.)

### 3. NuGet Packages Added

**BeaTraction.Infrastructure.csproj:**
- `Microsoft.EntityFrameworkCore` (8.0.10)
- `Microsoft.EntityFrameworkCore.Design` (8.0.10)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.10)
- Added reference to `BeaTraction.Domain`

**BeaTraction.WebAPI.csproj:**
- `Microsoft.EntityFrameworkCore.Design` (8.0.10)

### 4. Configuration Updated

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=beatraction;Username=postgres;Password=postgres"
  }
}
```

**Program.cs:**
Added DbContext registration:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 5. Migration Created
Successfully created migration: `20251008034300_InitialCreate`

## Next Steps

### To Apply the Migration to Your Database:

1. **Make sure your PostgreSQL server is running** on port 5432

2. **Create the database** (if it doesn't exist):
   ```bash
   # Using psql
   psql -U postgres
   CREATE DATABASE beatraction;
   \q
   ```

3. **Update the connection string** in `appsettings.json` with your actual PostgreSQL credentials

4. **Apply the migration**:
   ```powershell
   cd d:\ASLAB\TECH-TEST\RDT\BeaTraction
   C:\Users\bertr\.dotnet\tools\dotnet-ef.exe database update --project BeaTraction.Infrastructure --startup-project BeaTraction.WebAPI
   ```
   
   Or add the tools directory to your PATH and use:
   ```powershell
   dotnet ef database update --project BeaTraction.Infrastructure --startup-project BeaTraction.WebAPI
   ```

### To Add the dotnet-ef to PATH (Optional):
```powershell
setx PATH "%PATH%;C:\Users\bertr\.dotnet\tools"
```
Then restart your terminal.

## Database Schema

The migration will create the following tables:
- `users` - User accounts with roles
- `schedules` - Event schedules
- `attractions` - Attractions within schedules
- `registrations` - User registrations for attractions

All tables include:
- UUID primary keys (auto-generated)
- `row_version` for optimistic concurrency
- Timestamps where applicable
- Proper foreign key relationships with cascade delete

## Note on Triggers

The SQL file includes triggers for updating `row_version`. EF Core migrations don't include these by default. You have two options:

1. **Use EF Core's concurrency handling** (recommended):
   - EF Core will handle row versioning automatically through its concurrency tokens
   
2. **Add triggers manually** after running the migration:
   - Run your `create.sql` trigger definitions separately

The current setup uses EF Core's built-in concurrency handling with the `row_version` property.
