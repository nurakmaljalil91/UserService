# Agent Notes for UserService

## Role
You are a senior engineer working on this project

## Project Overview
- Service: User Service (REST API for Cerxos Web App)
- Stack: .NET 10 (C#), PostgreSQL
- Features: authentications (login/register/logout), user info, login info, profile data (address, blood, image, tag)
- RBAC (Roles and Permissions)

## Non-Negotiable Rules
- ALWAYS add comments to class, methods and properties - 1591
- ALWAYS write unit tests and integration tests - 80% coverage
- ALWAYS add `#nullable enable` if contain nullable

## Architecture Rules

- Domain layer has no dependency on Application or Infrastructure
- Controllers contain no business logic
- Use CQRS (Command / Query separation)
- Use Value Objects instead of primitives where applicable
- No static access except for constants

## Coding Standards

### General
- Prefer explicit over implicit
- No magic numbers
- Use meaningful names

### C# (.NET)
- Use records for immutable models
- Use async/await everywhere
- Use CancellationToken
- One public class per file

## Folder Structure

lib:
- Mediator

src:
- Domain/
- Application/
- Infrastructure/
- WebAPI/

tests:
- Domain.UnitTests/
- Application.UnitTests/
- Infrastructure.UnitTests/
- IntegrationTests/
- WebAPI.UnitTests/
  
## Agent Behavior

When asked to code:
- Follow existing patterns in the codebase
- Ask before making breaking changes

When unsure:
- Ask a clarification question
- Do NOT guess