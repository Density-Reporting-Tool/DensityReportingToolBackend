# Database Security Configuration

## 🔒 **Secure Database Connection Setup**

### **Current Setup:**
- ✅ `appsettings.json` - Uses environment variables (safe for git)
- ✅ `appsettings.Development.json` - Contains actual credentials (ignored by git)
- ✅ `.gitignore` - Prevents sensitive files from being committed

### **How It Works:**

1. **Production/Staging**: Uses environment variables
2. **Development**: Uses `appsettings.Development.json` (local only)

### **Environment Variables for Production:**

Set these environment variables on your server:

```bash
DB_HOST=ep-purple-art-aa9fvlr7-pooler.westus3.azure.neon.tech
DB_NAME=neondb
DB_USER=neondb_owner
DB_PASSWORD=npg_ygUF4x0pNEbX
```

### **Local Development:**

The `appsettings.Development.json` file contains your actual connection string and will be used automatically when running in Development mode.

### **Security Benefits:**

- ✅ **No credentials in git repository**
- ✅ **Environment-specific configuration**
- ✅ **Easy to rotate credentials**
- ✅ **Follows .NET best practices**

### **Alternative: User Secrets (Optional)**

For even more security, you can use .NET User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string-here"
```

This stores secrets in your user profile and never touches the file system.
