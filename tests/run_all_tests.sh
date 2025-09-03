#!/bin/bash

# Comprehensive test runner for all API tests
echo "🧪 Running All API Tests"
echo "========================="
echo ""

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Check if application is running
echo "🔍 Checking if application is running..."
if curl -s http://localhost:5013/api/jobs > /dev/null 2>&1; then
    echo "✅ Application is running on http://localhost:5013"
else
    echo "❌ Application is not running. Please start it with 'dotnet run'"
    exit 1
fi

echo ""
echo "🚀 Running Quick Test..."
echo "------------------------"
cd "$SCRIPT_DIR" && ./quick_test.sh

echo ""
echo "🚀 Running Full Test..."
echo "----------------------"
cd "$SCRIPT_DIR" && ./test_job_creation.sh

echo ""
echo "✅ All tests completed!"
echo "📊 Check the output above for results."
