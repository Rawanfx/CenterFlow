# CenterFlow

**CenterFlow** is a backend REST API for an educational/tutoring center that lets students enroll and book one-on-one sessions with teachers based on their real-time availability — with automated scheduling, conflict-safe booking, and background session lifecycle management.

Built with **ASP.NET Core (.NET 10)** following **Clean Architecture** and the **CQRS** pattern.

---

## ✨ Features

- **Authentication & Authorization**
- JWT-based authentication (ASP.NET Core Identity)
- Role-based access control: `Admin`, `Teacher`, `Student`
- Separate registration flows for students and teachers
- **Teacher Availability**
- Teachers publish available time slots
- Query available sessions (filterable)
- Delete/cancel available slots
- **Booking Engine**
- Students book sessions with teachers
- **Distributed locking with RedLock + Redis** to prevent double-booking under concurrent requests
- Cancel bookings & enrollments with business-rule enforcement
- **Session Lifecycle Automation**
- Hangfire **recurring job** (daily) that automatically marks completed sessions
- Hangfire dashboard at `/hangfire` for job monitoring
- **Email Notifications** — MailKit/SMTP integration (e.g., booking confirmation flows)
- **Robust API**
- Global exception handling middleware with custom domain exceptions (`NotFoundException`, `ConflictException`, `ForbiddenException`, ...)
- FluentValidation for input validation on commands
- OpenAPI (Swagger) documentation

## 🧱 Architecture

The solution follows **Clean Architecture** with **CQRS (MediatR)**:

```javascript
┌─────────────────────────────────────────────┐
│  CenterFlow (API)                           │
│  Controllers · Middleware · Program.cs      │
├─────────────────────────────────────────────┤
│  CenterFlow.Application                     │
│  CQRS Handlers · Validators · Interfaces ·  │
│  Custom Exceptions                          │
├─────────────────────────────────────────────┤
│  CenterFlow.Domain                          │
│  Entities · Enums (BookingStatus,           │
│  GradeLevel, SystemRoles)                   │
├─────────────────────────────────────────────┤
│  CenterFlow.Infrastructure                  │
│  EF Core DbContext · Migrations · Identity  │
│  Seeder · JWT/Email Services · Hangfire Job │
└─────────────────────────────────────────────┘
```

### Domain Entities

`ApplicationUser` · `Student` · `Teacher` · `Subject` · `Room` · `TeacherAvailability` · `Book` · `StudentBooking`

## 🛠️ Tech Stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core Web API (.NET 10) |
| Architecture | Clean Architecture, CQRS, MediatR |
| Validation | FluentValidation |
| Database | SQL Server + EF Core (Code-First, Migrations) |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Background Jobs | Hangfire (SQL Server storage) |
| Concurrency | RedLock.net + StackExchange.Redis (distributed locks) |
| Email | MailKit / MimeKit (SMTP) |
| Docs | OpenAPI / Swagger |

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- Redis (local or remote)

### Configuration

Add the following to `appsettings.json` (or user-secrets):

```json
{
  "cs": "Server=...;Database=CenterFlow;...",
  "hangfire": "Server=...;Database=CenterFlowHangfire;...",
  "Redis": "localhost:6379",
  "JWT": {
    "Issuer": "CenterFlow",
    "Audience": "CenterFlow-users",
    "Key": "your-256-bit-secret-key"
  },
  "EmailSetting": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Email": "you@example.com",
    "Password": "your-app-password"
  }
}
```

### Run

```bash
git clone https://github.com/Rawanfx/CenterFlow.git
cd CenterFlow
dotnet restore
dotnet ef database update --project CenterFlow.Infrastructure   # apply migrations
dotnet run --project CenterFlow
```

- API (OpenAPI): `https://localhost:<port>/openapi/v1.json` (dev)
- Hangfire Dashboard: `https://localhost:<port>/hangfire`

> On startup, roles (`Admin`, `Teacher`, `Student`) are auto-seeded via `IdentitySeeder`.

## 📡 API Overview

| Area | Endpoints |
| --- | --- |
| Auth | `POST /api/auth/login` · `POST /api/auth/student-register` · `POST /api/auth/teacher-register` |
| Availability | `POST/GET/DELETE /api/availability...` (add slot, list available sessions, delete slot) |
| Bookings | `POST /api/bookings` · `POST /api/bookings/cancel` |
| Enrollment | `POST /api/enrollment` · `POST /api/enrollment/cancel` |

## ⚙️ Key Design Decisions

- **Distributed locks (RedLock)** around the booking flow so two students can't book the same slot at the same time, without locking the whole database.
- **CQRS with MediatR** keeps features isolated (one folder per use-case: Command + Handler + Validator), making the codebase easy to extend and test.
- **Hangfire recurring job** replaces manual/admin intervention for closing out finished sessions.

## 🔮 Roadmap

- [ ] Unit & integration tests (xUnit)
- [ ] Docker support (API + SQL Server + Redis)
- [ ] SignalR real-time notifications
- [ ] Payment integration
- [ ] Refresh tokens

## 👤 Author

**Rawan** — GitHub: [@Rawanfx](https://github.com/Rawanfx)
