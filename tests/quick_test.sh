#!/bin/bash

# Quick test script with random job number
RANDOM_JOB_NUMBER=$((RANDOM % 10000 + 15000))
echo "🧪 Testing Job Creation with random job number: $RANDOM_JOB_NUMBER"
echo ""

# Test job creation
echo "📝 Creating job..."
RESPONSE=$(curl -s -w "\nHTTP_CODE:%{http_code}" \
    -X POST \
    -H "Content-Type: application/json" \
    -d '{
        "JobNumber": "'$RANDOM_JOB_NUMBER'",
        "ClientName": "Test Construction Company",
        "ProjectName": "Test Project - Random Validation",
        "SiteAddress": "123 Random Street, Test City, TC 12345",
        "StartDate": "2024-01-15T00:00:00Z",
        "EndDate": "2024-06-15T00:00:00Z",
        "JobNotes": "This is a test job with random number: '$RANDOM_JOB_NUMBER'"
    }' \
    http://localhost:5013/api/jobs)

# Extract HTTP code and response body
HTTP_CODE=$(echo "$RESPONSE" | grep "HTTP_CODE:" | cut -d: -f2)
RESPONSE_BODY=$(echo "$RESPONSE" | sed '/HTTP_CODE:/d')

echo "📊 HTTP Status Code: $HTTP_CODE"
echo "📄 Response:"
echo "$RESPONSE_BODY" | jq '.' 2>/dev/null || echo "$RESPONSE_BODY"

if [ "$HTTP_CODE" = "201" ]; then
    echo ""
    echo "✅ SUCCESS! Job created successfully"
    echo "🔍 Now testing job retrieval..."
    
    # Test job retrieval
    RETRIEVAL_RESPONSE=$(curl -s -w "\nHTTP_CODE:%{http_code}" \
        -X GET \
        http://localhost:5013/api/jobs/$RANDOM_JOB_NUMBER)
    
    RETRIEVAL_HTTP_CODE=$(echo "$RETRIEVAL_RESPONSE" | grep "HTTP_CODE:" | cut -d: -f2)
    RETRIEVAL_BODY=$(echo "$RETRIEVAL_RESPONSE" | sed '/HTTP_CODE:/d')
    
    echo "📊 Retrieval HTTP Status Code: $RETRIEVAL_HTTP_CODE"
    echo "📄 Retrieved Job Data:"
    echo "$RETRIEVAL_BODY" | jq '.' 2>/dev/null || echo "$RETRIEVAL_BODY"
    
    if [ "$RETRIEVAL_HTTP_CODE" = "200" ]; then
        echo "✅ SUCCESS! Job retrieved successfully"
    else
        echo "❌ ERROR: Failed to retrieve job"
    fi
else
    echo "❌ ERROR: Failed to create job"
fi
