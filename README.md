# MyWebApp

Clean Architecture ASP.NET Core Web API template intended for microservice-style solutions.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (latest version)
- [Node.js](https://nodejs.org/) (latest LTS, only required if you are using Angular or React)

## Install

From the repository root:

```bash
dotnet new install .
```

Or install just this template:

```bash
dotnet new install ./MyWebApp
```

## Create a project

```bash
dotnet new web-api-template -n MyService --safeName MyService
```

## Uninstall

```bash
dotnet new uninstall .
```

## Docker compose (optional)

The `docker-compose.dcproj` file is included for Visual Studio container tooling, but it is not part of the solution by default to avoid requiring Docker Desktop for test runs. If you want it in the solution, add it manually in Visual Studio.

## Production build and run (Docker Compose)

Use the production compose overlay and avoid the development override file.

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d
```

Stop containers:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml down
```

## Development build and run (Docker Compose)

The default Docker Compose behavior will automatically include `docker-compose.override.yml`.

```bash
docker compose up --build
```
