-- SQL script to seed people data
-- This script adds sample roles, employees, and contractors

-- First, add some roles
INSERT INTO "Roles" ("RoleTitle") VALUES 
('Project Manager'),
('Lab Technician'),
('Field Engineer'),
('Administrator'),
('Director');

-- Add some PersonalInfo records for contractors
INSERT INTO "PersonalInfos" ("FirstName", "LastName", "Email", "PhoneNumber", "Company") VALUES 
('Robert', 'Taylor', 'robert.taylor@construction.com', '555-0201', 'ABC Construction'),
('Jennifer', 'Anderson', 'jennifer.anderson@builders.com', '555-0202', 'Metro Builders'),
('Chris', 'Martinez', 'chris.martinez@contractors.com', '555-0203', 'City Contractors'),
('Amanda', 'Garcia', 'amanda.garcia@engineering.com', '555-0204', 'Engineering Solutions'),
('Michael', 'Thompson', 'michael.thompson@builders.com', '555-0205', 'Thompson Construction');

-- Add some PersonalInfo records for employees (without Company)
INSERT INTO "PersonalInfos" ("FirstName", "LastName", "Email", "PhoneNumber", "Company") VALUES 
('John', 'Smith', 'john.smith@geopacific.com', '555-0101', NULL),
('Sarah', 'Johnson', 'sarah.johnson@geopacific.com', '555-0102', NULL),
('Mike', 'Davis', 'mike.davis@geopacific.com', '555-0103', NULL),
('Lisa', 'Wilson', 'lisa.wilson@geopacific.com', '555-0104', NULL),
('David', 'Brown', 'david.brown@geopacific.com', '555-0105', NULL);

-- Add GeoPacificEmployee records (linking to PersonalInfo and Role)
INSERT INTO "GeoPacificEmployees" ("PersonalInfoId", "RoleId", "Password") VALUES 
(6, 1, 'password123'),  -- John Smith - Project Manager
(7, 2, 'password123'),  -- Sarah Johnson - Lab Technician
(8, 1, 'password123'),  -- Mike Davis - Project Manager
(9, 3, 'password123'),  -- Lisa Wilson - Field Engineer
(10, 4, 'password123'); -- David Brown - Administrator
