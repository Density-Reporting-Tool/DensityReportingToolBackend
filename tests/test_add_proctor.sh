#!/bin/bash

# Test script to add a proctor and retrieve it from the database
echo "Testing Proctor Creation and Retrieval"
echo "======================================"

# Configuration
API_BASE="http://localhost:5013"
PROCTOR_ENDPOINT="$API_BASE/api/proctors"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_success() { echo -e "${GREEN}✅ $1${NC}"; }
print_error() { echo -e "${RED}❌ $1${NC}"; }
print_info() { echo -e "${YELLOW}ℹ️  $1${NC}"; }

# Test data
TEST_JOB_NUMBER="TEST-$(date +%s)"
PROCTOR_DATA='{
  "jobNumber": "'$TEST_JOB_NUMBER'",
  "proctorTestNumber": "P-001",
  "materialType": "Granular Base",
  "dateSampled": "2024-01-15",
  "proctorType": "MPDD",
  "maxDryDensity": "1900.5",
  "correctedDensity": "1915.8",
  "labLocation": "Main Lab",
  "proctorId": "P-001",
  "dateTested": "2024-01-16",
  "oversizePercentage": 5.2,
  "optimumMoisture": 12.3,
  "specificGravity": "2.65"
}'

# First, create a job (since proctor requires existing job)
print_info "Step 1: Creating test job..."
JOB_DATA='{
  "jobNumber": "'$TEST_JOB_NUMBER'",
  "clientName": "Test Client",
  "projectName": "Test Project",
  "siteAddress": "123 Test Street"
}'

JOB_RESPONSE=$(curl -s -X POST "$API_BASE/api/jobs" \
  -H "Content-Type: application/json" \
  -d "$JOB_DATA" \
  -w "HTTPSTATUS:%{http_code}")

