-- =====================================================
-- SEED DATA FOR DENSITY REPORTING TOOL BACKEND
-- =====================================================
-- This script populates all tables with realistic test data
-- Run this after your migrations to have data for testing

-- =====================================================
-- CLIENTS (Companies that hire GeoPacific)
-- =====================================================
INSERT INTO "Clients" ("Id", "Name", "Address", "PhoneNumber", "Email") VALUES
(1, 'City Development Corp', '123 Main St, Seattle, WA 98101', '+1-206-555-0100', 'info@citydev.com'),
(2, 'Metro Construction', '456 Oak Ave, Portland, OR 97201', '+1-503-555-0200', 'contact@metroconstruction.com'),
(3, 'Pacific Infrastructure', '789 Pine St, San Francisco, CA 94102', '+1-415-555-0300', 'projects@pacificinfra.com'),
(4, 'Northwest Builders', '321 Cedar Blvd, Vancouver, WA 98660', '+1-360-555-0400', 'info@northwestbuilders.com'),
(5, 'Emerald City Projects', '654 Spruce Way, Bellevue, WA 98004', '+1-425-555-0500', 'admin@emeraldcity.com');

-- =====================================================
-- PERSONAL INFO (Base table for all people)
-- =====================================================
INSERT INTO "PersonalInfos" ("Id", "FirstName", "LastName", "Email", "PhoneNumber") VALUES
-- GeoPacific Employees (1-10)
(1, 'John', 'Smith', 'john.smith@geopacific.com', '+1-555-0101'),
(2, 'Sarah', 'Johnson', 'sarah.johnson@geopacific.com', '+1-555-0102'),
(3, 'Michael', 'Davis', 'michael.davis@geopacific.com', '+1-555-0103'),
(4, 'Emily', 'Wilson', 'emily.wilson@geopacific.com', '+1-555-0104'),
(5, 'David', 'Brown', 'david.brown@geopacific.com', '+1-555-0105'),
(6, 'Lisa', 'Garcia', 'lisa.garcia@geopacific.com', '+1-555-0106'),
(7, 'Robert', 'Martinez', 'robert.martinez@geopacific.com', '+1-555-0107'),
(8, 'Jennifer', 'Anderson', 'jennifer.anderson@geopacific.com', '+1-555-0108'),
(9, 'William', 'Taylor', 'william.taylor@geopacific.com', '+1-555-0109'),
(10, 'Amanda', 'Thomas', 'amanda.thomas@geopacific.com', '+1-555-0110'),

-- Contractors (11-20)
(11, 'James', 'Hernandez', 'james.hernandez@abcconstruction.com', '+1-555-0201'),
(12, 'Maria', 'Lopez', 'maria.lopez@abcconstruction.com', '+1-555-0202'),
(13, 'Christopher', 'Gonzalez', 'chris.gonzalez@xyzbuilders.com', '+1-555-0203'),
(14, 'Jessica', 'Perez', 'jessica.perez@xyzbuilders.com', '+1-555-0204'),
(15, 'Daniel', 'Sanchez', 'daniel.sanchez@qualitycontractors.com', '+1-555-0205'),
(16, 'Michelle', 'Ramirez', 'michelle.ramirez@qualitycontractors.com', '+1-555-0206'),
(17, 'Kevin', 'Torres', 'kevin.torres@fieldworksllc.com', '+1-555-0207'),
(18, 'Stephanie', 'Flores', 'stephanie.flores@primecontractors.com', '+1-555-0208'),

-- Site Contacts (21-25)
(21, 'Brian', 'Rivera', 'brian.rivera@citydev.com', '+1-206-555-0301'),
(22, 'Nicole', 'Cooper', 'nicole.cooper@metroconstruction.com', '+1-503-555-0302'),
(23, 'Ryan', 'Murphy', 'ryan.murphy@pacificinfra.com', '+1-415-555-0303'),
(24, 'Lauren', 'Scott', 'lauren.scott@northwestbuilders.com', '+1-360-555-0304'),
(25, 'Tyler', 'Reed', 'tyler.reed@emeraldcity.com', '+1-425-555-0305');

