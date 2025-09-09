# PowerShell script to seed people data using SQL
$baseUrl = "http://localhost:5013"

Write-Host "Seeding people data using SQL..." -ForegroundColor Green

# Check if the server is running
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/test/health" -Method Get -TimeoutSec 5
    Write-Host "✓ Server is running" -ForegroundColor Green
} catch {
    Write-Host "✗ Server is not running. Please start the server first with: dotnet run" -ForegroundColor Red
    exit 1
}

# Execute the SQL script
Write-Host "`nExecuting SQL seed script..." -ForegroundColor Yellow

try {
    # Use psql to execute the SQL script
    $env:PGPASSWORD = "npg_ygUF4x0pNEbX"
    $sqlFile = "tests/seed_people_data.sql"
    
    # Execute the SQL script
    psql -h ep-purple-art-aa9fvlr7-pooler.westus3.azure.neon.tech -U neondb_owner -d neondb -f $sqlFile
    
    Write-Host "✓ SQL script executed successfully" -ForegroundColor Green
} catch {
    Write-Host "✗ Failed to execute SQL script: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "You may need to install PostgreSQL client tools or run the SQL manually" -ForegroundColor Yellow
}

# Verify the data was added
Write-Host "`nVerifying seeded data..." -ForegroundColor Yellow

try {
    $people = Invoke-RestMethod -Uri "$baseUrl/api/people" -Method Get
    Write-Host "✓ Found $($people.Count) people in the database" -ForegroundColor Green
    
    foreach ($person in $people) {
        $type = if ($person.personType -eq "GeoPacific Employee") { "Employee" } else { "Contractor" }
        $role = if ($person.role) { " ($($person.role))" } else { "" }
        Write-Host "  - $($person.firstName) $($person.lastName) ($type)$role" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ Failed to verify data: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nSeeding completed!" -ForegroundColor Green
Write-Host "`nTo view all people, visit: $baseUrl/api/people" -ForegroundColor Cyan