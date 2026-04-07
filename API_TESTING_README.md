# API Testing Guide

## Overview
This guide explains how to test the refactored API endpoints for the Density Reporting Tool Backend.

## Prerequisites
- .NET 8.0 SDK installed
- PostgreSQL database running and accessible
- Database migration completed successfully

## Running the Application

1. **Start the API server:**
   ```bash
   dotnet run --urls=http://localhost:5000
   ```

2. **Verify the application is running:**
   - Open your browser and navigate to: http://localhost:5000/swagger
   - You should see the Swagger UI with all available endpoints

## CORS Configuration

The application is configured with CORS to allow frontend connections:

- **Allowed Origins**: `http://localhost:5173` (Vite), `http://localhost:3000` (Create React App)
- **Allowed Methods**: All HTTP methods (GET, POST, PUT, DELETE, etc.)
- **Allowed Headers**: All headers
- **CORS Middleware**: Applied before other middleware for proper handling

### Testing CORS
Use the provided `test_cors.html` file to verify CORS functionality:
1. Open `test_cors.html` in your browser
2. Click "Test CORS Headers" to verify CORS configuration
3. Test API endpoints to ensure they're accessible from frontend origins

## Testing the API

### Option 1: Use the PowerShell Test Script
Run the automated test script:
```powershell
.\test_api.ps1
```

### Option 2: Manual Testing with Swagger
1. Open http://localhost:5000/swagger in your browser
2. Test the following endpoints:

#### TestController Endpoints
- `GET /api/test/employees` - Get all GeoPacific employees
- `GET /api/test/contractors` - Get all contractors
- `GET /api/test/people` - Get all people (employees + contractors)

#### PeopleController Endpoints
- `GET /api/people` - Get all people with detailed information
- `GET /api/people/employees/{id}` - Get specific employee
- `GET /api/people/contractors/{id}` - Get specific contractor

#### JobsController Endpoints
- `GET /api/jobs` - Get all jobs with related information

### Option 3: Use curl or Postman
```bash
# Test employees endpoint
curl http://localhost:5000/api/test/employees

# Test contractors endpoint
curl http://localhost:5000/api/test/contractors

# Test people endpoint
curl http://localhost:5000/api/people
```

## Expected Results

### Database Structure
The refactored models now use composition instead of inheritance:

- **PersonalInfo**: Base table for all people
- **GeoPacificEmployee**: Links to PersonalInfo via PersonalInfoId
- **Contractor**: Links to PersonalInfo via PersonalInfoId + CompanyName
- **Client**: Represents client companies
- **Job**: Links to Client instead of storing ClientName as string

### Sample Response Structure
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@geopacific.com",
  "phoneNumber": "+1-555-0101",
  "personType": "GeoPacific Employee",
  "companyName": null,
  "role": "Admin"
}
```

## Troubleshooting

### Common Issues

1. **Build Errors**: Make sure no other instance of the application is running
2. **Database Connection**: Verify your connection string in `appsettings.Development.json`
3. **Migration Issues**: If you encounter schema conflicts, you may need to drop and recreate the database

### Database Reset (if needed)
```bash
# Remove existing migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop --force

# Add new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## Next Steps

Once the API is working correctly:
1. Test all endpoints to ensure they return the expected data
2. Verify that the new model relationships work as intended
3. Consider adding more comprehensive test data
4. Test the frontend integration if applicable

## Frontend Integration

### CORS is Already Configured
Your backend is ready for frontend integration with CORS properly configured for:
- React (Create React App) on `http://localhost:3000`
- Vue (Vite) on `http://localhost:5173`
- Any other frontend framework on these ports

### Example Frontend API Calls
```javascript
// Fetch employees
const employees = await fetch('http://localhost:5000/api/test/employees')
  .then(res => res.json());

// Fetch contractors
const contractors = await fetch('http://localhost:5000/api/test/contractors')
  .then(res => res.json());

// Fetch all people
const people = await fetch('http://localhost:5000/api/people')
  .then(res => res.json());
```

### Testing Frontend-Backend Communication
1. Open `test_cors.html` in your browser
2. Run the CORS tests to verify cross-origin requests work
3. Test API endpoints to ensure data flows correctly
4. Use browser DevTools to inspect CORS headers and requests

## Support

If you encounter issues:
1. Check the application logs for detailed error messages
2. Verify database connectivity and schema
3. Ensure all required dependencies are installed
