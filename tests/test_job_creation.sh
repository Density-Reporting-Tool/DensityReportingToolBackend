#!/bin/bash

# Test script to create a job via the API using curl
BASE_URL="http://localhost:5013"  # Adjust port if needed
ENDPOINT="$BASE_URL/api/jobs"

echo "Testing Job Creation API..."
echo "Endpoint: $ENDPOINT"
echo ""

# Generate random job number between 15000 and 25000
RANDOM_JOB_NUMBER=$((RANDOM % 10000 + 15000))
echo "Generated random job number: $RANDOM_JOB_NUMBER"

# Test job data
JOB_DATA='{
    "JobNumber": "'$RANDOM_JOB_NUMBER'",
    "ClientName": "Test Construction Company",
    "ProjectName": "Test Project - cURL Validation",
    "SiteAddress": "456 Test Avenue, Test City, TC 67890",
    "StartDate": "2024-02-01T00:00:00Z",
    "EndDate": "2024-07-01T00:00:00Z",
    "JobNotes": "This is a test job created via cURL to validate the job creation functionality."
}'

echo "Data: $JOB_DATA"
echo ""

# Make the POST request
echo "Making POST request..."
RESPONSE=$(curl -s -w "\nHTTP_CODE:%{http_code}" \
    -X POST \
    -H "Content-Type: application/json" \
    -d "$JOB_DATA" \
    "$ENDPOINT")

# Extract HTTP code and response body
HTTP_CODE=$(echo "$RESPONSE" | grep "HTTP_CODE:" | cut -d: -f2)
RESPONSE_BODY=$(echo "$RESPONSE" | sed '/HTTP_CODE:/d')

echo "HTTP Status Code: $HTTP_CODE"
echo "Response Body: $RESPONSE_BODY"

if [ "$HTTP_CODE" = "201" ]; then
    echo "✅ SUCCESS! Job created successfully"
    echo "$RESPONSE_BODY" | jq '.' 2>/dev/null || echo "$RESPONSE_BODY"
else
    echo "❌ ERROR: Failed to create job"
    echo "Status Code: $HTTP_CODE"
    echo "Response: $RESPONSE_BODY"
fi
