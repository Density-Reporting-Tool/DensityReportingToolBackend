# Test script to create a job via the API
$baseUrl = "https://localhost:7000"  # Adjust port if needed
$endpoint = "$baseUrl/api/jobs"

# Test job data
$jobData = @{
    JobNumber = "2024-TEST-001"
    ClientName = "Test Construction Company"
    ProjectName = "Test Project - API Validation"
    SiteAddress = "123 Test Street, Test City, TC 12345"
    StartDate = "2024-01-15T00:00:00Z"
    EndDate = "2024-06-15T00:00:00Z"
    JobNotes = "This is a test job created via API to validate the job creation functionality."
} | ConvertTo-Json

Write-Host "Testing Job Creation API..."
Write-Host "Endpoint: $endpoint"
Write-Host "Data: $jobData"
Write-Host ""

try {
    # Make the POST request
    $response = Invoke-RestMethod -Uri $endpoint -Method POST -Body $jobData -ContentType "application/json"
    
    Write-Host "✅ SUCCESS! Job created successfully:" -ForegroundColor Green
    Write-Host "Job ID: $($response.id)"
    Write-Host "Job Number: $($response.jobNumber)"
    Write-Host "Client Name: $($response.clientName)"
    Write-Host "Project Name: $($response.projectName)"
    Write-Host "Site Address: $($response.siteAddress)"
    
} catch {
    Write-Host "❌ ERROR: Failed to create job" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
    Write-Host "Error Message: $($_.Exception.Message)"
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody"
    }
}
