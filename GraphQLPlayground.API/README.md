# 🚀 ASP.NET 9 + GraphQL (HotChocolate) + Docker

Projeto de exemplo demonstrando como implementar uma API **GraphQL** moderna usando **ASP.NET Core 9** e a biblioteca **HotChocolate**. Este projeto utiliza **PostgreSQL** rodando em Docker para persistência.

---

## 🏗️ Estrutura do Projeto

*   **GraphQLPlayground.API**: A api principal com os tipos, queries e mutations.
*   **Services**: Camada de regras de negocio usada por `Query` e `Mutation`.
*   **Docker (infra)**: PostgreSQL configurado no `docker-compose.yml`.

---

## 🚦 Pré-requisitos

Certifique-se de ter instalado:
1.  [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2.  [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## 🏃 Como Rodar

### 1. Iniciar a Infraestrutura (Banco de Dados)
Como a aplicação rodará fora do Docker, iniciamos apenas o PostgreSQL nos containers:

```bash
docker-compose up -d
```

### 2. Rodar a Aplicação
No diretório `GraphQLPlayground.API`, execute:

```bash
cd GraphQLPlayground.API
dotnet run
```

### 3. Observabilidade (Grafana + Loki + Promtail)

Com o `docker-compose` deste projeto, o Grafana sobe em `http://localhost:3000`.

Credenciais padrao (dev):

- Usuario: `admin`
- Senha: `admin`

Provisionamento automatico ja configurado:

- Data source `Loki` (uid: `loki`)
- Dashboard `GraphQL Caller x Receiver Observability`
- Dashboard `GraphQL Failures Observability`

Arquivos:

- `docker/grafana/provisioning/datasources/loki.yml`
- `docker/grafana/provisioning/dashboards/dashboards.yml`
- `docker/grafana/dashboards/graphql-observability.json`
- `docker/grafana/dashboards/graphql-failures-observability.json`

Se preferir adicionar manualmente pela interface do Grafana:

1. **Connections** -> **Data sources** -> **Add data source**
2. Escolha **Loki**
3. URL: `http://loki:3100`
4. **Save & test**

> Se voce abrir Loki direto no navegador em `http://localhost:3100/` e ver `404`, isso e normal. A raiz nao tem UI.

Consultas uteis no **Explore**:

```logql
{job="graphql_console"}
```

```logql
{job="graphql_api"}
```

```logql
{app=~"api|console"} |= "ERRO"
```

```logql
{role="caller"} |= "INICIO:"
```

```logql
{role="receptor"}
```

Consultas focadas em falhas GraphQL:

```logql
{app=~"api|console"} |= "Unexpected Execution Error"
```

```logql
{app=~"api|console"} |~ "ERRO|Exception|GraphQL"
```

```logql
sum by (app) (count_over_time({app=~"api|console"} |~ "ERRO|Exception|Unexpected Execution Error" [$__interval]))
```

---

## 🔍 O que é GraphQL?

Diferente do REST, onde você tem múltiplos endpoints para cada recurso (`/api/products`, `/api/categories`), no **GraphQL** você tem um único endpoint (geralmente `/graphql`).

*   **Tipagem Forte (Schema):** Tudo no GraphQL é tipado. O cliente sabe exatamente o que pode pedir.
*   **Queries (Busca):** O cliente decide quais campos quer receber. Se pedir apenas o `name`, o servidor não enviará o `price`.
*   **Mutations (Escrita):** Usado para alterar dados no servidor.

---

## 🛠️ Exemplos de Uso

Acesse `http://localhost:<porta>/graphql` para abrir o **Banana Cake Pop** (o explorador visual do HotChocolate).

### Exemplo de Query (Listar Produtos)

```graphql
query GetProducts {
  products {
    id
    name
    price
    category {
      name
    }
  }
}
```

### Exemplo de Filtro (Filtrar por Nome)

```graphql
query FilterSpecific {
  products(where: { name: { contains: "Smartphone" } }) {
    name
    price
  }
}
```

### Exemplo de Filtro por Preço

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

### Exemplo de Mutation (Criar Categoria)

```graphql
mutation CreateNewCategory {
  addCategory(name: "Home & Garden") {
    id
    name
  }
}
```

### Exemplo de Mutation (Criar Produto)

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

### Exemplo de Mutation (Atualizar Produto)

```graphql
mutation UpdateProduct {
  updateProduct(id: 1, name: "Smartphone Pro", price: 1299.99) {
    id
    name
    price
  }
}
```

### Exemplo de Mutation (Desconto em Lote)

```graphql
mutation BatchDiscount {
  applyDiscount(productIds: [1, 2], discountPercentage: 5) {
    id
    name
    price
  }
}
```

📚 **Mais exemplos?** Veja o arquivo `docs/GRAPHQL-EXAMPLES.md` com 10+ exemplos de queries e mutations.

---

## 💡 Comentários Técnicos

*   **`UseProjection`**: Garante que o Entity Framework gere um SQL que selecione apenas as colunas solicitadas pelo cliente GraphQL, economizando banda e memória.
*   **`RegisterDbContextFactory`**: O HotChocolate gerencia o ciclo de vida do DbContext para execucao paralela com `PooledDbContextFactory`.
*   **Camada de `Services`**: Centraliza validacoes e regras de negocio para manter resolvers GraphQL enxutos.

---

## 🛡️ Contato
Projeto de exemplo criado pelo **Antigravity AI Assistant**.