-- =====================================================
-- ROLES
-- =====================================================
INSERT INTO "Roles" ("Id", "RoleTitle") VALUES
(1, 'Admin'),
(2, 'Field Tech'),
(3, 'Engineer'),
(4, 'Project Manager'),
(5, 'Reviewer');

-- =====================================================
-- GEOPACIFIC EMPLOYEES
-- =====================================================
INSERT INTO "GeoPacificEmployees" ("Id", "PersonalInfoId", "RoleId", "Password") VALUES
(1, 1, 1, 'hashedpassword1'),   -- John Smith - Admin
(2, 2, 4, 'hashedpassword2'),   -- Sarah Johnson - Project Manager
(3, 3, 2, 'hashedpassword3'),   -- Michael Davis - Field Tech
(4, 4, 3, 'hashedpassword4'),   -- Emily Wilson - Engineer
(5, 5, 4, 'hashedpassword5'),   -- David Brown - Project Manager
(6, 6, 3, 'hashedpassword6'),   -- Lisa Garcia - Engineer
(7, 7, 2, 'hashedpassword7'),   -- Robert Martinez - Field Tech
(8, 8, 5, 'hashedpassword8'),   -- Jennifer Anderson - Reviewer
(9, 9, 3, 'hashedpassword9'),   -- William Taylor - Engineer
(10, 10, 4, 'hashedpassword10'); -- Amanda Thomas - Project Manager

-- =====================================================
-- CONTRACTORS
-- =====================================================
INSERT INTO "Contractors" ("Id", "PersonalInfoId", "CompanyName", "ClientId") VALUES
(1, 11, 'ABC Construction', 1),           -- James Hernandez
(2, 12, 'ABC Construction', 1),           -- Maria Lopez
(3, 13, 'XYZ Builders', 2),              -- Christopher Gonzalez
(4, 14, 'XYZ Builders', 2),              -- Jessica Perez
(5, 15, 'Quality Contractors', 3),       -- Daniel Sanchez
(6, 16, 'Quality Contractors', 3),       -- Michelle Ramirez
(7, 17, 'FieldWorks LLC', NULL),         -- Kevin Torres (independent contractor)
(8, 18, 'Prime Contractors', 4);         -- Stephanie Flores

-- =====================================================
-- JOBS
-- =====================================================
INSERT INTO "Jobs" ("Id", "ClientId", "ProjectName", "SiteAddress", "StartDate", "EndDate") VALUES
(1, 1, 'Downtown Seattle Mixed-Use Development', '1200 3rd Ave, Seattle, WA 98101', '2024-01-15', NULL),
(2, 2, 'Portland Bridge Infrastructure Project', '800 SE Morrison Bridge, Portland, OR 97214', '2024-02-01', NULL),
(3, 3, 'San Francisco Bay Area Transit Expansion', '500 Mission St, San Francisco, CA 94105', '2024-01-20', '2024-08-15'),
(4, 4, 'Vancouver Waterfront Development', '101 E Columbia Way, Vancouver, WA 98660', '2024-03-01', NULL),
(5, 5, 'Bellevue Technology Campus', '1000 112th Ave NE, Bellevue, WA 98004', '2024-02-15', NULL);

-- =====================================================
-- JOB PROJECT MANAGERS
-- =====================================================
INSERT INTO "JobProjectManagers" ("Id", "JobId", "EmployeeId", "StartDate", "EndDate", "Notes", "IsActive", "CreatedDate") VALUES
(1, 1, 2, '2024-01-15', NULL, 'Primary contact for client', true, '2024-01-15 08:00:00+00'),
(2, 2, 5, '2024-02-01', NULL, 'Lead PM for infrastructure work', true, '2024-02-01 08:00:00+00'),
(3, 3, 10, '2024-01-20', '2024-08-15', 'Managed project completion', false, '2024-01-20 08:00:00+00'),
(4, 4, 2, '2024-03-01', NULL, 'Secondary assignment', true, '2024-03-01 08:00:00+00'),
(5, 5, 5, '2024-02-15', NULL, 'Tech campus specialist', true, '2024-02-15 08:00:00+00');

