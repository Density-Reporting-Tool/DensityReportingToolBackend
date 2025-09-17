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
```

You can paste this directly into your README.md file!