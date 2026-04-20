Arquitetura de soluções:

<img width="1976" height="1059" alt="image" src="https://github.com/user-attachments/assets/6ffc96fa-dfde-4940-8184-4f7072e7b925" />




MonitorEconomic

API para consultar dados do Bacen usando .NET 8, PostgreSQL e Docker, com fallback entre cache, banco e fonte externa.

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

> 💡 **Nota**: este projeto usa acesso direto com Npgsql e cria as tabelas por série sob demanda.

## 📡 API Endpoints

### 🔍 Consultar dados do Bacen

**GET** `/api/bacen`

Fluxo da consulta:

1. Busca primeiro no cache do servidor.
2. Se não encontrar, busca no banco PostgreSQL.
3. Se ainda não encontrar, consulta a API do Bacen.
4. Quando consulta externamente, grava o resultado no banco e atualiza o cache.

**Parâmetros de Query:**
- `serie` (enum, obrigatório): Série do Bacen a ser consultada.
- Mapeamento atual do enum `BacenSerie`:
  - `1 = Ipc`
  - `2 = Dolar`
  - `3 = Euro`
  - `4 = Selic`
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

### 🧠 Regras de cache

- O cache fica no servidor da API em execução no Docker.
- Cada entrada pode permanecer por até 15 dias.
- Para séries de moedas atualizadas diariamente, como `Dolar` e `Euro`, o cache expira no fim do dia e só volta a ser preenchido na primeira consulta seguinte.
- Toda virada para o dia 1 de um novo mês limpa o cache anterior.
- Quando o banco é atualizado por uma consulta externa, o cache da mesma consulta também é atualizado.

## 🗄️ Banco de Dados

### Acessar o PostgreSQL

```bash
# Entrar no container do banco
docker exec -it monitor_economic_db psql -U postgres -d monitor_economic

# Listar todas as tabelas
\dt

# Ver estrutura de uma tabela de serie
\d ipc

# Consultar os registros de uma serie
SELECT * FROM ipc ORDER BY "Data" DESC;

# Sair do PostgreSQL
\q
```

### Estrutura das tabelas por serie

```sql
CREATE TABLE ipc (
    "Id" UUID PRIMARY KEY,
    "Data" DATE NOT NULL,
    "Valor" DECIMAL(10,4) NOT NULL
);
```

Cada serie utiliza sua propria tabela, usando o nome do enum em minusculas.

Exemplos de mapeamento atual:

- `Ipc` -> tabela `ipc`
- `Dolar` -> tabela `dolar`
- `Euro` -> tabela `euro`
- `Selic` -> tabela `selic`

## 🧪 Testando a API

### Usando cURL

```bash
# 1. Consultar dados
curl "http://localhost:8080/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024"

```

### Usando Postman/Insomnia

1. **GET** `http://localhost:8080/api/bacen?serie=Ipc&dataInicial=01/01/2024&dataFinal=31/01/2024`

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
