# Library Management System

A .NET 10 Library Management System built for the CODE81 technical assessment — RESTful API, role-based access control, and a relational schema covering books, members, system users, and borrowing transactions.

## Tech Stack

- **.NET 10** / ASP.NET Core Web API
- **EF Core 10** + **SQL Server** (LocalDB for local dev)
- **ASP.NET Core Identity** — user accounts, roles, password hashing
- **JWT Bearer authentication**
- **MediatR** — CQRS command/query dispatch
- **FluentValidation** — request validation
- **Ardalis.Specification** — the Specification pattern for composable queries
- **Swashbuckle (Swagger)** — interactive API docs at `/swagger`

## Architecture

Clean Architecture, four projects, dependencies point inward only:

```
Domain          → Entities, enums, no dependencies on the rest of the solution
Application     → CQRS commands/queries/handlers, validators, DTOs, interfaces
Infrastructure  → EF Core, Identity, JWT, the audit interceptor
API             → Controllers, Program.cs, Swagger, middleware
```

Application never references Infrastructure directly — it depends on interfaces (`IApplicationDbContext`, `ICurrentUserService`, `IJwtTokenService`) that Infrastructure implements. Controllers are thin: they translate an HTTP request into a MediatR call and map the result to a status code. No business logic lives in a controller.

Every feature (Books, Authors, Publishers, Categories, Members, System Users, Borrowing) follows the same Command/Query/Handler/Validator shape, organized as vertical feature folders under `Application/Features/`.

Query filtering (Books' search/status filters) uses the Specification pattern via `Ardalis.Specification`, keeping filter logic out of handlers instead of a repository method with a long list of optional parameters.

## Database Schema

**Entities:** `Book`, `Author`, `Publisher`, `Category`, `Member`, `ApplicationUser` (system users, via Identity), `BorrowingTransaction`, `ActivityLog`, plus join entities `BookAuthor` and `BookCategory`.

**Key relationships:**
- `Book` ↔ `Author` and `Book` ↔ `Category`: many-to-many
- `Book` → `Publisher`: many-to-one
- `Category` → `Category`: self-referencing (`ParentCategoryId`), so genres nest — e.g. Fiction → Sci-Fi → Space Opera
- `BorrowingTransaction` → `Book`, `Member`, `ApplicationUser` (the staff member who processed it)

**Category hierarchy note:** the self-reference uses `DeleteBehavior.Restrict` (SQL Server rejects a cascading self-reference). Reassigning a category's parent is checked against creating a cycle before saving — `UpdateCategoryCommandHandler` walks the proposed parent's ancestor chain and rejects the change if it would loop back to the category being edited.

ERD: `docs/erd.png`.

## Role-Based Access Control

Three roles: **Administrator**, **Librarian**, **Staff**, seeded automatically on first run along with one Administrator account.

| Policy | Administrator | Librarian | Staff |
|---|---|---|---|
| `ManageSystemUsers` | ✅ | | |
| `ManageBooks` | ✅ | ✅ | |
| `ManageMembers` | ✅ | ✅ | |
| `ViewCatalog` (reads + borrowing history) | ✅ | ✅ | ✅ |
| `ProcessBorrowReturn` | ✅ | ✅ | ✅ |
| `ViewActivityLogs` | ✅ | ✅ | |

System Users are deactivated, not hard-deleted, via `DELETE /api/systemusers/{id}` — `BorrowingTransaction` references the processing user with a `Restrict` delete, so removing a user who's handled a transaction isn't possible anyway.

## Security & Activity Logging

- Passwords are hashed via ASP.NET Core Identity's `PasswordHasher` (PBKDF2 + per-user salt) — never handled or stored directly.
- JWTs carry the user's Id, name, and role claim, expiring after 60 minutes.
- The JWT signing key is not committed — see Setup below.
- An EF Core `SaveChangesInterceptor` automatically writes every Added/Modified/Deleted entity to `ActivityLogs`, excluding Identity's internal tables and the log table itself. It also stamps `CreatedAt`/`UpdatedAt` automatically.

## Bonus Requirements

| Requirement | Implementation |
|---|---|
| Search books by Name, Author, or Category | `GET /api/books?searchTerm=&categoryId=` |
| Get books by status | `GET /api/books?status=Available` |
| Postman collection | `docs/LibraryManagementSystem.postman_collection.json` |

## API Overview

All endpoints except `/api/auth/login` require a Bearer token.

- `POST /api/auth/login`
- `GET|POST|PUT|DELETE /api/books`
- `GET|POST|PUT|DELETE /api/authors`
- `GET|POST|PUT|DELETE /api/publishers`
- `GET|POST|PUT|DELETE /api/categories`
- `GET|POST|PUT|DELETE /api/members`
- `GET|POST|PUT|DELETE /api/systemusers` (Administrator only)
- `POST /api/borrowing/borrow`, `POST /api/borrowing/{transactionId}/return`, `GET /api/borrowing/history`
- `GET /api/activitylogs` (Administrator, Librarian only)

Full interactive docs at `/swagger`.

## Getting Started

**Prerequisites:** .NET 10 SDK, SQL Server LocalDB.

1. Clone the repo, open the solution.
2. Set the JWT signing key via User Secrets — right-click `API` → **Manage User Secrets**:
   ```json
   { "Jwt": { "SecretKey": "a-long-random-string-at-least-32-characters" } }
   ```
3. Confirm the connection string in `API/appsettings.json` (defaults to LocalDB).
4. Apply migrations — Package Manager Console, Default project = **Infrastructure**:
   ```powershell
   Update-Database -StartupProject API
   ```
5. Run (F5). Roles and one Administrator account are seeded automatically, then Swagger opens.

**Default seeded login:**
```
Email:    admin@library.com
Password: Admin@12345
```

## What I'd Add With More Time

- Automated tests (unit + integration) — the main known gap given the timeline
- Refresh tokens and book renewal (currently a fixed 14-day loan, no extension)
- Consistent pagination across all list endpoints (currently only Books and Activity Logs)
