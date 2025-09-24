# Proctor API Tests

This folder contains tests for the Proctor API functionality.

## Test Files

### `test_add_proctor.sh` (Linux/macOS)
Shell script that tests the complete proctor workflow:
- Creates a test job
- Adds a proctor to the job
- Retrieves the proctor by ID
- Lists proctors for the job
- Tests validation with invalid data

### `test_add_proctor.ps1` (Windows)
PowerShell script with the same functionality as the shell script.

## Prerequisites

1. **Backend server running** on `http://localhost:5013`
2. **Database updated** with latest migrations:
   ```bash
   dotnet ef database update
   ```
3. **ProctorService registered** in Program.cs:
   ```csharp
   builder.Services.AddScoped<IProctorService, ProctorService>();
   ```

## Running the Tests

### Linux/macOS:
```bash
cd /path/to/DensityReportingToolBackend/tests
./test_add_proctor.sh
```

### Windows:
```powershell
cd C:\path\to\DensityReportingToolBackend\tests
.\test_add_proctor.ps1
```

## Test Scenarios

### 1. Job Creation
- **Endpoint**: `POST /api/jobs`
- **Purpose**: Create a test job that proctor can reference
- **Expected**: HTTP 201 Created

### 2. Proctor Creation
- **Endpoint**: `POST /api/proctors/lab-admin`
- **Purpose**: Create a new proctor with all required fields
- **Expected**: HTTP 201 Created
- **Data**:
  ```json
  {
    "jobNumber": "00001",
    "proctorTestNumber": "P-001",
    "materialType": "Granular Base",
    "dateSampled": "2024-01-15",
    "proctorType": "MPDD",
    "maxDryDensity": "1900.0",
    "correctedDensity": "1915.0",
    "labLocation": "Main Lab",
    "proctorId": "P-001",
    "dateTested": "2024-01-16",
    "oversizePercentage": 5.2,
    "optimumMoisture": 12.3,
    "specificGravity": "2.65"
  }
  ```

### 3. Proctor Retrieval by ID
- **Endpoint**: `GET /api/proctors/{id}`
- **Purpose**: Retrieve the created proctor by its ID
- **Expected**: HTTP 200 OK with proctor data

### 4. Job Proctors List
- **Endpoint**: `GET /api/proctors/job/{jobNumber}`
- **Purpose**: Get all proctors associated with a job
- **Expected**: HTTP 200 OK with array of proctors

### 5. Validation Test
- **Endpoint**: `POST /api/proctors/lab-admin`
- **Purpose**: Test that invalid job numbers are rejected
- **Expected**: HTTP 400 Bad Request
- **Data**: Uses non-existent job number

## Expected Output

```
Testing Proctor Creation and Retrieval
======================================
ℹ️  Step 1: Creating test job...
✅ Job created successfully
Job Response: {"id":1,"jobNumber":"TEST-123456789",...}

ℹ️  Step 2: Adding proctor...
✅ Proctor created successfully
Proctor Response: {"id":"5","message":"Proctor created successfully",...}

ℹ️  Step 3: Retrieving proctor by ID...
✅ Proctor retrieved successfully
Retrieved Proctor: {"jobNumber":"TEST-123456789",...}

ℹ️  Step 4: Retrieving proctors for job...
✅ Job proctors retrieved successfully
Job Proctors: [{"jobNumber":"TEST-123456789",...}]

ℹ️  Step 5: Testing validation (invalid job number)...
✅ Validation working correctly (rejected invalid job)
Error Response: {"message":"Job with number 'NONEXISTENT-999' does not exist..."}

ℹ️  Test Summary:
- Job Creation: ✅ PASS
- Proctor Creation: ✅ PASS
- Proctor Retrieval: ✅ PASS
- Job Proctors List: ✅ PASS
- Validation Test: ✅ PASS

ℹ️  Test completed!
```

## Troubleshooting

### Common Issues:

1. **Connection refused**: Backend server not running
2. **404 Not Found**: Endpoint not registered or incorrect URL
3. **500 Internal Server Error**: Database not migrated or service not registered
4. **400 Bad Request**: Invalid data format or missing required fields

### Debug Steps:

1. Check backend server is running: `curl http://localhost:5013/api/health`
2. Check database migrations: `dotnet ef database update`
3. Check service registration in Program.cs
4. Check controller routes are correct
5. Verify ProctorTypes exist in database (SPDD, MPDD)

## Notes

- Each test run creates a unique job number using timestamp
- Tests are designed to be repeatable without cleanup
- All test data uses realistic proctor test values
- Validation tests ensure proper error handling
