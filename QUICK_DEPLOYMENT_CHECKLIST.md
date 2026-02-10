# ?? Quick Deployment Checklist

## ? **Fast Track to Production**

### **Before You Start:**
- [ ] Application tested locally ?
- [ ] All features working ?
- [ ] Build is successful ?

---

## **EASIEST METHOD: Azure App Service (15 minutes)**

### **What You Need:**
- Azure account (get free tier at https://azure.microsoft.com/free)
- Visual Studio 2022

### **Steps:**

#### **1. Set Up Azure SQL Database (5 min)**
```bash
# Login to Azure portal: https://portal.azure.com
# Create Resource Group: "TaskManagerRG"
# Create SQL Database: "TaskManagerDb"
# Copy connection string
```

#### **2. Update Production Config (2 min)**
Edit `TaskManager\appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PASTE_YOUR_AZURE_SQL_CONNECTION_STRING_HERE"
  }
}
```

#### **3. Publish from Visual Studio (8 min)**
1. **Right-click** `TaskManager` project
2. Click **Publish**
3. Choose **Azure** ? **Azure App Service (Windows)**
4. Click **Create New**
5. Fill in:
   - Name: `taskmanager-YOURNAME`
   - Resource Group: `TaskManagerRG`
6. Click **Create**
7. Wait for deployment
8. **Done!** Your app opens automatically ??

---

## **ALTERNATIVE: Deploy to IIS (if you have Windows Server)**

### **Prerequisites:**
- Windows Server with IIS
- SQL Server installed

### **Quick Steps:**

#### **1. Publish to Folder**
```powershell
dotnet publish -c Release -o C:\Deploy\TaskManager
```

#### **2. Create IIS Site**
```powershell
# In IIS Manager:
# - Create new Application Pool (no managed code)
# - Create new Website pointing to C:\Deploy\TaskManager
# - Start website
```

#### **3. Configure Database**
```sql
CREATE DATABASE TaskManagerDb;
GO
```

Update `appsettings.Production.json` with local SQL Server connection.

#### **4. Browse**
Navigate to `http://localhost` or your server IP

---

## **Post-Deployment:**

### **Test These:**
- [ ] Can register new user
- [ ] Can login
- [ ] Can create task
- [ ] Can see dashboard
- [ ] Charts display correctly

---

## **Common Issues:**

### **"500 Error"**
? Check logs in `logs/` folder  
? Verify database connection string

### **"Database error"**
? Run migrations: `dotnet ef database update`

### **"Charts not showing"**
? Clear browser cache  
? Check browser console (F12)

---

## **?? Recommended Path:**

**For Learning/Demo:**
? Azure App Service (easiest, free tier available)

**For Production:**
? Azure App Service with paid tier

**For Self-Hosted:**
? IIS on Windows Server or Docker

---

## **Need Help?**

See full guide: `PRODUCTION_DEPLOYMENT_GUIDE.md`

---

**Time to deploy:** 15-30 minutes  
**Difficulty:** ????? Easy

**Let's deploy! ??**
