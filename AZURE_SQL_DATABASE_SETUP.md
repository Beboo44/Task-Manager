# ?? Azure SQL Database Setup - Step-by-Step Guide

## ?? **What to Enter When Creating Azure SQL Database**

When Azure asks for these fields, here's what to enter:

---

## ?? **Fields Explained:**

### **1. Database Name**
```
What to enter: TaskManagerDb

Why: This is the name of your database
Example: TaskManagerDb
```

### **2. Server**
```
What to do: Click "Create new"

Then you'll need to create a SQL Server with these details:
```

---

## ??? **Creating SQL Server (Required First)**

When you click "Create new" for Server, you'll see these fields:

### **Server Name:**
```
What to enter: taskmanager-sql-server-YOURNAME

Rules:
  - Must be globally unique across all of Azure
  - Use lowercase letters, numbers, and hyphens only
  - Example: taskmanager-sql-server-john
  - Example: taskmanager-sql-2026
  - Example: tm-sql-yourcompany

? Good: taskmanager-sql-john2026
? Bad: TaskManager_SQL (uppercase/underscores not allowed)
```

### **Location:**
```
What to choose: Pick closest to you

Examples:
  - East US
  - West Europe
  - Southeast Asia
  - UK South

Recommendation: Choose the one nearest to you or your users
```

### **Authentication Method:**
```
Choose: Use SQL authentication

Then enter:

Server admin login:
  - Enter: sqladmin
  - Or: taskmanageradmin
  - Or: dbadmin

Password:
  - Must be strong!
  - Example: TaskManager2026!
  - Example: MySecurePass123!
  
  Requirements:
    ? At least 8 characters
    ? Contains uppercase letters
    ? Contains lowercase letters
    ? Contains numbers
    ? Contains special characters (!, @, #, $, etc.)

?? IMPORTANT: Write down your password! You'll need it!
```

---

## ?? **Complete Example:**

Here's what a filled-out form looks like:

```
???????????????????????????????????????????????????
? Create SQL Database                             ?
???????????????????????????????????????????????????
?                                                 ?
? Resource Group: TaskManagerRG                   ?
?                                                 ?
? Database name: TaskManagerDb                    ?
?                                                 ?
? Server: [Create new] ??? Click this            ?
?   ?????????????????????????????????????????   ?
?   ? Server name: taskmanager-sql-john     ?   ?
?   ? Location: East US                     ?   ?
?   ? Authentication: SQL authentication     ?   ?
?   ? Admin login: sqladmin                 ?   ?
?   ? Password: TaskManager2026!            ?   ?
?   ? Confirm password: TaskManager2026!    ?   ?
?   ?????????????????????????????????????????   ?
?                                                 ?
? Compute + storage: [Configure database]        ?
?   ? Choose: Basic (cheapest for learning)      ?
?   ? Or: Free tier if available                 ?
?                                                 ?
? Backup storage redundancy: Locally-redundant   ?
?                                                 ?
???????????????????????????????????????????????????
```

---

## ?? **Cost Settings:**

### **Recommended for Learning:**
```
Pricing Tier: Basic
Cost: ~$5/month
Storage: 2 GB
Good for: Development, learning, small apps
```

### **Recommended for Production:**
```
Pricing Tier: Standard S0
Cost: ~$15/month
Storage: 250 GB
Good for: Small to medium production apps
```

### **Free Tier (if available):**
```
Some regions offer a free SQL Database
Look for: "Free" or "Basic 5 DTU"
Duration: 12 months free (with new Azure account)
```

---

## ?? **After Creating:**

Once created, Azure will show you:

### **Connection String:**
```
Format:
Server=tcp:taskmanager-sql-john.database.windows.net,1433;
Initial Catalog=TaskManagerDb;
Persist Security Info=False;
User ID=sqladmin;
Password=TaskManager2026!;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

### **What to do with it:**
1. Copy the entire connection string
2. Open `TaskManager\appsettings.Production.json`
3. Replace `YOUR_PRODUCTION_CONNECTION_STRING_HERE` with it

---

## ?? **Quick Reference:**

| Field | What to Enter | Example |
|-------|--------------|---------|
| **Database name** | TaskManagerDb | TaskManagerDb |
| **Server name** | taskmanager-sql-YOURNAME | taskmanager-sql-john |
| **Location** | Nearest region | East US |
| **Admin login** | sqladmin | sqladmin |
| **Password** | Strong password! | TaskManager2026! |
| **Pricing tier** | Basic (for learning) | Basic |

---

## ?? **Step-by-Step Visual Guide:**

### **Step 1: Login to Azure Portal**
```
URL: https://portal.azure.com
```

### **Step 2: Create Resource**
```
1. Click "Create a resource"
2. Search: "SQL Database"
3. Click "Create"
```

### **Step 3: Fill Basics Tab**
```
Subscription: Your subscription
Resource Group: TaskManagerRG (or click Create new)
Database name: TaskManagerDb
Server: Click "Create new" ?
```

### **Step 4: Create SQL Server**
```
Server name: taskmanager-sql-yourname
Location: East US (or nearest)
Authentication: SQL authentication
Admin login: sqladmin
Password: YourStrongPassword123!
Confirm: YourStrongPassword123!

