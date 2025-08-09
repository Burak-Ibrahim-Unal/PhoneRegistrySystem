-- Örnek seed data (isteğe bağlı)

-- Örnek kişiler
INSERT INTO "Persons" ("Id", "FirstName", "LastName", "Company", "CreatedAt", "UpdatedAt") 
VALUES 
    ('550e8400-e29b-41d4-a716-446655440001', 'Ahmet', 'Yılmaz', 'ABC Teknoloji', NOW(), NOW()),
    ('550e8400-e29b-41d4-a716-446655440002', 'Ayşe', 'Kaya', 'XYZ Şirketi', NOW(), NOW()),
    ('550e8400-e29b-41d4-a716-446655440003', 'Mehmet', 'Demir', 'DEF Holding', NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Örnek iletişim bilgileri
INSERT INTO "ContactInfos" ("Id", "PersonId", "Type", "Content", "CreatedAt") 
VALUES 
    ('660e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', 1, '+905551234567', NOW()),
    ('660e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', 2, 'ahmet@abc.com', NOW()),
    ('660e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440001', 3, 'İstanbul', NOW()),
    ('660e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440002', 1, '+905559876543', NOW()),
    ('660e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440002', 3, 'Ankara', NOW()),
    ('660e8400-e29b-41d4-a716-446655440006', '550e8400-e29b-41d4-a716-446655440003', 3, 'İzmir', NOW())
ON CONFLICT ("Id") DO NOTHING;
