-- ====================================
-- Carga de Dados de Teste
-- ====================================

-- Limpar dados existentes
TRUNCATE TABLE "Products" CASCADE;
TRUNCATE TABLE "Categories" RESTART IDENTITY CASCADE;

-- Inserir Categorias
INSERT INTO "Categories" ("Id", "Name") VALUES
(1, 'Electronics'),
(2, 'Books'),
(3, 'Clothing'),
(4, 'Home & Garden'),
(5, 'Sports & Outdoors'),
(6, 'Toys & Games'),
(7, 'Health & Beauty'),
(8, 'Automotive');

-- Inserir Produtos
INSERT INTO "Products" ("Id", "Name", "Price", "CategoryId") VALUES
-- Electronics
(1, 'Smartphone Samsung Galaxy S24', 999.99, 1),
(2, 'Laptop Dell XPS 15', 1500.00, 1),
(3, 'Wireless Headphones Sony WH-1000XM5', 349.99, 1),
(4, 'Smart TV LG 55" OLED', 1299.00, 1),
(5, 'iPad Pro 12.9"', 1099.00, 1),
(6, 'Apple Watch Series 9', 399.00, 1),
(7, 'Gaming Console PlayStation 5', 499.99, 1),

-- Books
(8, 'Clean Code - Robert C. Martin', 45.99, 2),
(9, 'Design Patterns - Gang of Four', 54.99, 2),
(10, 'The Pragmatic Programmer', 42.50, 2),
(11, 'Domain-Driven Design - Eric Evans', 59.99, 2),
(12, 'Refactoring - Martin Fowler', 48.00, 2),

-- Clothing
(13, 'Nike Air Max Sneakers', 129.99, 3),
(14, 'Levi''s 501 Original Jeans', 89.99, 3),
(15, 'Adidas Performance T-Shirt', 29.99, 3),
(16, 'North Face Jacket', 199.00, 3),
(17, 'Ray-Ban Aviator Sunglasses', 154.00, 3),

-- Home & Garden
(18, 'Dyson V15 Vacuum Cleaner', 649.99, 4),
(19, 'KitchenAid Stand Mixer', 379.99, 4),
(20, 'Nespresso Coffee Machine', 189.00, 4),
(21, 'Garden Tool Set - 10 pieces', 79.99, 4),

-- Sports & Outdoors
(22, 'Mountain Bike Trek X-Caliber', 899.00, 5),
(23, 'Camping Tent - 4 Person', 249.99, 5),
(24, 'Yoga Mat Premium', 39.99, 5),
(25, 'Dumbbells Set 20kg', 89.99, 5),

-- Toys & Games
(26, 'LEGO Star Wars Millennium Falcon', 159.99, 6),
(27, 'Nintendo Switch OLED', 349.99, 6),
(28, 'Barbie Dreamhouse', 199.99, 6),
(29, 'Hot Wheels Track Set', 49.99, 6),

-- Health & Beauty
(30, 'Oral-B Electric Toothbrush', 89.99, 7),
(31, 'Philips Hair Dryer', 59.99, 7),
(32, 'Vitamin C Serum', 24.99, 7),
(33, 'Fitness Tracker Fitbit Charge 6', 149.99, 7),

-- Automotive
(34, 'Michelin Tires Set of 4', 599.99, 8),
(35, 'Car Dash Cam 4K', 129.99, 8),
(36, 'Bluetooth Car Kit', 39.99, 8),
(37, 'Car Vacuum Cleaner', 49.99, 8);

-- Resetar as sequências
SELECT setval('"Categories_Id_seq"', (SELECT MAX("Id") FROM "Categories"));
SELECT setval('"Products_Id_seq"', (SELECT MAX("Id") FROM "Products"));
