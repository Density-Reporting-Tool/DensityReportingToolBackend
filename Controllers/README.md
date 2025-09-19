## Proctor API Endpoints

### Lab Admin Routes
| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| `POST` | `/api/proctors/lab-admin` | Create a new proctor | `CreateProctorRequest` | `ProctorCreateResponse` |
| `PUT` | `/api/proctors/lab-admin/{id}` | Update existing proctor | `UpdateProctorRequest` | `ProctorUpdateResponse` |
| `GET` | `/api/proctors/lab-admin` | Get all proctors (paginated) | Query params: `page`, `limit`, `jobNumber` | `ProctorListResponse` |

### Field Tech Routes
| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| `GET` | `/api/proctors/field-tech/{id}/density-requirements` | Get density requirements for field testing | - | `DensityRequirementsResponse` |

### Shared Routes
| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| `GET` | `/api/proctors/{id}` | Get specific proctor by ID | - | `ProctorDataResponse` |
| `GET` | `/api/proctors/job/{jobNumber}` | Get all proctors for a job | - | `ProctorDataResponse[]` |

### Example Usage

**Create Proctor (Lab Admin):**
```http
POST /api/proctors/lab-admin
Content-Type: application/json

{
  "jobNumber": "12345",
  "proctorTestNumber": "P-001",
  "materialType": "Granular Base",
  "dateSampled": "2024-01-15",
  "proctorType": "Modified",
  "maxDryDensity": "125.5",
  "correctedDensity": "123.8",
  "labLocation": "Main Lab",
  "proctorId": "P-001",
  "dateTested": "2024-01-16",
  "oversizePercentage": 5.2,
  "optimumMoisture": 12.3,
  "specificGravity": "2.65"
}
```

**Get Density Requirements (Field Tech):**
```http
GET /api/proctors/field-tech/5/density-requirements
```

**Response:**
```json
{
  "proctorId": 5,
  "proctorTestNumber": "P-001",
  "maxDryDensity": 125.5,
  "correctedDensity": 123.8,
  "optimumMoisture": 12.3,
  "compactionRequirement": "95% of maximum dry density",
  "targetDensity95": 119.225,
  "targetDensity90": 112.95,
  "materialType": "Granular Base",
  "testMethod": "ASTM D1557",
  "proctorType": "Modified"
}
```

**Get Proctors for Job (Shared):**
```http
GET /api/proctors/job/12345
```

---

## Report API Endpoints

### Report Management Routes
| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| `POST` | `/api/reports` | Create a new report | `CreateReportRequest` | `ReportCreateResponse` |
| `GET` | `/api/reports/job/{jobNumber}` | Get all reports for a job (newest first) | - | `ReportListByJobResponse[]` |
| `GET` | `/api/reports/{reportId}` | Get specific report by ID with full details | - | `ReportDetailResponse` |

### Supporting Routes
| Method | Route | Description | Request Body | Response |
|--------|-------|-------------|--------------|----------|
| `GET` | `/api/reports/proctors/job/{jobId}` | Get proctors available for a job | - | `ProctorInfo[]` |
| `POST` | `/api/reports/{reportId}/density-test` | Create density test for a report | `CreateDensityTestRequest` | `DensityTestResponse` |

### Example Usage

**Create Report:**
```http
POST /api/reports
Content-Type: application/json

{
  "jobId": 19,
  "employeeId": 21,
  "reviewerId": 25,
  "startDate": "2025-09-19T10:00:00Z",
  "memo": {
    "purpose": "Daily compaction testing report",
    "commentsAndObservations": "Testing performed on granular base material. Weather conditions were optimal.",
    "conclusion": "All density tests met specification requirements."
  }
}
```

