-- Seed ProctorTypes for testing
-- Insert the two proctor types: SPDD and MPDD only

INSERT INTO "ProctorTypes" ("Type") VALUES 
('SPDD'),
('MPDD')
ON CONFLICT ("Type") DO NOTHING;

-- Verify the data was inserted
SELECT * FROM "ProctorTypes";