Click: OK
```

### **Step 5: Configure Database**
```
Compute + storage: Click "Configure database"
  ? Select: Basic (2 GB, ~$5/month)
  ? Or: General Purpose if you want better performance
Click: Apply
```

### **Step 6: Networking Tab**
```
Connectivity method: Public endpoint

Firewall rules:
  ? Allow Azure services to access server
  ? Add current client IP address

Click: Next
```

### **Step 7: Review + Create**
```
1. Review your settings
2. Click "Create"
3. Wait 2-5 minutes for deployment
```

---

## ?? **Getting Your Connection String:**

### **After Database is Created:**

**Method 1: From Azure Portal**
```
1. Go to your SQL Database (TaskManagerDb)
2. Click "Connection strings" in left menu
3. Copy the "ADO.NET" connection string
4. Replace {your_password} with your actual password
```

**Method 2: Manual Format**
```
Server=tcp:YOUR-SERVER-NAME.database.windows.net,1433;
Initial Catalog=TaskManagerDb;
User ID=sqladmin;
Password=YOUR_PASSWORD_HERE;
TrustServerCertificate=true;
MultipleActiveResultSets=true;
```

Replace:
- `YOUR-SERVER-NAME` with your server name (e.g., taskmanager-sql-john)
- `YOUR_PASSWORD_HERE` with your password (e.g., TaskManager2026!)

---

## ?? **Important Notes:**

### **Server Name Must Be Unique:**
```
? taskmanager-sql (probably taken)
? test-server (definitely taken)

? taskmanager-sql-john2026 (unique to you)
? tm-sql-mycompany (unique to you)
? taskmanager-sql-12345 (unique to you)

Tip: Add your name, company, or random numbers
```

### **Save These Credentials:**
```
?? Write down:
  1. Server name: taskmanager-sql-john
  2. Admin login: sqladmin
  3. Password: TaskManager2026!

You'll need them for:
  - Connection string
  - Managing database
  - Future access
```

### **Firewall Settings:**
```
? Must allow your IP address
? Must allow Azure services

Otherwise:
  ? Your app can't connect
  ? You can't access from your PC
```

---

## ?? **Common Errors:**

### **"Server name already exists"**
```
Problem: Someone else is using that name
Solution: Add your name/numbers to make it unique
  ? Try: taskmanager-sql-john2026
  ? Try: taskmanager-sql-yourname
```

### **"Weak password"**
```
Problem: Password doesn't meet requirements
Solution: Use strong password
  ? TaskManager2026!
  ? SecurePass123!
  ? MyApp@2026
```

### **"Can't connect to database"**
```
Problem: Firewall blocking connection
Solution:
  1. Go to SQL Server (not database)
  2. Click "Firewalls and virtual networks"
  3. Add your IP address
  4. Enable "Allow Azure services"
```

---

## ?? **Your Action Items:**

```
1. [ ] Login to Azure Portal
2. [ ] Create SQL Server with name: taskmanager-sql-YOURNAME
3. [ ] Set admin login: sqladmin
4. [ ] Set strong password (write it down!)
5. [ ] Create database: TaskManagerDb
6. [ ] Choose pricing: Basic (~$5/month)
7. [ ] Configure firewall rules
8. [ ] Copy connection string
9. [ ] Update appsettings.Production.json
10. [ ] Deploy your app!
```

---

## ?? **Summary:**

**Database Name:** `TaskManagerDb`  
**Server Name:** `taskmanager-sql-YOURNAME` (must be unique!)  
**Admin Login:** `sqladmin`  
**Password:** Your strong password (write it down!)  
**Location:** Nearest to you  
**Pricing:** Basic ($5/month) or Free tier

**Time to create:** 3-5 minutes  
**Then:** Copy connection string and deploy!

---

## ?? **Next Steps:**

After creating the database:

1. ? Copy connection string
2. ? Update `appsettings.Production.json`
3. ? Continue with Visual Studio Publish
4. ? Deploy your app!

**You're almost there!** ??
