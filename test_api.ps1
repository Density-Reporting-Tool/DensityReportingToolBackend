# PowerShell script to test the API endpoints
Write-Host "Testing API endpoints..." -ForegroundColor Green

# Test 1: Check if the application is running
Write-Host "`n1. Testing basic connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/swagger" -Method Get -TimeoutSec 5
    Write-Host "✓ Swagger endpoint accessible" -ForegroundColor Green
} catch {
    Write-Host "✗ Swagger endpoint not accessible. Make sure the application is running." -ForegroundColor Red
    Write-Host "Run: dotnet run --urls=http://localhost:5000" -ForegroundColor Yellow
    exit 1
}

# Test 2: Test the TestController endpoints
Write-Host "`n2. Testing TestController endpoints..." -ForegroundColor Yellow

try {
    $employees = Invoke-RestMethod -Uri "http://localhost:5000/api/test/employees" -Method Get
    Write-Host "✓ GetEmployees endpoint working" -ForegroundColor Green
    Write-Host "  Found $($employees.Count) employees" -ForegroundColor Gray
} catch {
    Write-Host "✗ GetEmployees endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

try {
    $contractors = Invoke-RestMethod -Uri "http://localhost:5000/api/test/contractors" -Method Get
    Write-Host "✓ GetContractors endpoint working" -ForegroundColor Green
    Write-Host "  Found $($contractors.Count) contractors" -ForegroundColor Gray
} catch {
    Write-Host "✗ GetContractors endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

try {
    $people = Invoke-RestMethod -Uri "http://localhost:5000/api/test/people" -Method Get
    Write-Host "✓ GetAllPeople endpoint working" -ForegroundColor Green
    Write-Host "  Found $($people.Count) people total" -ForegroundColor Gray
} catch {
    Write-Host "✗ GetAllPeople endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test the PeopleController endpoints
Write-Host "`n3. Testing PeopleController endpoints..." -ForegroundColor Yellow

try {
    $allPeople = Invoke-RestMethod -Uri "http://localhost:5000/api/people" -Method Get
    Write-Host "✓ PeopleController GetAllPeople endpoint working" -ForegroundColor Green
    Write-Host "  Found $($allPeople.Count) people" -ForegroundColor Gray
} catch {
    Write-Host "✗ PeopleController GetAllPeople endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Test the JobsController endpoints
Write-Host "`n4. Testing JobsController endpoints..." -ForegroundColor Yellow

try {
    $jobs = Invoke-RestMethod -Uri "http://localhost:5000/api/jobs" -Method Get
    Write-Host "✓ GetJobs endpoint working" -ForegroundColor Green
    Write-Host "  Found $($jobs.Count) jobs" -ForegroundColor Gray
} catch {
    Write-Host "✗ GetJobs endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nAPI testing completed!" -ForegroundColor Green
Write-Host "`nTo view the full API documentation, visit: http://localhost:5000/swagger" -ForegroundColor Cyan