-- =====================================================
-- JOB CONTRACTORS
-- =====================================================
INSERT INTO "JobContractors" ("JobId", "ContractorId") VALUES
(1, 1), -- ABC Construction on Seattle project
(1, 2), -- ABC Construction (Maria) on Seattle project
(2, 3), -- XYZ Builders on Portland project
(2, 4), -- XYZ Builders (Jessica) on Portland project
(3, 5), -- Quality Contractors on SF project
(3, 6), -- Quality Contractors (Michelle) on SF project
(4, 7), -- FieldWorks LLC on Vancouver project
(5, 8); -- Prime Contractors on Bellevue project

-- =====================================================
-- JOB SITE CONTACTS
-- =====================================================
INSERT INTO "JobSiteContacts" ("Id", "JobId", "PersonalInfoId", "Area", "Company", "Role", "IsPrimary", "StartDate", "IsActive", "CreatedDate") VALUES
(1, 1, 21, 'Main Site', 'City Development Corp', 'Site Supervisor', true, '2024-01-15', true, '2024-01-15 08:00:00+00'),
(2, 2, 22, 'Bridge Section', 'Metro Construction', 'Project Coordinator', true, '2024-02-01', true, '2024-02-01 08:00:00+00'),
(3, 3, 23, 'Transit Hub', 'Pacific Infrastructure', 'Construction Manager', true, '2024-01-20', true, '2024-01-20 08:00:00+00'),
(4, 4, 24, 'Waterfront Area', 'Northwest Builders', 'Site Foreman', true, '2024-03-01', true, '2024-03-01 08:00:00+00'),
(5, 5, 25, 'Tech Campus', 'Emerald City Projects', 'Development Manager', true, '2024-02-15', true, '2024-02-15 08:00:00+00');

-- =====================================================
-- DISTRIBUTION LISTS
-- =====================================================
INSERT INTO "DistributionLists" ("Id", "JobId", "Name", "Description") VALUES
(1, 1, 'Seattle Project Team', 'All stakeholders for downtown Seattle development'),
(2, 2, 'Portland Bridge Team', 'Infrastructure project distribution'),
(3, 3, 'SF Transit Team', 'Bay area transit expansion stakeholders'),
(4, 4, 'Vancouver Team', 'Waterfront development contacts'),
(5, 5, 'Bellevue Tech Team', 'Technology campus project distribution');

-- =====================================================
-- DISTRIBUTION MEMBERS
-- =====================================================
INSERT INTO "DistributionMembers" ("Id", "DistributionListId", "PersonalInfoId") VALUES
-- Seattle Project (DL 1)
(1, 1, 2),   -- Sarah Johnson (PM)
(2, 1, 21),  -- Brian Rivera (Site Contact)
(3, 1, 11),  -- James Hernandez (Contractor)
(4, 1, 12),  -- Maria Lopez (Contractor)

-- Portland Bridge (DL 2)
(5, 2, 5),   -- David Brown (PM)
(6, 2, 22),  -- Nicole Cooper (Site Contact)
(7, 2, 13),  -- Christopher Gonzalez (Contractor)
(8, 2, 14),  -- Jessica Perez (Contractor)

-- SF Transit (DL 3)
(9, 3, 10),  -- Amanda Thomas (PM)
(10, 3, 23), -- Ryan Murphy (Site Contact)
(11, 3, 15), -- Daniel Sanchez (Contractor)
(12, 3, 16), -- Michelle Ramirez (Contractor)