JOB_HTTP_STATUS=$(echo $JOB_RESPONSE | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
JOB_BODY=$(echo $JOB_RESPONSE | sed -e 's/HTTPSTATUS:.*//g')

if [ $JOB_HTTP_STATUS -eq 201 ]; then
    print_success "Job created successfully"
    echo "Job Response: $JOB_BODY"
else
    print_error "Failed to create job (HTTP $JOB_HTTP_STATUS)"
    echo "Response: $JOB_BODY"
    exit 1
fi

# Step 2: Add Proctor
print_info "Step 2: Adding proctor..."
PROCTOR_RESPONSE=$(curl -s -X POST "$PROCTOR_ENDPOINT/lab-admin" \
  -H "Content-Type: application/json" \
  -d "$PROCTOR_DATA" \
  -w "HTTPSTATUS:%{http_code}")

PROCTOR_HTTP_STATUS=$(echo $PROCTOR_RESPONSE | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
PROCTOR_BODY=$(echo $PROCTOR_RESPONSE | sed -e 's/HTTPSTATUS:.*//g')

if [ $PROCTOR_HTTP_STATUS -eq 201 ]; then
    print_success "Proctor created successfully"
    echo "Proctor Response: $PROCTOR_BODY"
    
    # Extract proctor ID from response
    PROCTOR_ID=$(echo $PROCTOR_BODY | grep -o '"id":"[^"]*"' | cut -d'"' -f4)
    print_info "Created Proctor ID: $PROCTOR_ID"
else
    print_error "Failed to create proctor (HTTP $PROCTOR_HTTP_STATUS)"
    echo "Response: $PROCTOR_BODY"
    exit 1
fi

# Step 3: Retrieve Proctor by ID
if [ -n "$PROCTOR_ID" ]; then
    print_info "Step 3: Retrieving proctor by ID..."
    GET_RESPONSE=$(curl -s -X GET "$PROCTOR_ENDPOINT/$PROCTOR_ID" \
      -H "Content-Type: application/json" \
      -w "HTTPSTATUS:%{http_code}")

    GET_HTTP_STATUS=$(echo $GET_RESPONSE | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
    GET_BODY=$(echo $GET_RESPONSE | sed -e 's/HTTPSTATUS:.*//g')

    if [ $GET_HTTP_STATUS -eq 200 ]; then
        print_success "Proctor retrieved successfully"
        echo "Retrieved Proctor: $GET_BODY"
    else
        print_error "Failed to retrieve proctor (HTTP $GET_HTTP_STATUS)"
        echo "Response: $GET_BODY"
    fi
else
    print_error "Could not extract proctor ID from creation response"
fi

# Step 4: Get Proctors for Job
print_info "Step 4: Retrieving proctors for job..."
JOB_PROCTORS_RESPONSE=$(curl -s -X GET "$PROCTOR_ENDPOINT/job/$TEST_JOB_NUMBER" \
  -H "Content-Type: application/json" \
  -w "HTTPSTATUS:%{http_code}")

JOB_PROCTORS_HTTP_STATUS=$(echo $JOB_PROCTORS_RESPONSE | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
JOB_PROCTORS_BODY=$(echo $JOB_PROCTORS_RESPONSE | sed -e 's/HTTPSTATUS:.*//g')

if [ $JOB_PROCTORS_HTTP_STATUS -eq 200 ]; then
    print_success "Job proctors retrieved successfully"
    echo "Job Proctors: $JOB_PROCTORS_BODY"
else
    print_error "Failed to retrieve job proctors (HTTP $JOB_PROCTORS_HTTP_STATUS)"
    echo "Response: $JOB_PROCTORS_BODY"
fi

# Step 5: Test Validation (try to create proctor with invalid job)
print_info "Step 5: Testing validation (invalid job number)..."
INVALID_PROCTOR_DATA='{
  "jobNumber": "NONEXISTENT-999",
  "proctorTestNumber": "P-002",
  "materialType": "Test Material",
  "dateSampled": "2024-01-15",
  "proctorType": "SPDD",
  "maxDryDensity": "120.0",
  "correctedDensity": "118.0",
  "labLocation": "Test Lab",
  "proctorId": "P-002",
  "dateTested": "2024-01-16",
  "oversizePercentage": 3.0,
  "optimumMoisture": 10.0,
  "specificGravity": "2.50"
}'

INVALID_RESPONSE=$(curl -s -X POST "$PROCTOR_ENDPOINT/lab-admin" \
  -H "Content-Type: application/json" \
  -d "$INVALID_PROCTOR_DATA" \
  -w "HTTPSTATUS:%{http_code}")

INVALID_HTTP_STATUS=$(echo $INVALID_RESPONSE | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')
INVALID_BODY=$(echo $INVALID_RESPONSE | sed -e 's/HTTPSTATUS:.*//g')

if [ $INVALID_HTTP_STATUS -eq 400 ]; then
    print_success "Validation working correctly (rejected invalid job)"
    echo "Error Response: $INVALID_BODY"
else
    print_error "Validation not working (HTTP $INVALID_HTTP_STATUS)"
    echo "Response: $INVALID_BODY"
fi

echo ""
print_info "Test Summary:"
echo "- Job Creation: $([ $JOB_HTTP_STATUS -eq 201 ] && echo "✅ PASS" || echo "❌ FAIL")"
echo "- Proctor Creation: $([ $PROCTOR_HTTP_STATUS -eq 201 ] && echo "✅ PASS" || echo "❌ FAIL")"
echo "- Proctor Retrieval: $([ $GET_HTTP_STATUS -eq 200 ] && echo "✅ PASS" || echo "❌ FAIL")"
echo "- Job Proctors List: $([ $JOB_PROCTORS_HTTP_STATUS -eq 200 ] && echo "✅ PASS" || echo "❌ FAIL")"
echo "- Validation Test: $([ $INVALID_HTTP_STATUS -eq 400 ] && echo "✅ PASS" || echo "❌ FAIL")"

echo ""
print_info "Test completed!"
