Diagrama de blocos:

<img width="1575" height="1198" alt="image" src="https://github.com/user-attachments/assets/4df30b69-f7be-4385-9a54-5fada629e1fe" />

Arquitetura de soluções:

<img width="1046" height="510" alt="image" src="https://github.com/user-attachments/assets/bb7ff5f6-7eb2-44af-a000-c40d1aa0248d" />


MonitorEconomic

API para consultar e armazenar dados do Bacen usando .NET 8, PostgreSQL e Docker.

## 🏗️ Arquitetura

- **Clean Architecture** com separação clara de responsabilidades
- **CQRS** com MediatR para commands e queries
- **Repository Pattern** para acesso a dados
- **AutoMapper** para mapeamento Domain → DTO
- **PostgreSQL** como banco de dados
- **Docker** para containerização

## 📋 Pré-requisitos

- Docker e Docker Compose instalados
- .NET SDK 8.0

## 🚀 Como executar

### 1. Clonagem e configuração

```bash
git clone <repository-url>
cd .NET
```

### 2. Arquivo de ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
POSTGRES_DB=monitor_economic
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123

ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=monitor_economic;Username=postgres;Password=postgres123
ASPNETCORE_ENVIRONMENT=Development
```

### 3. Executar com Docker Compose

```bash
docker compose up --build -d
```

A API estará acessível em: **http://localhost:8080**

O PostgreSQL estará rodando na porta **5432**.

> 💡 **Nota**: este projeto usa acesso direto com Npgsql. A estrutura da tabela `ipc` deve existir no banco conforme a seção de banco de dados abaixo.

## 📡 API Endpoints

### 🔍 Buscar dados da API externa

**GET** `/api/bacen`

Busca dados do Bacen para um período específico.

**Parâmetros de Query:**
- `serie` (enum, obrigatório): Série do Bacen a ser consultada. Exemplo atual: `Ipc`
- `dataInicial` (string): Data inicial no formato `dd/MM/yyyy`
- `dataFinal` (string): Data final no formato `dd/MM/yyyy`

**Exemplo:**
```bash
curl "http://localhost:8080/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/03/2024"
```

**Resposta de sucesso (200):**
```json
[
  {
    "data": "2024-01-01",
    "valor": "0.65"
  },
  {
    "data": "2024-02-01",
    "valor": "0.72"
  }
]
```

### 💾 Salvar dados no banco

**POST** `/api/bacen/store`

Busca dados da API externa e salva no banco de dados PostgreSQL.

Retorna os registros persistidos como entidades de domínio, incluindo o `id` gerado para cada item salvo.

**Parâmetros de Query:**
- `serie` (enum, obrigatório): Série do Bacen a ser consultada. Exemplo atual: `Ipc`
- `dataInicial` (string): Data inicial no formato `dd/MM/yyyy`
- `dataFinal` (string): Data final no formato `dd/MM/yyyy`

**Exemplo:**
```bash
curl -X POST "http://localhost:8080/api/bacen/store?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/03/2024"
```

**Resposta de sucesso (200):**
```json
[
  {
    "id": "2f55b7ce-8e13-4eb7-9f0c-7a8fbfc8a8e1",
    "data": "2024-01-01T00:00:00",
    "valor": 0.65
  }
]
```

### 📊 Consultar dados salvos no banco

**GET** `/api/bacen/db`

Retorna todos os registros do Bacen salvos no banco de dados.

**Exemplo:**
```bash
curl "http://localhost:8080/api/bacen/db"
```

**Resposta de sucesso (200):**
```json
[
  {
    "data": "2024-01-01",
    "valor": "0.65"
  },
  {
    "data": "2024-02-01",
    "valor": "0.72"
  }
]
```

**Resposta quando não há dados (404):**
```json
"Nenhum registro do Bacen encontrado no banco."
```

## 🗄️ Banco de Dados

### Acessar o PostgreSQL

```bash
# Entrar no container do banco
docker exec -it monitor_economic_db psql -U postgres -d monitor_economic

# Listar todas as tabelas
\dt

# Ver estrutura da tabela ipc
\d ipc

# Consultar todos os registros
SELECT * FROM ipc ORDER BY "Data" DESC;

# Sair do PostgreSQL
\q
```

### Estrutura da tabela `ipc`

```sql
CREATE TABLE ipc (
    "Id" UUID PRIMARY KEY,
    "Data" DATE NOT NULL,
    "Valor" DECIMAL(10,4) NOT NULL
);
```

## 🧪 Testando a API

### Usando cURL

```bash
# 1. Buscar dados da API externa
curl "http://localhost:8080/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024"

# 2. Salvar dados no banco
curl -X POST "http://localhost:8080/api/bacen/store?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024"

# 3. Consultar dados salvos
curl "http://localhost:8080/api/bacen/db"
```

### Usando Postman/Insomnia

1. **GET** `http://localhost:8080/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024`
2. **POST** `http://localhost:8080/api/bacen/store?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024`
3. **GET** `http://localhost:8080/api/bacen/db`

## 🏛️ Estrutura do Projeto

```
MonitorEconomic/
├── MonitorEconomic.WebUi/          # Camada de Apresentação (ASP.NET Core Web API)
├── MonitorEconomic.Application/    # Camada de Aplicação (CQRS, MediatR)
├── MonitorEconomic.Domain/         # Camada de Domínio (Entities, contratos, exceções)
├── MonitorEconomic.Infra.Data/     # Camada de Infraestrutura (Repository, Context, integração Bacen)
├── MonitorEconomic.Ioc/           # Injeção de Dependência
└── docker-compose.yml             # Configuração Docker
```

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **MediatR** - CQRS Pattern
- **AutoMapper** - Mapeamento de objetos
- **PostgreSQL** - Banco de dados
- **Npgsql** - Provider PostgreSQL para .NET
- **Docker** - Containerização
- **Docker Compose** - Orquestração de containers

## 📝 Scripts Úteis

```bash
# Ver logs dos containers
docker compose logs -f

# Parar os containers
docker compose down

# Reconstruir e executar
docker compose up --build

# Ver status dos containers
docker compose ps

# Limpar containers e volumes
docker compose down --volumes --remove-orphans
```

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 🔎 Swagger

O Swagger UI fica disponível na raiz da aplicação em ambiente de desenvolvimento:

`http://localhost:8080/`
