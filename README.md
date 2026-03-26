Diagrama de blocos:

<img width="1575" height="1198" alt="image" src="https://github.com/user-attachments/assets/4df30b69-f7be-4385-9a54-5fada629e1fe" />

Arquitetura de soluções:

<img width="1046" height="510" alt="image" src="https://github.com/user-attachments/assets/bb7ff5f6-7eb2-44af-a000-c40d1aa0248d" />


MonitorEconomic

API para consultar e armazenar IPC (Índice de Preços ao Consumidor) usando .NET 8, PostgreSQL e Docker.

## 🏗️ Arquitetura

- **Clean Architecture** com separação clara de responsabilidades
- **CQRS** com MediatR para commands e queries
- **Repository Pattern** para acesso a dados
- **AutoMapper** para mapeamento DTO ↔ Domain
- **PostgreSQL** como banco de dados
- **Docker** para containerização

## 📋 Pré-requisitos

- Docker e Docker Compose instalados
- .NET SDK 8.0

## 🚀 Como executar

### 1. Clonagem e configuração

```bash
git clone <repository-url>
cd Monitor-economic
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

> 💡 **Nota**: A API aplica as migrations automaticamente ao iniciar.

## 📡 API Endpoints

### 🔍 Buscar dados da API externa

**GET** `/api/ipc`

Busca dados de IPC da API externa do IBGE para um período específico.

**Parâmetros de Query:**
- `dataInicial` (string): Data inicial no formato YYYY-MM-DD
- `dataFinal` (string): Data final no formato YYYY-MM-DD

**Exemplo:**
```bash
curl "http://localhost:8080/api/ipc?dataInicial=2024-01-01&dataFinal=2024-03-31"
```

**Resposta de sucesso (200):**
```json
[
  {
    "data": "2024-01-01",
    "valor": 0.65
  },
  {
    "data": "2024-02-01",
    "valor": 0.72
  }
]
```

### 💾 Salvar dados no banco

**POST** `/api/ipc/store`

Busca dados da API externa e salva no banco de dados PostgreSQL.

**Parâmetros de Query:**
- `dataInicial` (string): Data inicial no formato YYYY-MM-DD
- `dataFinal` (string): Data final no formato YYYY-MM-DD

**Exemplo:**
```bash
curl -X POST "http://localhost:8080/api/ipc/store?dataInicial=2024-01-01&dataFinal=2024-03-31"
```

**Resposta de sucesso (200):**
```json
[
  {
    "data": "2024-01-01",
    "valor": 0.65
  }
]
```

### 📊 Consultar dados salvos no banco

**GET** `/api/ipc/db`

Retorna todos os registros de IPC salvos no banco de dados.

**Exemplo:**
```bash
curl "http://localhost:8080/api/ipc/db"
```

**Resposta de sucesso (200):**
```json
[
  {
    "data": "2024-01-01T00:00:00",
    "valor": 0.65
  },
  {
    "data": "2024-02-01T00:00:00",
    "valor": 0.72
  }
]
```

**Resposta quando não há dados (404):**
```json
"Nenhum registro de IPC encontrado no banco."
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
SELECT * FROM ipc ORDER BY data DESC;

# Sair do PostgreSQL
\q
```

### Estrutura da tabela `ipc`

```sql
CREATE TABLE ipc (
    data DATE PRIMARY KEY,
    valor DECIMAL(10,4) NOT NULL
);
```

## 🧪 Testando a API

### Usando cURL

```bash
# 1. Buscar dados da API externa
curl "http://localhost:8080/api/ipc?dataInicial=2024-01-01&dataFinal=2024-01-31"

# 2. Salvar dados no banco
curl -X POST "http://localhost:8080/api/ipc/store?dataInicial=2024-01-01&dataFinal=2024-01-31"

# 3. Consultar dados salvos
curl "http://localhost:8080/api/ipc/db"
```

### Usando Postman/Insomnia

1. **GET** `http://localhost:8080/api/ipc?dataInicial=2024-01-01&dataFinal=2024-01-31`
2. **POST** `http://localhost:8080/api/ipc/store?dataInicial=2024-01-01&dataFinal=2024-01-31`
3. **GET** `http://localhost:8080/api/ipc/db`

## 🏛️ Estrutura do Projeto

```
MonitorEconomic/
├── MonitorEconomic.WebUi/          # Camada de Apresentação (ASP.NET Core MVC)
├── MonitorEconomic.Application/    # Camada de Aplicação (CQRS, MediatR)
├── MonitorEconomic.Domain/         # Camada de Domínio (Entities, Interfaces)
├── MonitorEconomic.Infra.Data/     # Camada de Infraestrutura (Repository, Context)
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

4.4 Sair do psql

\q

5️⃣ Usar a API

A API possui endpoints para consultar e armazenar IPC.
Swagger estará disponível em:

http://localhost:8080/swagger

Permite testar os endpoints diretamente do navegador.

6️⃣ Comandos úteis

Ver containers ativos:

docker ps

Parar containers:

docker compose down

Listar tabelas e registros direto (sem entrar no psql):

docker exec -it monitor_economic_db psql -U postgres -d monitor_economic -c "\dt"

docker exec -it monitor_economic_db psql -U postgres -d monitor_economic -c "SELECT * FROM ipc;"

7️⃣ Estrutura do projeto

MonitorEconomic.WebUi → Projeto ASP.NET API

MonitorEconomic.Infra.Data → DbContext e Migrations

MonitorEconomic.Application → Casos de uso (UseCases)

MonitorEconomic.Domain → Entidades e regras de negócio

MonitorEconomic.Ioc → Injeção de dependência
