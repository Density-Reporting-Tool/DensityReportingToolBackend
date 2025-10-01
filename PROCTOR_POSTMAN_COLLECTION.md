# Proctor API - Postman Collection Guide

Base URL: `http://localhost:5000` or your deployed URL

---

## 📋 General Endpoints

### 1. Get All Proctors
**GET** `/api/proctors`

```
GET {{baseUrl}}/api/proctors
```

**Response:**
```json
[
  {
    "id": 1,
    "proctorId": "P-001",
    "jobNumber": "12345",
    "materialType": "Soil",
    "proctorType": "MPDD",
    "maxDensity": 125.5,
    "dateTested": "2024-01-15T00:00:00Z"
  }
]
```

---

### 2. Get Proctor by ID
**GET** `/api/proctors/{proctorId}`

```
GET {{baseUrl}}/api/proctors/1
```

**Response:**
```json
{
  "id": 1,
  "proctorId": "P-001",
  "jobNumber": "12345",
  "materialType": "Soil",
  "labLocation": "Main Lab",
  "proctorType": "MPDD",
  "maxDensity": 125.5,
  "correctedDensity": 123.0,
  "optimumMoisture": 12.5,
  "specificGravity": 2.65,
  "oversizePercentage": 5.0,
  "dateSampled": "2024-01-10T00:00:00Z",
  "dateTested": "2024-01-15T00:00:00Z",
  "densityTestsCount": 10,
  "additionalJobsCount": 2
}
```

---

### 3. Create Proctor (Simple)
**POST** `/api/proctors`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "jobNumber": "12345",
  "proctorId": "P-002",
  "proctorTestNumber": "PT-2024-002",
  "materialType": "Clay Soil",
  "labLocation": "Main Lab",
  "proctorType": "MPDD",
  "maxDensity": 2056.0,
  "correctedDensity": 2018.0,
  "optimumMoisture": 14.2,
  "specificGravity": 2.68,
  "oversizePercentage": 3.5,
  "dateSampled": "2024-01-20T00:00:00Z",
  "dateTested": "2024-01-25T00:00:00Z"
}
```

**Response:**
```json
{
  "id": 2,
  "proctorId": "P-002",
  "jobNumber": "12345",
  "materialType": "Clay Soil",
  "proctorType": "MPDD"
}
```

---

### 4. Update Proctor
**PUT** `/api/proctors/{proctorId}`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "id": 2,
  "proctorId": "P-002",
  "materialType": "Updated Clay Soil",
  "labLocation": "Main Lab",
  "proctorType": "MPDD",
  "maxDensity": 2064.0,
  "correctedDensity": 2032.0,
  "optimumMoisture": 14.5,
  "specificGravity": 2.70,
  "oversizePercentage": 3.0,
  "dateSampled": "2024-01-20T00:00:00Z",
  "dateTested": "2024-01-26T00:00:00Z"
}
```

**Response:**
```json
{
  "id": 2,
  "proctorId": "P-002",
  "jobNumber": "12345",
  "materialType": "Updated Clay Soil",
  "proctorType": "MPDD",
  "maxDensity": 129.0,
  "dateTested": "2024-01-26T00:00:00Z"
}
```

---

### 5. Search Proctors by Job Number
**GET** `/api/proctors/search?jobNumber={jobNumber}&limit={limit}`

```
GET {{baseUrl}}/api/proctors/search?jobNumber=12345&limit=10
```

**Query Parameters:**
- `jobNumber` (required): Job number to search (supports partial match)
- `limit` (optional, default: 10): Maximum results to return

**Response:**
```json
[
  {
    "id": 1,
    "proctorId": "P-001",
    "jobNumber": "12345",
    "materialType": "Soil",
    "proctorType": "MPDD",
    "maxDensity": 125.5,
    "dateTested": "2024-01-15T00:00:00Z"
  }
]
```

---

## 🔬 Lab Admin Endpoints

### 6. Create Proctor (Lab Admin - Legacy Format)
**POST** `/api/proctors/lab-admin`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "jobNumber": "12345",
  "proctorTestNumber": "PT-2024-003",
  "materialType": "Sandy Soil",
  "dateSampled": "2024-02-01",
  "proctorType": "SPDD",
  "maxDryDensity": "122.5",
  "correctedDensity": "120.0",
  "labLocation": "Main Lab",
  "proctorId": "P-003",
  "dateTested": "2024-02-05",
  "oversizePercentage": 2.5,
  "optimumMoisture": 10.5,
  "specificGravity": "2.62"
}
```

**Response:**
```json
{
  "id": "3",
  "message": "Proctor created successfully",
  "proctor": {
    "jobNumber": "12345",
    "proctorTestNumber": "PT-2024-003",
    "materialType": "Sandy Soil",
    "dateSampled": "2024-02-01",
    "proctorType": "SPDD",
    "maxDryDensity": "122.5",
    "correctedDensity": "120.0",
    "labLocation": "Main Lab",
    "proctorId": "P-003",
    "dateTested": "2024-02-05",
    "oversizePercentage": 2.5,
    "optimumMoisture": 10.5,
    "specificGravity": "2.62"
  }
}
```

---

### 7. Update Proctor (Lab Admin - Legacy Format)
**PUT** `/api/proctors/lab-admin/{id}`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "proctorTestNumber": "PT-2024-003-UPDATED",
  "materialType": "Updated Sandy Soil",
  "dateSampled": "2024-02-01",
  "proctorType": "SPDD",
  "maxDryDensity": "123.0",
  "correctedDensity": "121.0",
  "labLocation": "Main Lab",
  "dateTested": "2024-02-06",
  "oversizePercentage": 2.0,
  "optimumMoisture": 11.0,
  "specificGravity": "2.63"
}
```

