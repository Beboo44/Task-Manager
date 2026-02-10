# ?? Task Manager - Production Deployment Guide

## ? **Pre-Deployment Checklist**

Before deploying to production, ensure you've completed these steps:

- [ ] Code is tested and working locally
- [ ] All builds are successful
- [ ] Database migrations are up to date
- [ ] Production configuration is set up
- [ ] Security settings are configured
- [ ] Backup strategy is planned

---

## ?? **Deployment Options**

You have several options for deploying your ASP.NET Core application:

### **Option 1: Azure App Service (Recommended for Beginners)** ?
- Easy to set up
- Automatic scaling
- Built-in SSL certificates
- Managed database hosting

### **Option 2: IIS on Windows Server**
- Full control over server
- Good for enterprise environments
- Requires Windows Server license

### **Option 3: Docker Container**
- Platform-independent
- Easy to scale
- Good for cloud deployments

### **Option 4: Linux Server (Nginx + Kestrel)**
- Cost-effective
- High performance
- Requires Linux knowledge

---

## ?? **Step-by-Step Deployment**

## **OPTION 1: Azure App Service (Cloud Deployment)**

### **Prerequisites:**
- Azure account (free tier available)
- Visual Studio 2022 or Azure CLI

### **Step 1: Prepare Production Database**

#### **Option A: Azure SQL Database**

1. **Create Azure SQL Server:**
```bash
# Login to Azure
az login

# Create resource group
az group create --name TaskManagerRG --location eastus

# Create SQL Server
az sql server create \
  --name taskmanager-sql-server \
  --resource-group TaskManagerRG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourSecurePassword123!

# Create database
az sql db create \
  --resource-group TaskManagerRG \
  --server taskmanager-sql-server \
  --name TaskManagerDb \
  --service-objective S0
```

2. **Get Connection String:**
```
Server=tcp:taskmanager-sql-server.database.windows.net,1433;
Initial Catalog=TaskManagerDb;
Persist Security Info=False;
User ID=sqladmin;
Password=YourSecurePassword123!;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

#### **Option B: Your Own SQL Server**
- Use your existing SQL Server
- Update connection string in `appsettings.Production.json`

---

### **Step 2: Update Production Configuration**

**Edit `TaskManager\appsettings.Production.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_PRODUCTION_CONNECTION_STRING_HERE"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

?? **IMPORTANT:** Never commit this file to Git!

---

### **Step 3: Publish to Azure**

#### **Method A: Visual Studio (Easiest)**

1. **Right-click** on `TaskManager` project ? **Publish**

2. **Select Target:**
   - Choose **Azure**
   - Click **Next**

3. **Select Specific Target:**
   - Choose **Azure App Service (Windows)** or **Azure App Service (Linux)**
   - Click **Next**

4. **Create New App Service:**
   ```
   Name: taskmanager-webapp
   Subscription: Your Azure Subscription
   Resource Group: TaskManagerRG (or create new)
   Hosting Plan: Create new or select existing
   ```

5. **Configure Settings:**
   - Set **ASPNETCORE_ENVIRONMENT** to **Production**
   - Add connection string in **Configuration ? Connection Strings**

6. **Publish:**
   - Click **Publish** button
   - Wait for deployment (2-5 minutes)
   - Your app will open in browser automatically!

#### **Method B: Azure CLI**

```bash
# Create App Service Plan
az appservice plan create \
  --name TaskManagerPlan \
  --resource-group TaskManagerRG \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --name taskmanager-webapp \
  --resource-group TaskManagerRG \
  --plan TaskManagerPlan \
  --runtime "DOTNETCORE:8.0"

# Configure connection string
az webapp config connection-string set \
  --name taskmanager-webapp \
  --resource-group TaskManagerRG \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="YOUR_CONNECTION_STRING"

# Publish from command line
dotnet publish -c Release
cd TaskManager/bin/Release/net8.0/publish
zip -r publish.zip .
az webapp deployment source config-zip \
  --name taskmanager-webapp \
  --resource-group TaskManagerRG \
  --src publish.zip
```

---

## **OPTION 2: IIS on Windows Server**

### **Prerequisites:**
- Windows Server 2016 or later
- IIS installed with ASP.NET Core Hosting Bundle

### **Step 1: Install Prerequisites on Server**

1. **Install .NET 8 Hosting Bundle:**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Choose **Hosting Bundle** installer
   - Run installer and restart IIS

2. **Verify Installation:**
```powershell
dotnet --info
```

### **Step 2: Prepare SQL Server Database**

1. **Create Database:**
```sql
CREATE DATABASE TaskManagerDb;
GO
```