-- Vancouver (DL 4)
(13, 4, 2),  -- Sarah Johnson (PM)
(14, 4, 24), -- Lauren Scott (Site Contact)
(15, 4, 17), -- Kevin Torres (Contractor)

-- Bellevue (DL 5)
(16, 5, 5),  -- David Brown (PM)
(17, 5, 25), -- Tyler Reed (Site Contact)
(18, 5, 18); -- Stephanie Flores (Contractor)

-- =====================================================
-- REPORTS
-- =====================================================
INSERT INTO "Reports" ("Id", "JobId", "EmployeeId", "ReviewerId", "ReportNumber", "StartDate", "SubmitDate", "DistributionListId") VALUES
(1, 1, 4, 8, 2024001, '2024-01-15', '2024-01-20', 1),  -- Emily Wilson (Engineer), Jennifer Anderson (Reviewer)
(2, 2, 6, 8, 2024002, '2024-02-01', '2024-02-05', 2),  -- Lisa Garcia (Engineer), Jennifer Anderson (Reviewer)
(3, 3, 9, 8, 2024003, '2024-01-20', '2024-01-25', 3),  -- William Taylor (Engineer), Jennifer Anderson (Reviewer)
(4, 4, 4, 8, 2024004, '2024-03-01', '2024-03-05', 4),  -- Emily Wilson (Engineer), Jennifer Anderson (Reviewer)
(5, 5, 6, 8, 2024005, '2024-02-15', '2024-02-20', 5);  -- Lisa Garcia (Engineer), Jennifer Anderson (Reviewer)

-- =====================================================
-- PROCTOR TYPES
-- =====================================================
INSERT INTO "ProctorTypes" ("Id", "Type") VALUES
(1, 'Standard Proctor (ASTM D698)'),
(2, 'Modified Proctor (ASTM D1557)'),
(3, 'California Test 216'),
(4, 'AASHTO T99'),
(5, 'AASHTO T180');

-- =====================================================
-- LAB TESTS
-- =====================================================
INSERT INTO "LabTests" ("Id", "JobId", "MaterialType", "ImportLocation", "ReceiveDate") VALUES
(1, 1, 'Structural Fill', 'Pit A - Mile 15.5', '2024-01-18'),
(2, 1, 'Aggregate Base', 'Quarry B - North Site', '2024-01-22'),
(3, 2, 'Embankment Fill', 'Borrow Area C', '2024-02-03'),
(4, 3, 'Subgrade Soil', 'On-site Excavation', '2024-01-22'),
(5, 4, 'Structural Fill', 'Import Site D', '2024-03-03'),
(6, 5, 'Base Course', 'Regional Quarry', '2024-02-17');

-- =====================================================
-- PROCTORS
-- =====================================================
INSERT INTO "Proctors" ("Id", "ProctorID", "LabTestId", "ProctorTypeId", "MaxDensity", "OptimumMoistureContent") VALUES
(1, 'P-001-2024', 1, 2, 125.5, 8.2),
(2, 'P-002-2024', 2, 1, 118.3, 9.5),
(3, 'P-003-2024', 3, 2, 122.8, 7.8),
(4, 'P-004-2024', 4, 1, 115.7, 10.2),
(5, 'P-005-2024', 5, 2, 128.1, 7.5),
(6, 'P-006-2024', 6, 1, 120.9, 8.8);

-- =====================================================
-- COMMENTS
-- =====================================================
INSERT INTO "Comments" ("Id", "EmployeeId", "CreatedAt", "Details") VALUES
(1, 4, '2024-01-20 14:30:00+00', 'Initial density test results look good. Material meeting specifications.'),
(2, 6, '2024-02-05 11:15:00+00', 'Some areas showing lower compaction. Recommended additional passes.'),
(3, 9, '2024-01-25 16:45:00+00', 'Weather conditions affecting compaction. Monitoring moisture content.'),
(4, 4, '2024-03-05 09:20:00+00', 'Excellent compaction achieved on waterfront fill material.'),
(5, 6, '2024-02-20 13:10:00+00', 'Tech campus foundation prep completed successfully.');

