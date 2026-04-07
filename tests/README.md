# API Tests

This folder contains all the test scripts for the Density Reporting Tool Backend API.

## 🧪 **Available Tests**

### **1. Quick Test (Recommended)**
```bash
./tests/quick_test.sh
```
- ✅ Creates a job with random number (15000-25000)
- ✅ Tests job retrieval
- ✅ Shows formatted JSON output
- ✅ Fast and comprehensive

### **2. Full Test Script**
```bash
./tests/test_job_creation.sh
```
- ✅ Creates a job with random number
- ✅ Detailed output with HTTP status codes
- ✅ Cross-platform bash script

### **3. PowerShell Test (Windows/Mac)**
```powershell
./tests/test_job_creation.ps1
```
- ✅ Windows PowerShell compatible
- ✅ Random job number generation
- ✅ Error handling

## 🚀 **How to Run Tests**

### **From Project Root:**
```bash
# Quick test (recommended)
./tests/quick_test.sh

# Full test
./tests/test_job_creation.sh

# PowerShell (Windows/Mac)
./tests/test_job_creation.ps1
```

### **From Tests Folder:**
```bash
cd tests
./quick_test.sh
./test_job_creation.sh
```

## 📋 **What Tests Verify**

- ✅ **Random Job Number Generation** (15000-25000)
- ✅ **Job Creation** via POST API
- ✅ **Job Retrieval** via GET API
- ✅ **Database Persistence**
- ✅ **Job Notes Creation**
- ✅ **Proper HTTP Status Codes**
- ✅ **JSON Response Formatting**

## 🔧 **Prerequisites**

- Application running on `http://localhost:5013`
- `curl` command available
- `jq` command available (for JSON formatting)
- PowerShell (for .ps1 scripts)

## 📊 **Expected Results**

- **Job Creation**: HTTP 201 with job details
- **Job Retrieval**: HTTP 200 with complete job data
- **Random Numbers**: Unique job numbers between 15000-25000
- **Database**: Jobs persisted with notes and timestamps
