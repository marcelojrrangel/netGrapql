# 📚 Exemplos de Consultas GraphQL

Este documento contém exemplos práticos de queries e mutations para a API GraphQL.

Acesse: `http://localhost:<porta>/graphql` para testar no **Banana Cake Pop**.

---

## 🔍 Queries (Consultas)

### 1. Listar Todos os Produtos
Busca todos os produtos com suas informações básicas:

```graphql
query GetAllProducts {
  products {
    id
    name
    price
    category {
      id
      name
    }
  }
}
```

### 2. Listar Produtos de uma Categoria Específica
Filtra produtos por categoria usando o nome:

```graphql
query GetElectronics {
  products(where: { category: { name: { eq: "Electronics" } } }) {
    id
    name
    price
  }
}
```

### 3. Buscar Produtos por Nome
Busca produtos que contenham uma palavra específica no nome:

```graphql
query SearchProducts {
  products(where: { name: { contains: "Laptop" } }) {
    id
    name
    price
    category {
      name
    }
  }
}
```

### 4. Filtrar Produtos por Faixa de Preço
Retorna produtos entre R$ 50 e R$ 200:

```graphql
query ProductsByPriceRange {
  products(
    where: { 
      and: [
        { price: { gte: 50 } }
        { price: { lte: 200 } }
      ]
    }
  ) {
    id
    name
    price
  }
}
```

### 5. Ordenar Produtos por Preço
Lista produtos ordenados do mais caro para o mais barato:

```graphql
query ProductsSortedByPrice {
  products(order: { price: DESC }) {
    id
    name
    price
  }
}
```

### 6. Listar Categorias com Seus Produtos
Retorna todas as categorias e seus produtos associados:

```graphql
query CategoriesWithProducts {
  categories {
    id
    name
    products {
      id
      name
      price
    }
  }
}
```

### 7. Contar Produtos por Categoria
Lista categorias com contagem de produtos (usando projeção):

```graphql
query CategoryProductCount {
  categories {
    id
    name
    products {
      id
    }
  }
}
```

### 8. Buscar Produto por ID
Retorna um produto específico pelo ID:

```graphql
query GetProductById {
  products(where: { id: { eq: 1 } }) {
    id
    name
    price
    category {
      name
    }
  }
}
```

### 9. Produtos Mais Caros (Top 5)
Lista os 5 produtos mais caros:

```graphql
query TopExpensiveProducts {
  products(
    order: { price: DESC }
    take: 5
  ) {
    id
    name
    price
    category {
      name
    }
  }
}
```

### 10. Busca Combinada (Nome e Categoria)
Busca produtos com "Watch" no nome da categoria "Electronics":

```graphql
query SpecificSearch {
  products(
    where: {
      and: [
        { name: { contains: "Watch" } }
        { category: { name: { eq: "Electronics" } } }
      ]
    }
  ) {
    id
    name
    price
  }
}
```

---

## ✏️ Mutations (Modificações)

### 1. Criar Nova Categoria

```graphql
mutation CreateCategory {
  addCategory(name: "Food & Beverages") {
    id
    name
  }
}
```

### 2. Criar Novo Produto

```graphql
mutation CreateProduct {
  addProduct(
    name: "Espresso Machine Deluxe"
    price: 499.99
    categoryId: 4
  ) {
    id
    name
    price
    category {
      name
    }
  }
}
```

### 3. Criar Múltiplos Produtos
Você pode executar múltiplas mutations de uma vez:

```graphql
mutation CreateMultipleProducts {
  product1: addProduct(
    name: "Wireless Mouse"
    price: 29.99
    categoryId: 1
  ) {
    id
    name
  }

  product2: addProduct(
    name: "Mechanical Keyboard"
    price: 89.99
    categoryId: 1
  ) {
    id
    name
  }
}
```

### 4. Atualizar Nome e Preço de um Produto

```graphql
mutation UpdateProduct {
  updateProduct(
    id: 1
    name: "Smartphone Pro Max"
    price: 1899.90
  ) {
    id
    name
    price
  }
}
```

### 5. Trocar Categoria de um Produto

```graphql
mutation UpdateProductCategory {
  updateProductCategory(productId: 1, newCategoryId: 2) {
    id
    name
    category {
      id
      name
    }
  }
}
```

### 6. Aplicar Desconto em Lote

```graphql
mutation ApplyDiscount {
  applyDiscount(productIds: [1, 2, 3], discountPercentage: 10) {
    id
    name
    price
  }
}
```

### 7. Excluir Produto

```graphql
mutation DeleteProduct {
  deleteProduct(id: 3)
}
```

---

## 🎯 Queries Avançadas

### Paginação
Lista produtos com paginação (primeiros 10):

```graphql
query PaginatedProducts {
  products(take: 10, skip: 0) {
    id
    name
    price
  }
}
```

### Campos Seletivos (Performance)
Retorna apenas os campos necessários:

```graphql
query MinimalProductInfo {
  products {
    name
    price
  }
}
```

### Filtros Complexos
Produtos de "Books" ou "Electronics" com preço > R$ 100:

```graphql
query ComplexFilter {
  products(
    where: {
      and: [
        { price: { gt: 100 } }
        {
          or: [
            { category: { name: { eq: "Books" } } }
            { category: { name: { eq: "Electronics" } } }
          ]
        }
      ]
    }
  ) {
    id
    name
    price
    category {
      name
    }
  }
}
```

---

## 🚀 Como Executar os Scripts SQL

### Opção 1: Via Docker Exec
```bash
# Copiar scripts para o container
docker cp scripts/01-create-tables.sql graphql_pg_db:/tmp/
docker cp scripts/02-seed-data.sql graphql_pg_db:/tmp/

# Executar scripts
docker exec -it graphql_pg_db psql -U user -d graphql_db -f /tmp/01-create-tables.sql
docker exec -it graphql_pg_db psql -U user -d graphql_db -f /tmp/02-seed-data.sql
```

### Opção 2: Via Cliente PostgreSQL
```bash
psql -h localhost -U user -d graphql_db -f scripts/01-create-tables.sql
psql -h localhost -U user -d graphql_db -f scripts/02-seed-data.sql
```

### Opção 3: Via Entity Framework (Recomendado)
Descomente a linha no `Program.cs`:
```csharp
db.Database.EnsureCreated();
```
E execute a aplicação. O EF criará as tabelas e executará o seed automaticamente.

---

## 📊 Verificar Dados

Depois de executar os scripts, verifique os dados:

```graphql
query VerifyData {
  categories {
    id
    name
    products {
      id
    }
  }
}
```

Você deve ver 8 categorias com 37 produtos distribuídos entre elas.
