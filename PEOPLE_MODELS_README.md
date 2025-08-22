# People Models Structure

This document explains the updated people model structure for the Density Reporting Tool Backend.

## Overview

The new structure clearly separates personal information from role-specific data, making it easy to distinguish between GeoPacific employees and outside contractors while maintaining clean relationships.

## Model Structure

### 1. PersonalInfo (Base Class)
- **Purpose**: Stores generic personal information for any person in the system
- **Fields**: `Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`
- **Relationships**: 
  - One-to-one with `GeoPacificEmployee` (if the person is an employee)
  - One-to-one with `Contractor` (if the person is a contractor)

### 2. GeoPacificEmployee
- **Purpose**: Represents GeoPacific employees with role and authentication information
- **Fields**: `Id`, `PersonalInfoId`, `RoleId`, `Password`
- **Relationships**:
  - One-to-one with `PersonalInfo`
  - Many-to-one with `Role`
  - One-to-many with `JobProjectManager`

### 3. Contractor
- **Purpose**: Represents outside contractors with company information
- **Fields**: `Id`, `PersonalInfoId`, `CompanyName`, `ClientId` (optional)
- **Relationships**:
  - One-to-one with `PersonalInfo`
  - Many-to-one with `Client` (optional - if the company is a registered client)
  - One-to-many with `JobContractor`

### 4. Client
- **Purpose**: Represents client companies
- **Fields**: `Id`, `Name`, `Address`, `PhoneNumber`, `Email`
- **Relationships**:
  - One-to-many with `Contractor`
  - One-to-many with `Job`

### 5. Role
- **Purpose**: Defines employee roles within the system
- **Fields**: `Id`, `RoleTitle`
- **Relationships**: One-to-many with `GeoPacificEmployee`

## Key Benefits

1. **Clear Separation**: Easy to distinguish between employees and contractors
2. **Flexible Company Storage**: Contractors can have company names stored directly or linked to registered clients
3. **Clean Relationships**: No inheritance issues, clear foreign key relationships
4. **Extensible**: Easy to add new person types in the future
5. **Data Integrity**: Proper foreign key constraints and cascade rules

## Usage Examples

### Creating a GeoPacific Employee

```csharp
// 1. Create PersonalInfo first
var personalInfo = new PersonalInfo
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@geopacific.com",
    PhoneNumber = "+1-555-0123"
};

// 2. Create GeoPacificEmployee
var employee = new GeoPacificEmployee
{
    PersonalInfoId = personalInfo.Id,
    RoleId = 1, // Admin role
    Password = "hashedPassword"
};
```

### Creating a Contractor

```csharp
// 1. Create PersonalInfo first
var personalInfo = new PersonalInfo
{
    FirstName = "Jane",
    LastName = "Smith",
    Email = "jane.smith@contractor.com",
    PhoneNumber = "+1-555-0456"
};

// 2. Create Contractor
var contractor = new Contractor
{
    PersonalInfoId = personalInfo.Id,
    CompanyName = "ABC Construction",
    ClientId = null // Optional: link to existing client
};
```

### Querying People

```csharp
// Get all people with their type
var people = await _dbContext.PersonalInfo
    .Include(p => p.Employee)
    .ThenInclude(e => e.Role)
    .Include(p => p.Contractor)
    .ThenInclude(c => c.Client)
    .Select(p => new
    {
        p.FirstName,
        p.LastName,
        PersonType = p.Employee != null ? "GeoPacific Employee" : 
                    p.Contractor != null ? "Contractor" : "Unknown",
        CompanyName = p.Contractor?.CompanyName,
        Role = p.Employee?.Role.RoleTitle
    })
    .ToListAsync();
```

## API Endpoints

The `PeopleController` provides the following endpoints:

- `POST /api/people/employees` - Create a new GeoPacific employee
- `POST /api/people/contractors` - Create a new contractor
- `GET /api/people/employees/{id}` - Get a specific employee
- `GET /api/people/contractors/{id}` - Get a specific contractor
- `GET /api/people` - Get all people with their type and details

## Database Migration

After updating the models, you'll need to create and run a new migration:

```bash
dotnet ef migrations add UpdatePeopleModels
dotnet ef database update
```

## Seed Data

The existing seed data in `seed_data.sql` will need to be updated to work with the new structure. The current structure stores company names directly in PersonalInfo, but the new structure separates this information.

## Future Enhancements

1. **Person Types**: Could add an enum or lookup table for different person types
2. **Audit Trail**: Add created/modified timestamps to track changes
3. **Soft Delete**: Implement soft delete for people records
4. **Validation**: Add business rules (e.g., employees must have valid roles)
5. **Search**: Implement full-text search across people and companies
