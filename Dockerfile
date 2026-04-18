# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todos os projetos de forma organizada
COPY *.sln ./
COPY MonitorEconomic.WebUi/MonitorEconomic.WebUi.csproj MonitorEconomic.WebUi/
COPY MonitorEconomic.Application/MonitorEconomic.Application.csproj MonitorEconomic.Application/
COPY MonitorEconomic.Domain/MonitorEconomic.Domain.csproj MonitorEconomic.Domain/
COPY MonitorEconomic.Infra.Data/MonitorEconomic.Infra.Data.csproj MonitorEconomic.Infra.Data/
COPY MonitorEconomic.Ioc/MonitorEconomic.Infra.Ioc.csproj MonitorEconomic.Ioc/
COPY MonitorEconomic.Tests/MonitorEconomic.Tests.csproj MonitorEconomic.Tests/
 

# Restaurar dependências
RUN dotnet restore

# Copia todo o código (subpastas)
COPY . .

# Publica o projeto
RUN dotnet publish MonitorEconomic.WebUi/MonitorEconomic.WebUi.csproj -c Release -o /app/publish

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MonitorEconomic.WebUi.dll"]