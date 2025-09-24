# DensityReportingTooltBackend

## Getting started Notes
appsettings.json: Base config for the app, has settings that apply to ALL environments
appsettings.development.json: Only loaded when environment is set to development, overrides values in appsettings.json
appsettings.development.json is ignored, use for Db connection strings, API keys, secrets

Properties/launchSettings.json: controls how the app is run during development, defines environment variables
                                for each profile, not used in production
Properties/PublishProfiles/*: Contains .pubxml files which determine where you're deploying your app and the configuration

DensityReportingToolBackend.http: lets us run HTTP requests difrectly from VisualStudio without needing Postman, curl, etc. OPTIONAL

WeatherFrecast.cs: Simple data model
Controllers/WeatherForecastController.cs: Controller template to return a list of WeatherForecasts



Adding Migrations and Migrating

Tools>NuGet Package Manager>Package Manager Console
run 'dotnet ef migrations add <MigrationName>'
run 'dotnet ef database update'


Running the Server:
run 'dotnet run'


## 🧪 **Testing**

### **Quick Test (Recommended):**
```bash
./tests/quick_test.sh
```

### **Full Test Suite:**
```bash
./tests/run_all_tests.sh
```

### **Individual Tests:**
```bash
# Quick test
./tests/quick_test.sh

# Full test
./tests/test_job_creation.sh

# PowerShell (Windows/Mac)
./tests/test_job_creation.ps1
```

### **Test Features:**
- ✅ Random job numbers (15000-25000)
- ✅ Job creation and retrieval
- ✅ Database persistence verification
- ✅ JSON response formatting

