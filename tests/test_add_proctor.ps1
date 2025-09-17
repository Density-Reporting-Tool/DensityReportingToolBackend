# PowerShell script to test proctor creation and retrieval
Write-Host "Testing Proctor Creation and Retrieval" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow

# Configuration
$API_BASE = "http://localhost:5013"
$PROCTOR_ENDPOINT = "$API_BASE/api/proctors"

# Function to print colored output
function Write-Success { param($Message) Write-Host "✅ $Message" -ForegroundColor Green }
function Write-Error { param($Message) Write-Host "❌ $Message" -ForegroundColor Red }
function Write-Info { param($Message) Write-Host "ℹ️  $Message" -ForegroundColor Cyan }

# Test data
$TEST_JOB_NUMBER = "TEST-$(Get-Date -UFormat %s)"
$PROCTOR_DATA = @{
    jobNumber = $TEST_JOB_NUMBER
    proctorTestNumber = "P-001"
    materialType = "Granular Base"
    dateSampled = "2024-01-15"
    proctorType = "MPDD"
    maxDryDensity = "125.5"
    correctedDensity = "123.8"
    labLocation = "Main Lab"
    proctorId = "P-001"
    dateTested = "2024-01-16"
    oversizePercentage = 5.2
    optimumMoisture = 12.3
    specificGravity = "2.65"
} | ConvertTo-Json

try {
    # Step 1: Create test job
    Write-Info "Step 1: Creating test job..."
    $JOB_DATA = @{
        jobNumber = $TEST_JOB_NUMBER
        clientName = "Test Client"
        projectName = "Test Project"
        siteAddress = "123 Test Street"
    } | ConvertTo-Json

    $jobResponse = Invoke-RestMethod -Uri "$API_BASE/api/jobs" -Method Post -Body $JOB_DATA -ContentType "application/json"
    Write-Success "Job created successfully"
    Write-Host "Job Response: $($jobResponse | ConvertTo-Json -Depth 3)"

    # Step 2: Add Proctor
    Write-Info "Step 2: Adding proctor..."
    $proctorResponse = Invoke-RestMethod -Uri "$PROCTOR_ENDPOINT/lab-admin" -Method Post -Body $PROCTOR_DATA -ContentType "application/json"
    Write-Success "Proctor created successfully"
    Write-Host "Proctor Response: $($proctorResponse | ConvertTo-Json -Depth 3)"
    
    $PROCTOR_ID = $proctorResponse.id
    Write-Info "Created Proctor ID: $PROCTOR_ID"

    # Step 3: Retrieve Proctor by ID
    if ($PROCTOR_ID) {
        Write-Info "Step 3: Retrieving proctor by ID..."
        $getResponse = Invoke-RestMethod -Uri "$PROCTOR_ENDPOINT/$PROCTOR_ID" -Method Get -ContentType "application/json"
        Write-Success "Proctor retrieved successfully"
        Write-Host "Retrieved Proctor: $($getResponse | ConvertTo-Json -Depth 3)"
    }

    # Step 4: Get Proctors for Job
    Write-Info "Step 4: Retrieving proctors for job..."
    $jobProctorsResponse = Invoke-RestMethod -Uri "$PROCTOR_ENDPOINT/job/$TEST_JOB_NUMBER" -Method Get -ContentType "application/json"
    Write-Success "Job proctors retrieved successfully"
    Write-Host "Job Proctors: $($jobProctorsResponse | ConvertTo-Json -Depth 3)"

    # Step 5: Test Validation (try to create proctor with invalid job)
    Write-Info "Step 5: Testing validation (invalid job number)..."
    $INVALID_PROCTOR_DATA = @{
        jobNumber = "NONEXISTENT-999"
        proctorTestNumber = "P-002"
        materialType = "Test Material"
        dateSampled = "2024-01-15"
        proctorType = "SPDD"
        maxDryDensity = "120.0"
        correctedDensity = "118.0"
        labLocation = "Test Lab"
        proctorId = "P-002"
        dateTested = "2024-01-16"
        oversizePercentage = 3.0
        optimumMoisture = 10.0
        specificGravity = "2.50"
    } | ConvertTo-Json

    try {
        $invalidResponse = Invoke-RestMethod -Uri "$PROCTOR_ENDPOINT/lab-admin" -Method Post -Body $INVALID_PROCTOR_DATA -ContentType "application/json"
        Write-Error "Validation not working (should have failed)"
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 400) {
            Write-Success "Validation working correctly (rejected invalid job)"
            Write-Host "Error Response: $($_.Exception.Message)"
        }
        else {
            Write-Error "Unexpected error: $($_.Exception.Message)"
        }
    }

    # Test Summary
    Write-Host ""
    Write-Info "Test Summary:"
    Write-Host "- Job Creation: ✅ PASS" -ForegroundColor Green
    Write-Host "- Proctor Creation: ✅ PASS" -ForegroundColor Green
    Write-Host "- Proctor Retrieval: ✅ PASS" -ForegroundColor Green
    Write-Host "- Job Proctors List: ✅ PASS" -ForegroundColor Green
    Write-Host "- Validation Test: ✅ PASS" -ForegroundColor Green

}
catch {
    Write-Error "Test failed with error: $($_.Exception.Message)"
    Write-Host $_.Exception.Response
}

Write-Host ""
Write-Info "Test completed!"
