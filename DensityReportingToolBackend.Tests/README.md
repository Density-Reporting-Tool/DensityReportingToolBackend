# Job Creation Tests

This test project contains comprehensive tests for the job creation functionality in the DensityReportingToolBackend.

## Test Types

### 1. Unit Tests (`JobCreationUnitTests.cs`)
- **Purpose**: Test the controller logic in isolation
- **Database**: Uses in-memory database for fast execution
- **Dependencies**: Mocks the logger using Moq
- **Coverage**: Tests all validation scenarios, success cases, and edge cases

### 2. Integration Tests (`JobsControllerTests.cs`)
- **Purpose**: Test the full HTTP request/response cycle
- **Database**: Uses in-memory database
- **Dependencies**: Full web application setup
- **Coverage**: Tests the complete API endpoint behavior

## Test Scenarios Covered

### ✅ Success Cases
- Create job with valid data
- Create job with alphanumeric job number (e.g., "2024-A")
- Create job with numeric job number (e.g., "25482")
- Create job with optional dates
- Create job with job notes
- Create job without job notes

### ✅ Validation Cases
- Missing job number
- Missing client name
- Missing project name
- Missing site address
- Duplicate job number

### ✅ Edge Cases
- Null dates
- Empty strings
- Long job notes

## Running the Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "JobCreationUnitTests"
dotnet test --filter "JobsControllerTests"
```

### Run with Verbose Output
```bash
dotnet test --verbosity normal
```

### Run with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Data Examples

### Valid Job Request
```json
{
  "jobNumber": "TEST-001",
  "clientName": "Test Client",
  "projectName": "Test Project",
  "siteAddress": "123 Test Street, Test City, TC 12345",
  "startDate": "2024-01-15T00:00:00Z",
  "endDate": "2024-12-31T00:00:00Z",
  "jobNotes": "This is a test job note"
}
```

### Expected Success Response
```json
{
  "id": 1,
  "jobNumber": "TEST-001",
  "clientName": "Test Client",
  "project": {
    "projectName": "Test Project",
    "siteAddress": "123 Test Street, Test City, TC 12345",
    "startDate": "2024-01-15T00:00:00Z",
    "endDate": "2024-12-31T00:00:00Z"
  },
  "message": "Job created successfully"
}
```

## Test Structure

Each test follows the **Arrange-Act-Assert** pattern:
1. **Arrange**: Set up test data and conditions
2. **Act**: Execute the method being tested
3. **Assert**: Verify the expected outcome

## Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking framework for dependencies
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for testing
