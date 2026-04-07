# CoreBankingSystem

API do Sistema de Caixa de Banco, desenvolvida com C# .NET 10.

## Tecnologias

- **.NET 10** — framework principal
- **PostgreSQL 16** — banco de dados relacional
- **Entity Framework Core**
- **pgAdmin 4**
- **Docker / Docker Compose** 
- **Swagger / OpenAPI**
- **xUnit + Moq + FluentAssertions** — 

---

## Pré-requisitos

- [Docker](https://www.docker.com/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

---

## Executando com Docker

```bash
# Clone o repositório
git clone https://github.com/Mariana-SN/CoreBankingSystem.git
cd CoreBankingSystem

# Suba todos os containers (API + PostgreSQL + pgAdmin)
docker compose up -d --build
```
---

## Configure as credenciais do banco de dados conforme o .env.example

### API
ASPNETCORE_ENVIRONMENT=Development
API_PORT=5009

### PostgreSQL
POSTGRES_DB=corebanking
POSTGRES_USER=postgres
POSTGRES_PASSWORD=
POSTGRES_PORT=5439

### pgAdmin
PGADMIN_EMAIL=
PGADMIN_PASSWORD=
PGADMIN_PORT=5056

---

## Executando localmente 

```bash
# 1. Suba apenas o banco e o pgAdmin
docker compose up db pgadmin -d

# 2. Restaure os pacotes e aplique as migrations
dotnet restore
dotnet ef database update --project src/CoreBankingSystem.Infrastructure --startup-project src/CoreBankingSystem.API

# 3. Rode a API
dotnet run --project src/CoreBankingSystem.API
```

---

## Migrations (EF Core)

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration \
  --project src/CoreBankingSystem.Infrastructure \
  --startup-project src/CoreBankingSystem.API

# Aplicar ao banco
dotnet ef database update \
  --project src/CoreBankingSystem.Infrastructure \
  --startup-project src/CoreBankingSystem.API

# Desfazer última migration
dotnet ef migrations remove \
  --project src/CoreBankingSystem.Infrastructure \
  --startup-project src/CoreBankingSystem.API

# Voltar para uma migration específica
dotnet ef database update NomeDaMigration \
  --project src/CoreBankingSystem.Infrastructure \
  --startup-project src/CoreBankingSystem.API

# Listar migrations
dotnet ef migrations list \
  --project src/CoreBankingSystem.Infrastructure \
  --startup-project src/CoreBankingSystem.API
```

---

## Rodando os testes

```bash
dotnet test
```

Outra opção seria: vá em Teste > Executar Todos os Testes. Ou ainda abra o Text Explorer (Ctrl+E, T) e execute os testes desejads por lá.

---

## Comandos úteis

```bash
docker compose up --build        # sobre tudo com build
docker compose up -d             # sobe  tudo em background
docker compose down              # derruba todos os containers
docker compose down -v           # derruba e apaga os volumes

docker compose up db pgadmin -d  # sobe só o banco e o pgAdmin
docker compose up --build api    # rebuild só da API
docker compose restart api       # reinicia só a API sem rebuild

```