**Response:**
```json
{
  "id": "17",
  "message": "Report created successfully",
  "report": {
    "id": 17,
    "jobId": 19,
    "reportNumber": 4,
    "startDate": "2025-09-19T10:00:00Z",
    "employee": {
      "id": 21,
      "firstName": "Mark",
      "lastName": "Jackson",
      "email": "mark.jackson@geopacific.test.com",
      "phoneNumber": "555-0402"
    },
    "reviewer": {
      "id": 25,
      "firstName": "Patricia",
      "lastName": "White",
      "email": "patricia.white@geopacific.test.com",
      "phoneNumber": "555-0501"
    },
    "job": {
      "id": 19,
      "jobNumber": "000001",
      "clientName": "ABC Construction",
      "projectName": "Highway Expansion Project"
    }
  }
}
```

**Get Reports for Job:**
```http
GET /api/reports/job/000001
```

**Response:**
```json
[
  {
    "id": 17,
    "jobId": 19,
    "reportNumber": 4,
    "startDate": "2025-09-19T10:00:00Z",
    "employee": {
      "id": 21,
      "firstName": "Mark",
      "lastName": "Jackson",
      "email": "mark.jackson@geopacific.test.com",
      "phoneNumber": "555-0402"
    },
    "reviewer": {
      "id": 25,
      "firstName": "Patricia",
      "lastName": "White",
      "email": "patricia.white@geopacific.test.com",
      "phoneNumber": "555-0501"
    },
    "densityTestsCount": 5,
    "photosCount": 3,
    "memosCount": 1,
    "distributionListId": null
  }
]
```

**Get Detailed Report:**
```http
GET /api/reports/17
```

**Response:**
```json
{
  "id": 17,
  "jobId": 19,
  "job": {
    "id": 19,
    "jobNumber": "000001",
    "clientName": "ABC Construction",
    "projectName": "Highway Expansion Project"
  },
  "reportNumber": 4,
  "startDate": "2025-09-19T10:00:00Z",
  "employee": {
    "id": 21,
    "firstName": "Mark",
    "lastName": "Jackson",
    "email": "mark.jackson@geopacific.test.com",
    "phoneNumber": "555-0402"
  },
  "reviewer": {
    "id": 25,
    "firstName": "Patricia",
    "lastName": "White",
    "email": "patricia.white@geopacific.test.com",
    "phoneNumber": "555-0501"
  },
  "densityTests": [
    {
      "id": 1,
      "testArea": "Area A",
      "location": "Station 100+50",
      "elevationReference": "AboveSubgrade",
      "elevationValue": 2.5,
      "elevationUnit": "Meters",
      "compactionSpecification": 95.0,
      "compactionSpecificationUnit": "SPDD",
      "densityValue": 119.2,
      "moistureValue": 12.8,
      "createdDate": "2025-09-19T10:30:00Z"
    }
  ],
  "photos": [
    {
      "id": 1,
      "code": "P001",
      "url": "/photos/report-17-photo-1.jpg",
      "description": "Test location overview",
      "latitude": 49.2827,
      "longitude": -123.1207,
      "gpsAccuracyMeters": 3.5
    }
  ],
  "memos": [
    {
      "id": 1,
      "purpose": "Daily compaction testing report",
      "commentsAndObservations": "Testing performed on granular base material. Weather conditions were optimal.",
      "conclusion": "All density tests met specification requirements.",
      "createdDate": "2025-09-19T10:00:00Z",
      "updatedDate": null
    }
  ],
  "distributionListId": null
}
```

### Notes for Frontend Development

**Important Considerations:**
- **Job Numbers**: Use job numbers (strings like "000001") instead of internal job IDs for user-friendly URLs
- **Employee/Reviewer IDs**: Must reference PersonalInfo table entries with Company = "GeoPacific"
- **Optional Reviewer**: ReviewerId can be null when creating reports (assigned later during review process)
- **Report Numbering**: Report numbers are auto-generated per job (1, 2, 3, etc.)
- **Newest First**: Reports are returned in descending order by report number (newest first)
- **Comprehensive Data**: Detailed report endpoint includes all related density tests, photos, and memos

**Error Handling:**
- 400 Bad Request: Invalid employee/reviewer IDs, missing job, validation errors
- 404 Not Found: Report or job not found
- 500 Internal Server Error: Database or server issues