2. **Create SQL Login:**
```sql
CREATE LOGIN TaskManagerUser WITH PASSWORD = 'YourSecurePassword123!';
GO

USE TaskManagerDb;
GO

CREATE USER TaskManagerUser FOR LOGIN TaskManagerUser;
GO

ALTER ROLE db_owner ADD MEMBER TaskManagerUser;
GO
```

3. **Update Connection String** in `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TaskManagerDb;User Id=TaskManagerUser;Password=YourSecurePassword123!;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### **Step 3: Publish Application**

**In Visual Studio:**

1. **Right-click** project ? **Publish**
2. **Select Target:** Folder
3. **Choose Folder:** `C:\inetpub\wwwroot\TaskManager`
4. **Click Publish**

**Or via Command Line:**
```powershell
dotnet publish -c Release -o C:\inetpub\wwwroot\TaskManager
```

### **Step 4: Configure IIS**

1. **Open IIS Manager**

2. **Create Application Pool:**
   - Name: `TaskManagerPool`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`

3. **Create Website:**
   - Site name: `TaskManager`
   - Application pool: `TaskManagerPool`
   - Physical path: `C:\inetpub\wwwroot\TaskManager`
   - Binding: Port 80 (or 443 for HTTPS)

4. **Set Permissions:**
```powershell
icacls "C:\inetpub\wwwroot\TaskManager" /grant "IIS AppPool\TaskManagerPool:(OI)(CI)F" /T
```

5. **Restart IIS:**
```powershell
iisreset
```

6. **Browse to:** `http://your-server-ip` or `http://localhost`

---

## **OPTION 3: Docker Container**

### **Step 1: Create Dockerfile**

Create `Dockerfile` in solution root:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["TaskManager/TaskManager.csproj", "TaskManager/"]
COPY ["TaskManager.Business/TaskManager.Business.csproj", "TaskManager.Business/"]
COPY ["TaskManager.DataAccess/TaskManager.DataAccess.csproj", "TaskManager.DataAccess/"]

# Restore dependencies
RUN dotnet restore "TaskManager/TaskManager.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/TaskManager"
RUN dotnet build "TaskManager.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TaskManager.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "TaskManager.dll"]
```

### **Step 2: Create docker-compose.yml**

```yaml
version: '3.8'

services:
  web:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"
      - "443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=TaskManagerDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
    depends_on:
      - db
    restart: unless-stopped

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    restart: unless-stopped

volumes:
  sqldata:
```

### **Step 3: Deploy**

```bash
# Build and run
docker-compose up -d

# View logs
docker-compose logs -f

# Stop containers
docker-compose down
```

---

## **OPTION 4: Linux Server (Ubuntu + Nginx)**

### **Step 1: Prepare Server**

```bash
# Update system
sudo apt update
sudo apt upgrade -y

# Install .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0

# Install Nginx
sudo apt install -y nginx