-- =====================================================
-- DENSITY TESTS
-- =====================================================
INSERT INTO "DensityTests" ("Id", "ProctorId", "ReportId", "TestArea", "Location", "ElevationValue", "ProbeDepth", "CompactionSpecification", "DensityValue", "MoistureValue", "CreatedDate") VALUES
(1, 1, 1, 'Area A', 'Station 1+00', 105.5, 12, 95.0, 119.2, 8.5, '2024-01-19 10:30:00+00'),
(2, 1, 1, 'Area A', 'Station 2+00', 105.8, 12, 95.0, 121.1, 8.3, '2024-01-19 14:15:00+00'),
(3, 2, 2, 'Area B', 'Grid B-1', 98.2, 8, 90.0, 108.5, 9.2, '2024-01-23 09:45:00+00'),
(4, 3, 3, 'Embankment', 'Section C', 112.3, 15, 95.0, 117.9, 7.9, '2024-02-04 11:20:00+00'),
(5, 4, 4, 'Subgrade', 'Foundation Area', 89.5, 6, 85.0, 99.2, 10.5, '2024-01-23 15:30:00+00'),
(6, 5, 5, 'Fill Area', 'Northwest Corner', 118.7, 18, 95.0, 122.3, 7.2, '2024-03-04 08:45:00+00'),
(7, 6, 5, 'Base Course', 'Parking Area', 102.1, 10, 92.0, 112.4, 8.9, '2024-02-18 13:25:00+00');

-- =====================================================
-- DENSITY TEST COMMENTS
-- =====================================================
INSERT INTO "DensityTestComments" ("Id", "CommentId", "DensityTestId") VALUES
(1, 1, 1),
(2, 1, 2),
(3, 2, 3),
(4, 3, 4),
(5, 3, 5),
(6, 4, 6),
(7, 5, 7);

-- =====================================================
-- REPORT MEMOS
-- =====================================================
INSERT INTO "ReportMemos" ("Id", "ReportId", "Purpose", "CommentsAndObservations", "Conclusion", "CreatedDate") VALUES
(1, 1, 'Field density testing for structural fill placement', 'Testing performed in accordance with ASTM D6938. Weather conditions were favorable with temperature around 68°F.', 'All tested areas met or exceeded the required 95% compaction specification.', '2024-01-20 16:00:00+00'),
(2, 2, 'Quality control testing for aggregate base course', 'Multiple lifts tested during placement. Some areas required additional compaction effort.', 'Overall compaction levels acceptable with minor areas requiring rework.', '2024-02-05 17:30:00+00'),
(3, 3, 'Embankment construction monitoring', 'Continuous monitoring during placement. Weather impacts noted on Day 3.', 'Embankment construction proceeding according to specifications.', '2024-01-25 15:45:00+00'),
(4, 4, 'Foundation preparation verification', 'Subgrade preparation completed prior to structural fill placement.', 'Subgrade meets design requirements for planned loading.', '2024-03-05 14:20:00+00'),
(5, 5, 'Technology campus foundation work', 'Specialized requirements for technology infrastructure foundations.', 'All foundation elements meeting enhanced specifications for technology applications.', '2024-02-20 16:15:00+00');

-- =====================================================
-- MEMO COMMENTS
-- =====================================================
INSERT INTO "MemoComments" ("Id", "CommentId", "MemoId") VALUES
(1, 1, 1),
(2, 2, 2),
(3, 3, 3),
(4, 4, 4),
(5, 5, 5);

