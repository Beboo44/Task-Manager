# Azure AD Passwordless Authentication - Setup Guide

## What I Updated

You chose Azure AD (Microsoft Entra) passwordless authentication instead of SQL authentication. This is more secure and modern!

### Changes Made

1. Updated appsettings.Production.json with passwordless connection string
2. Updated appsettings.Development.json with passwordless connection string
3. Installed Azure.Identity package (for Azure AD authentication)
4. Installed Microsoft.Data.SqlClient package (latest version with Azure AD support)
5. Build successful!

## Your New Connection String

```
Server=tcp:taskmanager-sql-abanoub.database.windows.net,1433;
Initial Catalog=TaskManagerDb;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
Authentication=Active Directory Default;
```

Notice: 
- No User ID needed
- No Password needed  
- Uses your Azure AD account automatically!

## Setup Steps Required

### Step 1: Configure Your Azure SQL Server

Go to Azure Portal:

1. Navigate to your SQL Server (taskmanager-sql-abanoub)
2. Click "Microsoft Entra admin" in left menu (formerly "Active Directory admin")
3. Click "Set admin"
4. Search for your email/account
5. Select your account
6. Click "Select"
7. Click "Save"

This makes YOU the admin of the database!

### Step 2: Login to Azure (For Local Development)

You need to login to Azure on your local machine:

**Option A: Using Azure CLI** (Recommended)

```bash
# Install Azure CLI if not already installed
# Download from: https://aka.ms/installazurecliwindows

# Login to Azure
az login

# Verify you're logged in
az account show
```

**Option B: Using Visual Studio**

1. Open Visual Studio
2. Go to Tools ? Options
3. Azure Service Authentication
4. Click "Account Selection"
5. Add your Azure account
6. Make sure the correct account is selected

### Step 3: Grant Database Access to Your Account

After setting yourself as Azure AD admin:

**Connect to the database and run:**

```sql
-- Replace with your actual email
CREATE USER [your.email@domain.com] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [your.email@domain.com];
```

Or use Azure Portal:

1. Go to your SQL Database (TaskManagerDb)
2. Click "Query editor" in left menu
3. Login using Azure Active Directory authentication
4. Run the SQL above

## For Production (Azure App Service)

When you deploy to Azure App Service, you need to enable Managed Identity:

### Step 1: Enable Managed Identity on App Service

In Azure Portal:

1. Go to your App Service
2. Click "Identity" in left menu
3. Switch "System assigned" to "On"
4. Click "Save"
5. Copy the Object ID shown

### Step 2: Grant Database Access to Managed Identity

Connect to SQL Database and run:

```sql
-- Replace with your App Service name
CREATE USER [your-app-service-name] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [your-app-service-name];
```

## Testing Locally

### Before Testing:

1. Azure AD admin set on SQL Server
2. Logged into Azure (az login or Visual Studio)
3. Database access granted to your account
4. Firewall allows your IP

### Test Your App:

```bash
# Run the app
dotnet run --project TaskManager

# Or in Visual Studio:
Press F5
```

### What Should Happen:

- App starts successfully
- No login prompts
- Database connection works
- Migrations run automatically
- You can register/login
- You can create tasks

## Firewall Configuration

You still need to configure firewall!

In Azure Portal:

1. Go to SQL Server (taskmanager-sql-abanoub)
2. Click "Firewalls and virtual networks"
3. Enable:
   - "Allow Azure services and resources to access this server"
   - Add your current client IP address
4. Click "Save"

## Your Current Status

### Completed:

- Azure SQL Database created
- Connection strings updated (passwordless)
- Azure.Identity package installed
- Microsoft.Data.SqlClient package installed
- Build successful

### TODO Before Testing:

- Set yourself as Azure AD admin on SQL Server
- Login to Azure locally (az login)
- Grant database access to your account
- Configure firewall rules
- Test locally

### TODO Before Production Deployment:

- Create Azure App Service
- Enable Managed Identity on App Service
- Grant database access to Managed Identity
- Deploy your app

## Quick Setup Commands

### 1. Login to Azure:

```bash
az login
```

### 2. Set Azure AD Admin:

```bash
# Replace with your email
az sql server ad-admin create \
  --resource-group TaskManagerRG \
  --server-name taskmanager-sql-abanoub \
  --display-name "Your Name" \
  --object-id $(az ad signed-in-user show --query id -o tsv)
```

### 3. Configure Firewall:

```bash
# Add your current IP (replace YOUR_IP)
az sql server firewall-rule create \
  --resource-group TaskManagerRG \
  --server taskmanager-sql-abanoub \
  --name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP

# Allow Azure services
az sql server firewall-rule create \
  --resource-group TaskManagerRG \
  --server taskmanager-sql-abanoub \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

## Troubleshooting

### "Login failed for user"

Problem: Not logged into Azure

Solution:
1. Run: az logout
2. Run: az login
3. Verify: az account show
4. Try again

### "Cannot connect to database"

Problem: Firewall blocking

Solution:
1. Azure Portal ? SQL Server
2. Firewalls and virtual networks
3. Add your IP address
4. Enable Azure services access
5. Save

### "User does not exist"

Problem: Database access not granted

Solution:
1. Connect to database as Azure AD admin
2. Run: CREATE USER [your-email] FROM EXTERNAL PROVIDER;
3. Run: ALTER ROLE db_owner ADD MEMBER [your-email];

## Advantages of Your Choice

You Chose the BEST Option!

Azure AD Passwordless is:
- More secure than passwords
- Easier to manage for teams
- Microsoft's recommendation
- Free (no extra cost)
- Future-proof (modern standard)

Well done!

## Next Steps

### RIGHT NOW:

1. Set Azure AD Admin:
   - Azure Portal ? SQL Server
   - Microsoft Entra admin ? Set admin
   - Select your account

2. Login to Azure:
   ```bash
   az login
   ```

3. Grant Database Access:
   - Connect to database
   - Run SQL to create user
   - Grant permissions

4. Test Locally:
   ```bash
   dotnet run --project TaskManager
   ```

### THEN:

5. Deploy to Azure:
   - Create App Service
   - Enable Managed Identity
   - Grant database access to identity
   - Publish your app

## Summary

What You Have:
- Azure SQL Database with Azure AD authentication
- Passwordless connection string
- Required packages installed
- Modern, secure setup

What You Need:
- Set Azure AD admin (5 min)
- Login to Azure (1 min)
- Grant database access (3 min)
- Test locally (5 min)

Total time: 15 minutes to fully working setup!

Next: Set Azure AD admin and login to Azure, then test your app!
