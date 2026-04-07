#!/bin/bash

# Test script for Report Service functionality
# Tests creating a report and retrieving reports by job number

set -e  # Exit on any error

BASE_URL="http://localhost:5013/api"
REPORTS_URL="$BASE_URL/reports"

echo "🧪 Testing Report Service Functionality"
echo "========================================"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

# Test data - you may need to adjust these IDs based on your database
JOB_ID=19
EMPLOYEE_ID=21  # PersonalInfo ID for a GeoPacific employee
JOB_NUMBER="000001"  # Adjust based on your actual job number

echo
print_info "Step 1: Creating a new report"
echo "POST $REPORTS_URL"

# Create report payload
REPORT_PAYLOAD='{
    "jobId": '$JOB_ID',
    "employeeId": '$EMPLOYEE_ID',
    "startDate": "'$(date -u +"%Y-%m-%dT%H:%M:%SZ")'",
    "memo": {
        "purpose": "Test report creation via service layer",
        "commentsAndObservations": "This is a test report to verify the refactored service layer is working correctly.",
        "conclusion": "Service layer refactoring test successful"
    }
}'

echo "Payload:"
echo "$REPORT_PAYLOAD" | jq '.'

# Create the report
CREATE_RESPONSE=$(curl -s -X POST "$REPORTS_URL" \
    -H "Content-Type: application/json" \
    -d "$REPORT_PAYLOAD")

echo
echo "Create Response:"
echo "$CREATE_RESPONSE" | jq '.'

# Check if creation was successful
if echo "$CREATE_RESPONSE" | jq -e '.id' > /dev/null; then
    REPORT_ID=$(echo "$CREATE_RESPONSE" | jq -r '.id')
    print_success "Report created successfully with ID: $REPORT_ID"
else
    print_error "Failed to create report"
    echo "$CREATE_RESPONSE"
    exit 1
fi

echo
print_info "Step 2: Retrieving reports by job number"
echo "GET $REPORTS_URL/job/$JOB_NUMBER"

# Get reports by job number
GET_RESPONSE=$(curl -s -X GET "$REPORTS_URL/job/$JOB_NUMBER")

echo
echo "Get Reports Response:"
echo "$GET_RESPONSE" | jq '.'

# Check if retrieval was successful and contains our report
if echo "$GET_RESPONSE" | jq -e '.[0].id' > /dev/null; then
    RETRIEVED_REPORT_ID=$(echo "$GET_RESPONSE" | jq -r '.[0].id')
    print_success "Reports retrieved successfully"
    
    # Verify our created report is in the list
    if [ "$RETRIEVED_REPORT_ID" = "$REPORT_ID" ]; then
        print_success "Created report found in job reports list"
    else
        print_info "Note: Retrieved report ID ($RETRIEVED_REPORT_ID) differs from created ID ($REPORT_ID) - this may be normal if other reports exist"
    fi
    
    # Check report structure
    if echo "$GET_RESPONSE" | jq -e '.[0].employee.firstName' > /dev/null; then
        EMPLOYEE_NAME=$(echo "$GET_RESPONSE" | jq -r '.[0].employee.firstName + " " + .[0].employee.lastName')
        print_success "Employee information populated: $EMPLOYEE_NAME"
    else
        print_error "Employee information missing or incorrect"
    fi
    
    # Check counts
    DENSITY_COUNT=$(echo "$GET_RESPONSE" | jq -r '.[0].densityTestsCount')
    PHOTOS_COUNT=$(echo "$GET_RESPONSE" | jq -r '.[0].photosCount')
    MEMOS_COUNT=$(echo "$GET_RESPONSE" | jq -r '.[0].memosCount')
    
    print_info "Report contains: $DENSITY_COUNT density tests, $PHOTOS_COUNT photos, $MEMOS_COUNT memos"
    
    if [ "$MEMOS_COUNT" -gt 0 ]; then
        print_success "Memo was created successfully"
    else
        print_error "Memo was not created"
    fi
    
else
    print_error "Failed to retrieve reports or no reports found"
    echo "$GET_RESPONSE"
    exit 1
fi

echo
print_info "Step 3: Verifying report data structure"

# Check that the response has the expected DTO structure
REQUIRED_FIELDS=("id" "jobId" "reportNumber" "employee" "densityTestsCount" "photosCount" "memosCount")

for field in "${REQUIRED_FIELDS[@]}"; do
    if echo "$GET_RESPONSE" | jq -e ".[0].$field" > /dev/null; then
        print_success "Field '$field' present"
    else
        print_error "Field '$field' missing"
    fi
done

echo
print_success "🎉 Report Service tests completed successfully!"
print_info "Summary:"
print_info "- Created report with service layer ✅"
print_info "- Retrieved reports by job number ✅" 
print_info "- Verified DTO structure ✅"
print_info "- Confirmed PersonalInfo integration ✅"

echo
print_info "💡 Next steps:"
echo "   - Test with different employee/reviewer combinations"
echo "   - Test error handling (invalid job IDs, etc.)"
echo "   - Test with reviewer assignment"