# Install SQL Server (optional)
# Or use Azure SQL / external SQL Server
```

### **Step 2: Publish Application**

**On your local machine:**
```bash
dotnet publish -c Release -o ./publish
```

**Copy to server:**
```bash
scp -r ./publish/* user@your-server:/var/www/taskmanager/
```

### **Step 3: Configure Systemd Service**

Create `/etc/systemd/system/taskmanager.service`:

```ini
[Unit]
Description=Task Manager ASP.NET Core App
After=network.target

[Service]
WorkingDirectory=/var/www/taskmanager
ExecStart=/usr/bin/dotnet /var/www/taskmanager/TaskManager.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=taskmanager
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

**Enable and start:**
```bash
sudo systemctl enable taskmanager
sudo systemctl start taskmanager
sudo systemctl status taskmanager
```

### **Step 4: Configure Nginx**

Create `/etc/nginx/sites-available/taskmanager`:

```nginx
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

**Enable site:**
```bash
sudo ln -s /etc/nginx/sites-available/taskmanager /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

---

## ?? **Security Configuration**

### **1. HTTPS/SSL Certificate**

#### **Azure:**
- Free SSL certificate included
- Configure in App Service ? Custom domains ? Add binding

#### **IIS:**
```powershell
# Install SSL certificate in IIS
# Bind HTTPS (port 443) to your site
```

#### **Nginx (Let's Encrypt):**
```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

### **2. Application Security**

**Update `Program.cs`:**
```csharp
// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
```

### **3. Database Security**

- ? Use strong passwords
- ? Enable firewall rules
- ? Use separate DB user (not sa/admin)
- ? Enable SQL Server encryption
- ? Regular backups

---

## ?? **Post-Deployment Tasks**

### **1. Run Database Migrations**

**Azure/Cloud:**
```bash
# Migrations run automatically on first start (see Program.cs)
```

**IIS/Manual:**
```powershell
cd C:\inetpub\wwwroot\TaskManager
dotnet ef database update
```

### **2. Create First Admin User**

Browse to: `https://your-app-url/Account/Register`

### **3. Test Application**

- [ ] Register new user
- [ ] Login
- [ ] Create category
- [ ] Create task
- [ ] Mark task complete
- [ ] Check dashboard
- [ ] Test all features

### **4. Monitor Logs**

**Azure:**
- App Service ? Monitoring ? Log stream

**IIS:**
- `C:\inetpub\wwwroot\TaskManager\logs\`

**Linux:**
```bash
sudo journalctl -u taskmanager -f
```

---

## ?? **Updating Production**

### **Azure:**
```bash
# Simply publish again from Visual Studio
# Or use Azure DevOps / GitHub Actions
```

### **IIS:**
```powershell
# Stop application pool
Stop-WebAppPool -Name TaskManagerPool

# Publish new version
dotnet publish -c Release -o C:\inetpub\wwwroot\TaskManager

# Start application pool
Start-WebAppPool -Name TaskManagerPool
```

### **Docker:**
```bash
docker-compose down
docker-compose build
docker-compose up -d
```

---

## ?? **Environment Variables**

Set these in production:

| Variable | Value | Where to Set |
|----------|-------|--------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | All platforms |
| `ConnectionStrings__DefaultConnection` | Your DB connection | App config |
| `ASPNETCORE_URLS` | `http://+:80` | Linux/Docker |

---

## ?? **Backup Strategy**

### **Database Backups:**

**Azure SQL:**
```bash
# Automatic backups enabled
# Retention: 7-35 days
```

**SQL Server:**
```sql
-- Daily full backup
BACKUP DATABASE TaskManagerDb
TO DISK = 'C:\Backups\TaskManagerDb_Full.bak'
WITH FORMAT;

-- Schedule via SQL Server Agent
```

### **Application Backups:**
- Keep deployment packages
- Use version control (Git)
- Store in cloud storage

---

## ?? **Troubleshooting**

### **Issue: 500 Internal Server Error**

**Check:**
1. Application logs (`logs/` folder)
2. Event Viewer (Windows)
3. `journalctl` (Linux)
4. Database connection string
5. File permissions

### **Issue: Database Connection Failed**

**Fix:**
```
1. Verify connection string
2. Check firewall rules
3. Test SQL Server connectivity
4. Verify user permissions
```

### **Issue: HTTPS Redirect Loop**

**Fix:**
```csharp
// In Program.cs, check if behind proxy:
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
```

---

## ?? **Performance Optimization**

### **1. Enable Response Compression**

```csharp
// In Program.cs
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

### **2. Add Response Caching**

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

### **3. Database Optimization**

```sql
-- Add indexes to frequently queried columns
CREATE INDEX IX_UserTask_UserId ON UserTasks(UserId);
CREATE INDEX IX_UserTask_Deadline ON UserTasks(Deadline);
CREATE INDEX IX_UserTask_Status ON UserTasks(Status);
```

---

## ?? **Quick Start Commands**

### **Publish Locally:**
```bash
dotnet publish -c Release
```

### **Test Production Build:**
```bash
cd TaskManager/bin/Release/net8.0/publish
dotnet TaskManager.dll --environment=Production
```

### **Database Update:**
```bash
dotnet ef database update --project TaskManager.DataAccess --startup-project TaskManager
```

---

## ? **Deployment Verification Checklist**

After deployment, verify:

- [ ] Application loads without errors
- [ ] User registration works
- [ ] Login/logout works
- [ ] Database connection successful
- [ ] All CRUD operations work
- [ ] Charts display correctly
- [ ] Performance curve shows
- [ ] Email notifications work (if configured)
- [ ] HTTPS is enforced
- [ ] Logs are being written
- [ ] Backups are scheduled

---

## ?? **Support & Resources**

### **Official Documentation:**
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Azure: https://docs.microsoft.com/azure
- Entity Framework: https://docs.microsoft.com/ef

### **Community:**
- Stack Overflow: tag `asp.net-core`
- GitHub Issues
- Microsoft Q&A

---

## ?? **Production Checklist Summary**

```
? appsettings.Production.json created
? .gitignore configured
? web.config created (for IIS)
? Database connection string updated
? Security headers added
? HTTPS enabled
? Error handling configured
? Logging configured
? Migrations ready
? Backup strategy planned
```

---

## ?? **You're Ready to Deploy!**

Choose your deployment option above and follow the steps.

**Recommended for beginners:** Start with **Azure App Service** (Option 1)

**Good luck with your deployment!** ??

---

**Document Version:** 1.0  
**Last Updated:** 2024  
**Application:** Task Manager ASP.NET Core 8.0
