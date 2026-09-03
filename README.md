# ProjectManager

ProjectManager is a project and task management REST API built with ASP.NET Core and .NET 10. It provides user registration and login, JWT-based authentication, project and task management, task assignment and filtering, and threaded comments.

## Features

- User registration, login, and current-user profile
- JWT Bearer authentication
- Project creation, editing, archiving, deletion, and filtering
- Task creation, editing, deletion, assignment, status tracking, and filtering
- Task comments with replies
- Task and comment relationship counts in detail responses
- Request validation with FluentValidation
- SQL Server persistence with Entity Framework Core and ASP.NET Core Identity
- Interactive API documentation with Swagger UI

## Architecture

The solution is split into four projects:

| Project | Responsibility |
| --- | --- |
| `ProjectManager.API` | HTTP controllers, middleware, authentication configuration, and Swagger |
| `ProjectManager.Application` | Use cases, MediatR commands/queries, DTOs, validation, and application interfaces |
| `ProjectManager.Domain` | Entities, enums, and domain-level exceptions |
| `ProjectManager.Infrastructure` | EF Core `DbContext`, SQL Server persistence, repositories, Identity, and JWT services |

## Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express
- The SQL Server instance configured by the connection string must be available
- Entity Framework Core CLI tools for database migrations

The default connection string targets the SQL Server Express instance `.\\SQLEXPRESS` and the database `ProjectManagerDb`. Update `src/ProjectManager.API/appsettings.json` or use an environment-specific configuration value if your SQL Server setup is different.

Install the EF Core CLI tool if it is not already available:

```powershell
dotnet tool install --global dotnet-ef
```

## Getting Started

Run these commands from the repository root:

```powershell
dotnet restore ProjectManager.slnx
dotnet ef database update `
	--project src/ProjectManager.Infrastructure `
	--startup-project src/ProjectManager.API
dotnet build ProjectManager.slnx
dotnet run --project src/ProjectManager.API --launch-profile http
```

The database update command applies the existing initial migration. When the API runs in Development, it also applies pending migrations and creates the seed data described below. Seeding is idempotent and will not create duplicate records on subsequent starts.

### Seed Data

Development startup creates one demo user and related sample data:

| Field | Value |
| --- | --- |
| Email | `demo@projectmanager.local` |
| Password | `Password123` |

The seed includes two projects, three tasks, and a comment with a reply. Use the demo credentials with Swagger's **Authorize** button to try the protected endpoints. The seed runs only in Development and is intended for local use; change or remove these credentials before deploying to a shared or production environment.

To run with HTTPS instead:

```powershell
dotnet run --project src/ProjectManager.API --launch-profile https
```

The HTTPS profile may require a trusted ASP.NET Core development certificate. Run `dotnet dev-certs https --trust` if needed.

## Swagger

Swagger is enabled when the application runs in the `Development` environment. The launch profiles set this environment automatically.

After starting the API, open:

- [Swagger UI](http://localhost:5183/swagger) when using the HTTP profile
- [Swagger UI over HTTPS](https://localhost:7104/swagger) when using the HTTPS profile

The Swagger document includes the Bearer authentication scheme. To call protected endpoints:

1. Register a user or log in using the Users endpoints.
2. Copy the JWT returned in the response.
3. Select **Authorize** in Swagger UI.
4. Enter `Bearer <your-token>` and authorize.

The token expires after 120 minutes by default.

## API Overview

All routes use the `/api` prefix. Project, task, and comment endpoints require a Bearer token.

### Users

| Method | Route | Description | Authentication |
| --- | --- | --- | --- |
| `POST` | `/api/users/register` | Register a user and return a JWT | Anonymous |
| `POST` | `/api/users/login` | Authenticate a user and return a JWT | Anonymous |
| `GET` | `/api/users/me` | Get the authenticated user's profile | Required |

Registration accepts `email`, `password`, `firstName`, and optional `lastName`. Passwords must satisfy the Identity password policy, which currently requires at least eight characters.

### Projects

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/projects` | Create a project |
| `GET` | `/api/projects` | List projects |
| `GET` | `/api/projects/{id}` | Get project details and task statistics |
| `PUT` | `/api/projects/{id}` | Update a project |
| `DELETE` | `/api/projects/{id}` | Delete a project |

The list endpoint supports the optional query parameters `status`, `isArchived`, and `searchTerm`.

### Tasks

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/tasks` | Create a task under a project |
| `GET` | `/api/tasks` | List tasks |
| `GET` | `/api/tasks/{id}` | Get task details and related counts |
| `PUT` | `/api/tasks/{id}` | Update a task |
| `DELETE` | `/api/tasks/{id}` | Delete a task |

The list endpoint supports the optional query parameters `projectId`, `status`, `priority`, and `assignedToId`. Completing a task automatically sets `CompletedAt`; changing it away from `Completed` clears that value.

### Comments

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/tasks/{taskId}/comments` | Add a comment to a task |
| `GET` | `/api/tasks/{taskId}/comments` | List task comments and replies |
| `DELETE` | `/api/comments/{id}` | Delete a comment |

## Domain Values

Project statuses: `Active`, `OnHold`, `Completed`, `Archived`, `Cancelled`.

Task statuses: `Pending`, `InProgress`, `Review`, `Blocked`, `Completed`, `Cancelled`.

Task priorities: `Low`, `Medium`, `High`, `Critical`, `Urgent`.

## Configuration

Important settings are in `src/ProjectManager.API/appsettings.json`:

- `ConnectionStrings:DefaultConnection`: SQL Server connection string
- `Jwt:Key`: JWT signing key
- `Jwt:Issuer`: token issuer, default `ProjectManager.API`
- `Jwt:Audience`: token audience, default `ProjectManager.Client`
- `Jwt:ExpirationInMinutes`: token lifetime, default `120`

For production, replace the development JWT key and connection string with secure environment variables or a secret store. Do not commit production secrets to source control.

## Sample Requests

Ready-to-use HTTP requests are available in [`src/ProjectManager.API/ProjectManager.API.http`](src/ProjectManager.API/ProjectManager.API.http). Protected requests must include:

```http
Authorization: Bearer <your-token>
```

## Useful Commands

```powershell
# Run the API in Development mode
dotnet run --project src/ProjectManager.API --launch-profile http

# Add a migration
dotnet ef migrations add <MigrationName> `
	--project src/ProjectManager.Infrastructure `
	--startup-project src/ProjectManager.API

# Apply migrations
dotnet ef database update `
	--project src/ProjectManager.Infrastructure `
	--startup-project src/ProjectManager.API
```

There is currently no test project in the solution.