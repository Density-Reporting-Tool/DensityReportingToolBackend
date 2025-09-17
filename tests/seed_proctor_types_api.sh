#!/bin/bash

# Script to seed ProctorTypes using the API (since we don't have direct DB access)
echo "Seeding ProctorTypes via Database..."
echo "===================================="

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_success() { echo -e "${GREEN}✅ $1${NC}"; }
print_error() { echo -e "${RED}❌ $1${NC}"; }
print_info() { echo -e "${YELLOW}ℹ️  $1${NC}"; }

print_info "Note: This script would need direct database access to seed ProctorTypes."
print_info "For now, let's check if ProctorTypes exist by testing the proctor creation."
print_info ""
print_info "If the test fails due to missing ProctorTypes, you'll need to:"
print_info "1. Use a database client (like pgAdmin, DBeaver, or TablePlus)"
print_info "2. Connect to your PostgreSQL database"
print_info "3. Run this SQL:"
print_info ""
echo "   INSERT INTO \"ProctorTypes\" (\"Type\") VALUES "
echo "   ('SPDD'),"
echo "   ('MPDD')"
echo "   ON CONFLICT (\"Type\") DO NOTHING;"
print_info ""
print_info "Alternative: If you have psql installed, run:"
print_info "   psql -d your_database_name -f tests/seed_proctor_types.sql"
print_info ""
print_info "Let's test the current setup..."

# Test if we can create a proctor (this will tell us if ProctorTypes exist)
cd /Users/senyk/Desktop/DRT/DensityReportingToolBackend
./tests/test_add_proctor.sh