**Response:**
```json
{
  "id": "3",
  "message": "Proctor updated successfully",
  "proctor": {
    "jobNumber": "12345",
    "proctorTestNumber": "PT-2024-003-UPDATED",
    "materialType": "Updated Sandy Soil",
    "dateSampled": "2024-02-01",
    "proctorType": "SPDD",
    "maxDryDensity": "123.0",
    "correctedDensity": "121.0",
    "labLocation": "Main Lab",
    "proctorId": "P-003",
    "dateTested": "2024-02-06",
    "oversizePercentage": 2.0,
    "optimumMoisture": 11.0,
    "specificGravity": "2.63"
  }
}
```

---

### 8. Get All Proctors (Lab Admin - Paginated)
**GET** `/api/proctors/lab-admin?page={page}&limit={limit}&jobNumber={jobNumber}`

```
GET {{baseUrl}}/api/proctors/lab-admin?page=1&limit=50&jobNumber=12345
```

**Query Parameters:**
- `page` (optional, default: 1): Page number
- `limit` (optional, default: 50): Items per page
- `jobNumber` (optional): Filter by job number

**Response:**
```json
{
  "proctors": [
    {
      "jobNumber": "12345",
      "proctorTestNumber": "PT-2024-001",
      "materialType": "Soil",
      "dateSampled": "2024-01-10",
      "proctorType": "MPDD",
      "maxDryDensity": "125.5",
      "correctedDensity": "123.0",
      "labLocation": "Main Lab",
      "proctorId": "P-001",
      "dateTested": "2024-01-15",
      "oversizePercentage": 5.0,
      "optimumMoisture": 12.5,
      "specificGravity": "2.65"
    }
  ],
  "total": 15,
  "page": 1,
  "limit": 50,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

---

## 👷 Field Tech Endpoints

### 9. Get Density Requirements for Field Testing
**GET** `/api/proctors/field-tech/{id}/density-requirements`

```
GET {{baseUrl}}/api/proctors/field-tech/1/density-requirements
```

**Response:**
```json
{
  "proctorId": 1,
  "proctorTestNumber": "PT-2024-001",
  "maxDryDensity": 125.5,
  "correctedDensity": 123.0,
  "optimumMoisture": 12.5,
  "compactionRequirement": "95% of maximum dry density",
  "targetDensity95": 119.225,
  "targetDensity90": 112.95,
  "targetDensity98": 122.99,
  "materialType": "Soil",
  "testMethod": "Modified Proctor (MPDD)",
  "proctorType": "MPDD",
  "specificGravity": 2.65,
  "oversizePercentage": 5.0,
  "moistureGuidance": "Target moisture content: 12.5% ± 2%"
}
```

---

## 🔗 Shared Endpoints

### 10. Get All Proctors for a Specific Job
**GET** `/api/proctors/job/{jobNumber}`

```
GET {{baseUrl}}/api/proctors/job/12345
```

**Response:**
```json
[
  {
    "jobNumber": "12345",
    "proctorTestNumber": "PT-2024-001",
    "materialType": "Soil",
    "dateSampled": "2024-01-10",
    "proctorType": "MPDD",
    "maxDryDensity": "125.5",
    "correctedDensity": "123.0",
    "labLocation": "Main Lab",
    "proctorId": "P-001",
    "dateTested": "2024-01-15",
    "oversizePercentage": 5.0,
    "optimumMoisture": 12.5,
    "specificGravity": "2.65"
  }
]
```

---

## 🛠️ Postman Environment Variables

Create a Postman Environment with these variables:

```json
{
  "baseUrl": "http://localhost:5000",
  "jobNumber": "12345",
  "proctorId": "1"
}
```

Use them in requests like: `{{baseUrl}}/api/proctors/{{proctorId}}`

---

## ⚠️ Common Validation Rules

### ProctorType
- Must be either `"SPDD"` or `"MPDD"`

### MaxDensity / CorrectedDensity
- Must be between 0 and 3000 kg/m³
- Typical soil densities: 1400-2400 kg/m³

### OptimumMoisture
- Must be between 0 and 100

### OversizePercentage
- Must be between 0 and 100

### SpecificGravity
- Must be between 0 and 10

### DateTested
- Cannot be before DateSampled

---

## 📝 Error Responses

### 400 Bad Request
```json
{
  "errors": [
    {
      "field": "JobNumber",
      "message": "Job Number is required"
    }
  ]
}
```

### 404 Not Found
```json
{
  "message": "Proctor with ID 999 not found",
  "proctorId": 999
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while retrieving the proctor"
}
```

---

## 🚀 Quick Test Sequence

1. **Create a Job first** (use JobsController)
2. **Create Proctor**: `POST /api/proctors` with job number
3. **Get by ID**: `GET /api/proctors/1`
4. **Search**: `GET /api/proctors/search?jobNumber=12345`
5. **Update**: `PUT /api/proctors/1`
6. **Get Density Requirements**: `GET /api/proctors/field-tech/1/density-requirements`
7. **Get All for Job**: `GET /api/proctors/job/12345`

---

## 📦 Import to Postman

You can import this collection directly into Postman by creating a new collection and adding these requests manually, or use the Postman UI to auto-generate from the Swagger/OpenAPI documentation if available.

