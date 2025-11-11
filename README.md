# Future of Work API

API RESTful desenvolvida em .NET para gestão de vagas, candidatos e aplicações no contexto do **Futuro do Trabalho**. Esta solução implementa boas práticas REST, monitoramento, versionamento, integração com banco de dados, testes automatizados e integração com ML.NET para previsões inteligentes.

## 📋 Índice

- [Características](#características)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Executando a API](#executando-a-api)
- [Documentação da API](#documentação-da-api)
- [Versionamento](#versionamento)
- [Autenticação](#autenticação)
- [Endpoints Principais](#endpoints-principais)
- [ML.NET - Machine Learning](#mlnet---machine-learning)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)

## 🚀 Características

### 1. Boas Práticas REST (30 pts)

- ✅ **Paginação**: Implementada em todos os endpoints de listagem com parâmetros `pageNumber` e `pageSize`
- ✅ **HATEOAS**: Links de navegação incluídos em todas as respostas (self, first, prev, next, last)
- ✅ **Status Codes Adequados**: Uso correto de códigos HTTP (200, 201, 204, 400, 401, 404, 500)
- ✅ **Verbos HTTP**: Implementação correta de GET, POST, PUT, DELETE

### 2. Monitoramento e Observabilidade (15 pts)

- ✅ **Health Check**: Endpoint `/health` para verificação de saúde da API e banco de dados
- ✅ **Logging**: Configurado com Serilog, registrando logs em arquivo e console
- ✅ **Tracing**: Rastreamento distribuído usando W3C Trace Context para rastreamento de requisições

### 3. Versionamento da API (10 pts)

- ✅ **Versões**: API estruturada em `/api/v1` e `/api/v2`
- ✅ **Controle de Versões**: Uso do pacote `Microsoft.AspNetCore.Mvc.Versioning`
- ✅ **Swagger**: Documentação separada por versão no Swagger UI

**Diferenças entre V1 e V2:**
- **V1**: Funcionalidades básicas de CRUD para Jobs e Applications
- **V2**: Filtros aprimorados para Jobs (employmentType, minSalary, maxSalary)

### 4. Integração e Persistência (30 pts)

- ✅ **Banco de Dados**: SQL Server (LocalDB) com Entity Framework Core
- ✅ **Migrations**: Sistema de migrations para versionamento do esquema do banco
- ✅ **Repositórios**: Padrão Repository para abstração de acesso a dados

### 5. Testes Integrados (15 pts)

- ✅ **xUnit**: Testes unitários e de integração usando xUnit
- ✅ **Mocking**: Uso de Moq para isolamento de dependências
- ✅ **Cobertura**: Testes para controllers, services e repositórios

### Itens Opcionais

- ✅ **ML.NET**: Integração com ML.NET para:
  - Previsão de compatibilidade entre candidatos e vagas
  - Previsão de demanda de habilidades no mercado
- ✅ **Autenticação**: Autenticação via API Key (header `X-API-Key`)

## 🛠 Tecnologias

- **.NET 9.0**: Framework principal
- **ASP.NET Core**: Framework web
- **Entity Framework Core 9.0**: ORM para acesso a dados
- **SQL Server**: Banco de dados relacional
- **ML.NET 4.0**: Machine Learning
- **Serilog**: Logging estruturado
- **Swashbuckle/Swagger**: Documentação da API
- **xUnit**: Framework de testes
- **Moq**: Framework de mocking

## 📦 Pré-requisitos

- .NET 9.0 SDK ou superior
- SQL Server LocalDB (incluído no Visual Studio) ou SQL Server
- Visual Studio 2022 ou VS Code (opcional)

## 🔧 Instalação

1. Clone o repositório:
```bash
git clone <repository-url>
cd DOTNET
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Configure a connection string no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FutureOfWorkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. Execute as migrations:
```bash
cd FutureOfWork.API
dotnet ef database update --project ../FutureOfWork.Data --startup-project .
```

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FutureOfWorkDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "ApiSettings": {
    "BaseUrl": "https://localhost:7000",
    "ApiKey": "FutureOfWork-API-Key-2024"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

## 🏃 Executando a API

1. Navegue até a pasta da API:
```bash
cd FutureOfWork.API
```

2. Execute a API:
```bash
dotnet run
```

3. Acesse a documentação Swagger:
```
https://localhost:7000/swagger
```

## 📚 Documentação da API

A documentação completa da API está disponível via Swagger UI em:
- **Swagger UI**: `https://localhost:7000/swagger`
- **V1**: `https://localhost:7000/swagger/v1/swagger.json`
- **V2**: `https://localhost:7000/swagger/v2/swagger.json`

## 🔄 Versionamento

A API suporta múltiplas versões através do versionamento de URL:

### Versão 1 (V1)
- **Base URL**: `/api/v1`
- **Características**: Funcionalidades básicas de CRUD
- **Endpoints**:
  - `GET /api/v1/jobs` - Listar vagas
  - `GET /api/v1/jobs/{id}` - Obter vaga por ID
  - `POST /api/v1/jobs` - Criar vaga
  - `PUT /api/v1/jobs/{id}` - Atualizar vaga
  - `DELETE /api/v1/jobs/{id}` - Deletar vaga
  - `GET /api/v1/applications` - Listar aplicações
  - `POST /api/v1/applications` - Criar aplicação
  - `POST /api/v1/skills/{skillId}/predict-demand` - Prever demanda de habilidade

### Versão 2 (V2)
- **Base URL**: `/api/v2`
- **Características**: Filtros aprimorados para busca de vagas
- **Endpoints**:
  - `GET /api/v2/jobs` - Listar vagas com filtros avançados (employmentType, minSalary, maxSalary)
  - `GET /api/v2/jobs/{id}` - Obter vaga por ID

## 🔐 Autenticação

A API utiliza autenticação via API Key. Todas as requisições devem incluir o header:

```
X-API-Key: FutureOfWork-API-Key-2024
```

**Configuração da API Key:**
A API Key pode ser configurada no arquivo `appsettings.json` na seção `ApiSettings:ApiKey`.

**Exemplo de requisição:**
```bash
curl -X GET "https://localhost:7000/api/v1/jobs" \
  -H "X-API-Key: FutureOfWork-API-Key-2024"
```

## 📡 Endpoints Principais

### Jobs

#### Listar Vagas (com paginação e HATEOAS)
```http
GET /api/v1/jobs?pageNumber=1&pageSize=10&title=Developer&location=Remote
```

**Resposta:**
```json
{
  "items": [...],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "links": [
    {
      "href": "https://localhost:7000/api/v1/jobs?pageNumber=1&pageSize=10",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://localhost:7000/api/v1/jobs?pageNumber=2&pageSize=10",
      "rel": "next",
      "method": "GET"
    }
  ]
}
```

#### Criar Vaga
```http
POST /api/v1/jobs
Content-Type: application/json

{
  "title": "Software Developer",
  "description": "Desenvolvimento de aplicações .NET",
  "company": "Tech Corp",
  "location": "Remote",
  "salaryMin": 5000,
  "salaryMax": 10000,
  "employmentType": "Full-time"
}
```

### Applications

#### Criar Aplicação (com cálculo de compatibilidade via ML.NET)
```http
POST /api/v1/applications
Content-Type: application/json

{
  "jobId": 1,
  "candidateId": 1,
  "coverLetter": "Estou interessado na vaga..."
}
```

**Resposta inclui `compatibilityScore` calculado via ML.NET:**
```json
{
  "id": 1,
  "jobId": 1,
  "candidateId": 1,
  "status": "Pending",
  "compatibilityScore": 0.85,
  "appliedAt": "2024-01-01T00:00:00Z",
  "links": [...]
}
```

### Skills - ML.NET

#### Prever Demanda de Habilidade
```http
POST /api/v1/skills/1/predict-demand
```

**Resposta:**
```json
{
  "skillId": 1,
  "demandScore": 85,
  "predictedAt": "2024-01-01T00:00:00Z"
}
```

#### Prever Demanda de Todas as Habilidades
```http
POST /api/v1/skills/predict-all-demand
```

## 🤖 ML.NET - Machine Learning

A API utiliza ML.NET para duas funcionalidades principais:

### 1. Previsão de Compatibilidade
O serviço `CompatibilityService` utiliza um modelo de regressão para prever a compatibilidade entre candidatos e vagas baseado em:
- **SkillMatchRatio**: Proporção de habilidades correspondentes
- **ExperienceMatch**: Correspondência de experiência
- **LevelMatch**: Correspondência de níveis de habilidades

### 2. Previsão de Demanda de Habilidades
O serviço `SkillDemandService` utiliza um modelo de regressão para prever a demanda de habilidades no mercado baseado em:
- **JobCount**: Número de vagas que requerem a habilidade
- **ApplicationCount**: Número de aplicações para essas vagas
- **SkillAge**: Idade da habilidade no sistema

## 🧪 Testes

### Executar Testes
```bash
dotnet test
```

### Cobertura de Testes
- ✅ `JobsControllerTests` - Testes do controller de vagas
- ✅ `ApplicationServiceTests` - Testes do serviço de aplicações
- ✅ `CompatibilityServiceTests` - Testes do serviço de compatibilidade ML.NET
- ✅ `SkillDemandServiceTests` - Testes do serviço de previsão de demanda ML.NET

### Exemplo de Teste
```csharp
[Fact]
public async Task GetJobs_ReturnsOkResult_WithJobs()
{
    // Arrange
    var expectedResult = new PagedResult<JobDto> { ... };
    _mockJobService.Setup(s => s.GetJobsAsync(1, 10, null, null, null))
        .ReturnsAsync(expectedResult);

    // Act
    var result = await _controller.GetJobs();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.Equal(2, returnValue.Items.Count());
}
```

## 📁 Estrutura do Projeto

```
FutureOfWork/
├── FutureOfWork.API/              # Camada de API (Controllers, Program.cs)
│   ├── Controllers/
│   │   ├── V1/                   # Controllers da versão 1
│   │   └── V2/                   # Controllers da versão 2
│   └── Program.cs                # Configuração da aplicação
├── FutureOfWork.Domain/           # Entidades de domínio
│   └── Entities/
├── FutureOfWork.Data/             # Camada de dados (DbContext, Repositories)
│   ├── Repositories/
│   └── Migrations/               # Migrations do Entity Framework
├── FutureOfWork.Services/         # Camada de serviços (Business Logic, ML.NET)
│   ├── Services/
│   └── DTOs/
└── FutureOfWork.Tests/            # Testes unitários e de integração
```

## 🔍 Health Check

A API possui um endpoint de health check para monitoramento:

```http
GET /health
```

**Resposta:**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "FutureOfWork.Data.ApplicationDbContext",
      "status": "Healthy"
    }
  ]
}
```

## 📊 Logging

Os logs são gerados usando Serilog e são salvos em:
- **Console**: Saída padrão
- **Arquivo**: `logs/futureofwork-YYYYMMDD.txt`

**Níveis de Log:**
- Information: Operações normais
- Warning: Situações de atenção
- Error: Erros que não interrompem a aplicação
- Fatal: Erros críticos

## 🚦 Status Codes

A API utiliza os seguintes códigos de status HTTP:

- `200 OK` - Requisição bem-sucedida
- `201 Created` - Recurso criado com sucesso
- `204 No Content` - Recurso deletado com sucesso
- `400 Bad Request` - Erro de validação
- `401 Unauthorized` - API Key inválida ou ausente
- `404 Not Found` - Recurso não encontrado
- `500 Internal Server Error` - Erro interno do servidor

## 🔗 HATEOAS

Todas as respostas incluem links HATEOAS para navegação:

```json
{
  "id": 1,
  "title": "Software Developer",
  "links": [
    {
      "href": "https://localhost:7000/api/v1/jobs/1",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://localhost:7000/api/v1/jobs/1",
      "rel": "update",
      "method": "PUT"
    },
    {
      "href": "https://localhost:7000/api/v1/jobs/1",
      "rel": "delete",
      "method": "DELETE"
    }
  ]
}
```

## 📝 Migrations

### Criar Migration
```bash
dotnet ef migrations add NomeDaMigration --project FutureOfWork.Data --startup-project FutureOfWork.API
```

### Aplicar Migrations
```bash
dotnet ef database update --project FutureOfWork.Data --startup-project FutureOfWork.API
```

### Reverter Migration
```bash
dotnet ef migrations remove --project FutureOfWork.Data --startup-project FutureOfWork.API
```

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request


## 👥 Autores

- Desenvolvido por Pietro Saccarrrão Cougo

## 🙏 Agradecimentos

- Professor Humberto pelos ensinamentos

---

**Desenvolvido com horas de trabalho usando .NET 9.0**

