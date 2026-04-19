# BackendDemo

An ASP.NET Core 8 Web API for managing alerts and recipient groups. It supports creating alerts targeted at groups, and tracking per-recipient delivery status.

**Note: the project doesn't support sending alerts to end users yet.**

## Project Structure

```
BackendDemo/
├── BackendDemo/                        # Main Web API project
│   ├── Controllers/                    # HTTP endpoint handlers
│   │   ├── AlertsController.cs         # Alert creation, status, listing
│   │   └── GroupsController.cs         # Group creation and listing
│   ├── Managers/                       # Business logic layer
│   │   ├── Interfaces/
│   │   │   ├── IAlertManager.cs
│   │   │   └── IGroupManager.cs
│   │   ├── AlertManager.cs
│   │   └── GroupManager.cs
│   ├── Repositories/                   # Data access layer
│   │   ├── Interfaces/
│   │   │   ├── IAlertRepository.cs
│   │   │   └── IGroupRepository.cs
│   │   ├── AlertRepository.cs
│   │   └── GroupRepository.cs
│   ├── Models/                         # Domain entities
│   │   ├── Alert.cs
│   │   ├── AlertGroup.cs               # Many-to-many join entity
│   │   ├── DeliveryLog.cs
│   │   ├── DeliveryStatus.cs           # Enum: Pending, Sent, Failed
│   │   └── Group.cs
│   ├── DTOs/                           # Request/response data transfer objects
│   │   ├── CreateAlertRequest.cs
│   │   ├── CreateGroupRequest.cs
│   │   ├── AlertListItem.cs
│   │   ├── AlertStatusResponse.cs
│   │   └── PagedResult.cs
│   ├── Data/
│   │   ├── AppDbContext.cs             # EF Core DbContext
│   │   └── IAppDbContext.cs            # DbContext interface for testability
│   ├── Program.cs                      # App startup and DI registration
│   ├── appsettings.json
│   └── appsettings.Development.json    # SQL Server connection string
└── BackendDemo.Tests/                  # Unit test project (NUnit + Moq)
    └── AlertManagerTests.cs
```

## Architecture

The application follows a layered architecture:

```
Controllers  →  Managers  →  Repositories  →  AppDbContext (EF Core)
```

- **Controllers** handle HTTP concerns and delegate to managers.
- **Managers** contain business logic (validation, orchestration).
- **Repositories** abstract all database queries.
- **Interfaces** at each layer enable dependency injection and unit testing.

## API Endpoints

### Alerts — `v1/alerts`

| Method | Endpoint                        | Description                           |
| ------ | ------------------------------- | ------------------------------------- |
| `POST` | `/v1/alerts`                    | Create a new alert                    |
| `GET`  | `/v1/alerts/{id}/status`        | Get delivery status for an alert      |
| `GET`  | `/v1/alerts?page=1&pageSize=10` | List alerts (paginated, newest first) |

**Create Alert request body:**

```json
{
  "title": "System Maintenance",
  "body": "Scheduled downtime at 2am.",
  "createdBy": "admin",
  "groupIds": [1, 2]
}
```

**Alert status response:**

```json
{
  "totalRecipients": 10,
  "sentCount": 7,
  "failedCount": 2,
  "pendingCount": 1
}
```

### Groups — `v1/groups`

| Method | Endpoint     | Description        |
| ------ | ------------ | ------------------ |
| `GET`  | `/v1/groups` | List all groups    |
| `POST` | `/v1/groups` | Create a new group |

**Create Group request body:**

```json
{
  "name": "Engineering"
}
```

## Data Model

```
Alert (1) ──< AlertGroup >── (1) Group
Alert (1) ──< DeliveryLog
```

- **Alert** — the message being distributed.
- **Group** — a named collection of recipients.
- **AlertGroup** — many-to-many join between Alert and Group.
- **DeliveryLog** — tracks delivery status (`Pending`, `Sent`, `Failed`) per user per alert.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server instance

### Configuration

Update the connection string in `BackendDemo/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=BackendDemo;..."
  }
}
```

### Run

```bash
dotnet run --project BackendDemo
```

The API is available at `http://localhost:5153` (HTTP) or `https://localhost:7100` (HTTPS). The database is created automatically on first startup.

### Test

```bash
dotnet test
```

## Dependencies

| Package                                   | Version | Purpose                 |
| ----------------------------------------- | ------- | ----------------------- |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.15  | ORM / SQL Server driver |
| `Microsoft.EntityFrameworkCore.Design`    | 9.0.15  | EF Core tooling         |
| `NUnit`                                   | 3.14.0  | Unit testing framework  |
| `Moq`                                     | 4.20.72 | Mocking for unit tests  |

## CORS

The API allows requests from `http://localhost:5173` (Vite/frontend dev server) by default. Update the origin in `Program.cs` as needed.
