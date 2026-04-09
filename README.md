# DotnetWin API

ASP.NET Core 8 REST API com instrumentação OpenTelemetry.

## Stack

- **Framework**: ASP.NET Core 8
- **Banco de dados**: PostgreSQL 16 + Entity Framework Core 8
- **Cache**: Redis 7
- **Validação**: FluentValidation
- **Observabilidade**: OpenTelemetry

---

## Observabilidade com OpenTelemetry

### O que é instrumentado

| Sinal | Fonte |
|---|---|
| Traces | Requests HTTP, queries EF Core, operações Redis, HttpClient |
| Metrics | Requests HTTP, runtime .NET (GC, threads, memória) |
| Logs | Todos os logs via `ILogger`, correlacionados por trace |

### Exportador

Os sinais são enviados via **OTLP HTTP/Protobuf** para qualquer backend compatível (Grafana Cloud, Jaeger, Tempo, etc.).

### Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `OTEL_SERVICE_NAME` | Nome do serviço nos traces | `dotnet-win-api` |
| `OTEL_SERVICE_VERSION` | Versão do serviço | `1.0.0` |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | URL do coletor OTLP | `https://collector.example.com` |
| `OTEL_EXPORTER_OTLP_PROTOCOL` | Protocolo do exportador | `http/protobuf` |
| `OTEL_EXPORTER_OTLP_HEADERS` | Headers de autenticação | `Authorization=Basic <base64>` |

### Header `otel-trace-id`

Toda resposta da API inclui o header `otel-trace-id` com o trace ID da requisição, facilitando a correlação entre cliente e backend de observabilidade.

```
otel-trace-id: 4bf92f3577b34da6a3ce929d0e0e4736
```

---

## Rodando localmente

### Pré-requisitos

- .NET 8 SDK
- Docker

### 1. Suba o banco e o cache

```bash
docker compose up postgres redis -d
```

### 2. Configure o `.env`

O arquivo `.env` na raiz do projeto contém todas as variáveis necessárias. As variáveis do OpenTelemetry estão comentadas por padrão — descomente para habilitar o envio de telemetria:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;...
ConnectionStrings__Redis=localhost:6379

# Descomente para habilitar telemetria
#OTEL_SERVICE_NAME=dotnet-win-api
#OTEL_EXPORTER_OTLP_ENDPOINT=https://collector.example.com
```

### 3. Carregue as variáveis e rode

```powershell
Get-Content .env | Where-Object { $_ -notmatch '^#' -and $_ -match '=' } | ForEach-Object { $k,$v = $_ -split '=',2; [System.Environment]::SetEnvironmentVariable($k,$v,'Process') }
dotnet run --project DotnetWin.Api
```

A API sobe em `http://localhost:5000` e o Swagger em `http://localhost:5000/swagger`.

---

## Rodando com Docker

```bash
docker compose up --build
```

As variáveis do `.env` são injetadas automaticamente nos containers via `compose.yaml`.

---

## Migrations

As migrations são gerenciadas manualmente via CLI. Para aplicar:

```bash
dotnet ef database update --project DotnetWin.Api --connection "Host=localhost;Port=5432;Database=dotnetwin;Username=postgres;Password=postgres"
```

Para criar uma nova migration após alterar o modelo:

```bash
dotnet ef migrations add <NomeDaMigration> --project DotnetWin.Api --output-dir Infrastructure/Data/Migrations
```

---

## Endpoints

Base URL: `http://localhost:5000/api/v1`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/products` | Lista todos os produtos |
| `GET` | `/products/{id}` | Busca produto por ID |
| `POST` | `/products` | Cria produto |
| `PUT` | `/products/{id}` | Atualiza produto |
| `DELETE` | `/products/{id}` | Remove produto |
