-- Seed test people data for PersonalInfo table
-- Creates GeoPacific employees for testing report functionality

-- Clear existing test data (optional - uncomment if needed)
-- DELETE FROM "PersonalInfos" WHERE "Company" = 'GeoPacific' AND "Email" LIKE '%test.com';

-- Insert test GeoPacific employees
INSERT INTO "PersonalInfos" ("FirstName", "LastName", "Email", "PhoneNumber", "Company") VALUES
-- Field Technicians
('John', 'Smith', 'john.smith@geopacific.test.com', '555-0101', 'GeoPacific'),
('Sarah', 'Johnson', 'sarah.johnson@geopacific.test.com', '555-0102', 'GeoPacific'),
('Mike', 'Williams', 'mike.williams@geopacific.test.com', '555-0103', 'GeoPacific'),
('Lisa', 'Brown', 'lisa.brown@geopacific.test.com', '555-0104', 'GeoPacific'),

-- Senior Technicians / Reviewers
('David', 'Wilson', 'david.wilson@geopacific.test.com', '555-0201', 'GeoPacific'),
('Jennifer', 'Davis', 'jennifer.davis@geopacific.test.com', '555-0202', 'GeoPacific'),
('Robert', 'Miller', 'robert.miller@geopacific.test.com', '555-0203', 'GeoPacific'),

-- Project Managers
('Amanda', 'Taylor', 'amanda.taylor@geopacific.test.com', '555-0301', 'GeoPacific'),
('Chris', 'Anderson', 'chris.anderson@geopacific.test.com', '555-0302', 'GeoPacific'),

-- Lab Technicians
('Emily', 'Thomas', 'emily.thomas@geopacific.test.com', '555-0401', 'GeoPacific'),
('Mark', 'Jackson', 'mark.jackson@geopacific.test.com', '555-0402', 'GeoPacific'),

-- Quality Control / Reviewers
('Patricia', 'White', 'patricia.white@geopacific.test.com', '555-0501', 'GeoPacific'),
('James', 'Harris', 'james.harris@geopacific.test.com', '555-0502', 'GeoPacific'),
('Michelle', 'Martin', 'michelle.martin@geopacific.test.com', '555-0503', 'GeoPacific'),

-- Senior Staff
('Dr. William', 'Thompson', 'william.thompson@geopacific.test.com', '555-0601', 'GeoPacific'),
('Karen', 'Garcia', 'karen.garcia@geopacific.test.com', '555-0602', 'GeoPacific');

-- Display the inserted records
SELECT 
    "Id",
    "FirstName", 
    "LastName", 
    "Email", 
    "PhoneNumber", 
    "Company"
FROM "PersonalInfos" 
WHERE "Company" = 'GeoPacific' 
ORDER BY "Id";

-- Show count of GeoPacific employees
SELECT COUNT(*) as "GeoPacific_Employee_Count" 
FROM "PersonalInfos" 
WHERE "Company" = 'GeoPacific';

-- Useful queries for testing:

-- Get employee IDs for testing (first 3)
SELECT 
    "Id",
    "FirstName" || ' ' || "LastName" as "FullName",
    'Employee ID: ' || "Id" as "ForTesting"
FROM "PersonalInfos" 
WHERE "Company" = 'GeoPacific' 
ORDER BY "Id" 
LIMIT 3;

-- Get reviewer IDs (senior staff)
SELECT 
    "Id",
    "FirstName" || ' ' || "LastName" as "FullName",
    'Reviewer ID: ' || "Id" as "ForTesting"
FROM "PersonalInfos" 
WHERE "Company" = 'GeoPacific' 
  AND ("FirstName" LIKE '%David%' OR "FirstName" LIKE '%Jennifer%' OR "FirstName" LIKE '%Patricia%' OR "FirstName" LIKE '%Dr.%')
ORDER BY "Id";

COMMIT;
