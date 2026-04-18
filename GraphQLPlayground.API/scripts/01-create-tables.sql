-- ====================================
-- Criação das Tabelas
-- ====================================

-- Tabela Categories
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL
);

-- Tabela Products
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "CategoryId" INTEGER NOT NULL,
    CONSTRAINT "FK_Products_Categories" 
        FOREIGN KEY ("CategoryId") 
        REFERENCES "Categories"("Id") 
        ON DELETE CASCADE
);

-- Índices para melhorar performance
CREATE INDEX IF NOT EXISTS "IX_Products_CategoryId" ON "Products"("CategoryId");
CREATE INDEX IF NOT EXISTS "IX_Products_Name" ON "Products"("Name");
CREATE INDEX IF NOT EXISTS "IX_Categories_Name" ON "Categories"("Name");