-- =====================================================
-- SITE PLANS
-- =====================================================
INSERT INTO "SitePlans" ("Id", "JobId", "SitePlanUrl") VALUES
(1, 1, 'https://storage.geopacific.com/plans/seattle-downtown-site-plan.pdf'),
(2, 2, 'https://storage.geopacific.com/plans/portland-bridge-layout.pdf'),
(3, 3, 'https://storage.geopacific.com/plans/sf-transit-expansion.pdf'),
(4, 4, 'https://storage.geopacific.com/plans/vancouver-waterfront.pdf'),
(5, 5, 'https://storage.geopacific.com/plans/bellevue-tech-campus.pdf');

-- =====================================================
-- SHOT PLACEMENTS
-- =====================================================
INSERT INTO "ShotPlacements" ("Id", "SitePlanId", "DensityTestId", "XCoordinate", "YCoordinate") VALUES
(1, 1, 1, 150.5, 275.8),
(2, 1, 2, 225.3, 275.8),
(3, 2, 3, 89.2, 156.4),
(4, 3, 4, 312.7, 445.1),
(5, 4, 5, 198.6, 88.9),
(6, 5, 6, 267.4, 189.3),
(7, 5, 7, 156.8, 234.7);

-- =====================================================
-- REPORT PHOTOS
-- =====================================================
INSERT INTO "ReportPhotos" ("Id", "ReportId", "Code", "Url", "Description", "Latitude", "Longitude") VALUES
(1, 1, 'DT-001', 'https://storage.geopacific.com/photos/seattle-dt-001.jpg', 'Density test at Station 1+00, Area A', 47.6062, -122.3321),
(2, 1, 'DT-002', 'https://storage.geopacific.com/photos/seattle-dt-002.jpg', 'Density test at Station 2+00, Area A', 47.6065, -122.3318),
(3, 2, 'DT-003', 'https://storage.geopacific.com/photos/portland-dt-003.jpg', 'Base course compaction, Grid B-1', 45.5152, -122.6784),
(4, 3, 'DT-004', 'https://storage.geopacific.com/photos/sf-dt-004.jpg', 'Embankment section C overview', 37.7749, -122.4194),
(5, 4, 'DT-005', 'https://storage.geopacific.com/photos/vancouver-dt-005.jpg', 'Subgrade preparation complete', 45.6387, -122.6615),
(6, 5, 'DT-006', 'https://storage.geopacific.com/photos/bellevue-dt-006.jpg', 'Tech campus foundation fill', 47.6101, -122.2015);

-- =====================================================
-- Set sequences to continue from the last inserted ID
-- =====================================================
SELECT setval('"Clients_Id_seq"', 5, true);
SELECT setval('"PersonalInfos_Id_seq"', 25, true);
SELECT setval('"Roles_Id_seq"', 5, true);
SELECT setval('"GeoPacificEmployees_Id_seq"', 10, true);
SELECT setval('"Contractors_Id_seq"', 8, true);
SELECT setval('"Jobs_Id_seq"', 5, true);
SELECT setval('"JobProjectManagers_Id_seq"', 5, true);
SELECT setval('"JobSiteContacts_Id_seq"', 5, true);
SELECT setval('"DistributionLists_Id_seq"', 5, true);
SELECT setval('"DistributionMembers_Id_seq"', 18, true);
SELECT setval('"Reports_Id_seq"', 5, true);
SELECT setval('"ProctorTypes_Id_seq"', 5, true);
SELECT setval('"LabTests_Id_seq"', 6, true);
SELECT setval('"Proctors_Id_seq"', 6, true);
SELECT setval('"Comments_Id_seq"', 5, true);
SELECT setval('"DensityTests_Id_seq"', 7, true);
SELECT setval('"DensityTestComments_Id_seq"', 7, true);
SELECT setval('"ReportMemos_Id_seq"', 5, true);
SELECT setval('"MemoComments_Id_seq"', 5, true);
SELECT setval('"SitePlans_Id_seq"', 5, true);
SELECT setval('"ShotPlacements_Id_seq"', 7, true);
SELECT setval('"ReportPhotos_Id_seq"', 6, true);

-- =====================================================
-- END OF SEED DATA
-- =====================================================