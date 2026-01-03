# Agent Notes for UserService

## Overview
- Service: User Service (REST API for Cerxos Web App)
- Stack: .NET 10 (C#), PostgreSQL
- Features: auth (login/register/logout), user info, login info, profile data (address, blood, image, tag)

## Setup
- Open `UserService.slnx` in Visual Studio 2026 or JetBrains Rider
- Configure DB connection in `appsettings.json`
- To run without a database, set:
  - `"UseInMemoryDatabase": false` (per README; verify intent before changing)

## Tools
- `dotnet-ef` (global tool recommended)
  - Install: `dotnet tool install --global dotnet-ef`
  - Update: `dotnet tool update --global dotnet-ef`

## Migrations (from repo root)
- Add migration:
  - `dotnet ef migrations add "MainMigration" --project src\Infrastructure --startup-project src\WebAPI --output-dir Persistence\Migrations`
- Update database:
  - `dotnet ef database update "MainMigration" --project src\Infrastructure --startup-project src\WebAPI`
- List migrations:
  - `dotnet ef migrations list --project src\Infrastructure --startup-project src\WebAPI`

## Tests
- Run all tests:
  - `dotnet test UserService.slnx`