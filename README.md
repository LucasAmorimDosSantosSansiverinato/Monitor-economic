MonitorEconomic

API para consultar e armazenar IPC (Índice de Preços ao Consumidor) usando .NET 8, Entity Framework Core, PostgreSQL e Docker.

Pré-requisitos

Docker e Docker Compose instalados

.NET SDK 8.0

Git (opcional para clonar o projeto)

1️⃣ Clonar o projeto
git clone <URL_DO_SEU_REPOSITORIO>
cd Monitor-economic
2️⃣ Configurar variáveis de ambiente

Crie um arquivo .env na raiz do projeto:

POSTGRES_DB=monitor_economic
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123

ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=monitor_economic;Username=postgres;Password=postgres123
ASPNETCORE_ENVIRONMENT=Development
3️⃣ Rodar com Docker Compose

O projeto já contém:

Um container para a API (MonitorEconomic.WebUi)

Um container para o banco PostgreSQL (monitor_economic_db)

docker compose up --build

A API estará acessível em: http://localhost:8080

O PostgreSQL estará rodando na porta 5432.

💡 A API aplica as migrations automaticamente ao iniciar, então se a tabela ipc não existir, ela será criada.

4️⃣ Acessar o PostgreSQL e ver as tabelas

4.1 Entrar no container do banco

docker exec -it monitor_economic_db psql -U postgres -d monitor_economic

4.2 Listar todas as tabelas

\dt

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
