# User Service
 
User Service is a microservices for master user for all the application. Have authentication for login and all the information regarding the user.

## Technologies
> List the technologies used in the development of the project, including programming languages, frameworks, libraries, and databases.

- PostgreSQL
- .NET 10 C#

## Project Overview 

> Provide a high-level overview of the project, including the project goals, the target audience, and the project scope.

This is user service for Cerxos Web App. It is a RESTful API that provides data for the frontend application. It is built using .NET 10 and PostgreSQL.
Here are functionalities for this web api:

- Authentication for login, register a new user and logout
- User information
- Login information
- Profile including address, blood, image, tag
- RBAC (Role-Based Access Control) with Permissions and Roles management

## Pre-requisites

- .NET 10, Install .NET 10 SDK from [here](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL, Install PostgreSQL from [here](https://www.postgresql.org/download/)
- Visual Studio 2026, Install Visual Studio 2026 from [here](https://visualstudio.microsoft.com/downloads/) (optional)
- JetBrains Rider, Install JetBrains Rider from [here](https://www.jetbrains.com/rider/download/) (optional)
- Visual Studio Code, Install Visual Studio Code from [here](https://code.visualstudio.com/download) (optional)
- Docker, Install Docker from [here](https://www.docker.com/products/docker-desktop) (optional)
- dotnet-ef, Install dotnet-ef from [here](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) (optional)
- NSWag Studio, Install NSWag Studio from [here](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio)
- Postman, Install Postman from [here](https://www.postman.com/downloads/) (optional)

## Setup

> Provide instructions on how to set up the development environment for the project, including installing dependencies, configuring environment variables, and running the project.

- Clone the repository: 
```bash
  git clone 
```
- Open the `UserService.slnx` using Visual Studio 2026 or JetBrains Rider
- Configure the database connection string in `appsettings.json`

```
"UseInMemoryDatabase": false,
```
- This will make sure the application run without database connection
- Start the project using Visual Studio 2026 or JetBrains Rider

## Using Real Database

- Install PostgreSQL from [here](https://www.postgresql.org/download/)

## Installing the tools

`dotnet ef` can be installed as either a global or local tool. Most developers prefer installing `dotnet ef` as a global tool using the following command:

```bash  
dotnet tool install --global dotnet-ef
```  
To use it as a local tool, restore the dependencies of a project that declares it as a tooling dependency using a tool manifest file.

Update the tool using the following command:

```bash  
dotnet tool update --global dotnet-ef
```  

## Verify installation

Run the following commands to verify that EF Core CLI tools are correctly installed:

```bash  
dotnet ef
```  

The output from the command identifies the version of the tools in use:

```bash  

                     _/\__
               ---==/    \\
         ___  ___   |.    \|\
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\

Entity Framework Core .NET Command-line Tools 2.1.3-rtm-32065  
  
<Usage documentation follows, not shown.>  

```  

## Database Migrations
To use `dotnet-ef` for your migrations first ensure that `UseInMemoryDatabase` is disabled, as described within previous section. Then, add the following flags to your command (values assume you are executing from repository root)

- `--project src/Infrastructure (optional if in this folder)`
- `--startup-project src/WebAPI`
- `--output-dir Persistence/Migrations`

For example, to add a new migration from the root folder:
```bash  
dotnet ef migrations add "MainMigration" --project src\Infrastructure --startup-project src\WebAPI --output-dir Persistence\Migrations
```  

To update the database:
```bash  
dotnet ef database update "MainMigration" --project src\Infrastructure --startup-project src\WebAPI
```
Get the list of migrations:
```bash
dotnet ef migrations list --project src\Infrastructure --startup-project src\WebAPI
```
## Architecture

> Describe the overall architecture of the project, including any notable design patterns, frameworks, or libraries used.

## Code Style

> Describe the code style guidelines used for the project, including any specific linting rules or formatting guidelines.

Using C# coding style

## Testing

> Describe the testing strategy for the project, including any testing frameworks or libraries used. Provide instructions on how to run tests and how to generate test coverage reports.

This project containt 4 unit tests and one integration test, to run all the test

```bash
dotnet test UserService.slnx
```

## Releases

```bash
dotnet build UserService.slnx -c Release /p:TreatWarningsAsErrors=true
```

## Deployment

> Describe the deployment process for the project, including any cloud platforms, hosting providers, or containerization strategies used. Provide instructions on how to deploy the project to a production environment.

## API Documentation

> If the project includes an API, provide detailed documentation for each API endpoint, including the endpoint URL, request and response parameters, and expected responses.

### RBAC (Role-Based Access Control) Endpoints

The User Service includes comprehensive RBAC functionality through Permissions and Roles endpoints. All RBAC endpoints require authentication via JWT token in the Authorization header.

#### Permissions Endpoints

**Base URL:** `/api/Permissions`

##### Get All Permissions

```http
GET /api/Permissions
Authorization: Bearer {token}
```

Query Parameters:
- `PageNumber` (optional): Page number for pagination (default: 1)
- `PageSize` (optional): Number of items per page (default: 10)
- `Filter` (optional): Filter expression using OData-like syntax. Examples:
  - `name eq 'users.read'` - exact match
  - `name contains 'user'` - contains substring
  - `name startswith 'users'` - starts with
- `OrderBy` (optional): Field to sort by (e.g., `name`, `name desc`)

Response:
```json
{
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "users.read",
        "normalizedName": "USERS.READ",
        "description": "Permission to read user data"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1
  },
  "succeeded": true,
  "message": "Request successful"
}
```

##### Get Permission by ID

```http
GET /api/Permissions/{id}
Authorization: Bearer {token}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "users.read",
    "normalizedName": "USERS.READ",
    "description": "Permission to read user data"
  },
  "succeeded": true,
  "message": "Request successful"
}
```

##### Create Permission

```http
POST /api/Permissions
Authorization: Bearer {token}
Content-Type: application/json
```

Request Body:
```json
{
  "name": "users.write",
  "description": "Permission to create and update user data"
}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "users.write",
    "normalizedName": "USERS.WRITE",
    "description": "Permission to create and update user data"
  },
  "succeeded": true,
  "message": "Created permission with id 3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

##### Update Permission

```http
PATCH /api/Permissions/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

Request Body:
```json
{
  "name": "users.write",
  "description": "Updated permission description"
}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "users.write",
    "normalizedName": "USERS.WRITE",
    "description": "Updated permission description"
  },
  "succeeded": true,
  "message": "Permission updated."
}
```

##### Delete Permission

```http
DELETE /api/Permissions/{id}
Authorization: Bearer {token}
```

Response:
```json
{
  "data": "Permission deleted.",
  "succeeded": true,
  "message": "Permission deleted."
}
```

#### Roles Endpoints

**Base URL:** `/api/Roles`

##### Get All Roles

```http
GET /api/Roles
Authorization: Bearer {token}
```

Query Parameters:
- `PageNumber` (optional): Page number for pagination (default: 1)
- `PageSize` (optional): Number of items per page (default: 10)
- `Filter` (optional): Filter expression using OData-like syntax. Examples:
  - `name eq 'Administrator'` - exact match
  - `name contains 'admin'` - contains substring
  - `name startswith 'Admin'` - starts with
- `OrderBy` (optional): Field to sort by (e.g., `name`, `name desc`)

Response:
```json
{
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Administrator",
        "normalizedName": "ADMINISTRATOR",
        "description": "Full system access",
        "permissions": ["users.read", "users.write", "roles.manage"]
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1
  },
  "succeeded": true,
  "message": "Request successful"
}
```

##### Get Role by ID

```http
GET /api/Roles/{id}
Authorization: Bearer {token}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Administrator",
    "normalizedName": "ADMINISTRATOR",
    "description": "Full system access",
    "permissions": ["users.read", "users.write", "roles.manage"]
  },
  "succeeded": true,
  "message": "Request successful"
}
```

##### Create Role

```http
POST /api/Roles
Authorization: Bearer {token}
Content-Type: application/json
```

Request Body:
```json
{
  "name": "Manager",
  "description": "Manager role with limited permissions"
}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Manager",
    "normalizedName": "MANAGER",
    "description": "Manager role with limited permissions",
    "permissions": []
  },
  "succeeded": true,
  "message": "Created role with id 3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

##### Update Role

```http
PATCH /api/Roles/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

Request Body:
```json
{
  "name": "Manager",
  "description": "Updated manager role description"
}
```

Response:
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Manager",
    "normalizedName": "MANAGER",
    "description": "Updated manager role description",
    "permissions": []
  },
  "succeeded": true,
  "message": "Role updated."
}
```

##### Delete Role

```http
DELETE /api/Roles/{id}
Authorization: Bearer {token}
```

Response:
```json
{
  "data": "Role deleted.",
  "succeeded": true,
  "message": "Role deleted."
}
```

## Endpoint URL

> Describe the functionality of the API endpoint, including any required parameters or headers.

## Request

> Provide examples of valid requests to the endpoint.

| Operator	                   | Example Filter String       | Effect                               |
|-----------------------------|-----------------------------|--------------------------------------|
| eq (equal)                  | 	"age eq 30"	               | Matches where age == 30              |
| contains	                   | "name contains 'John'"	     | Matches where name.Contains("John")  |   
| startswith                  | 	"name startswith 'Jo'"	    | Matches where name.StartsWith("Jo")  | 
| endswith	                   | "name endswith 'hn'"	       | Matches where name.EndsWith("hn")    |        
| gt (greater than)           | "age gt 30"	                | Matches where age > 30               |       
| ge (greater than or equal)  | 	"age ge 30"	               | Matches where age >= 30              |        
| lt (less than)	             | "age lt 30"	                | Matches where age < 30               |    
| le (less than or equal)	    | "age le 30"	                | Matches where age <= 30              |  

## Response

> Provide examples of valid responses from the endpoint.

## Known Issues

> List any known issues or bugs in the project, including any workarounds or temporary fixes.

## Future Enhancements

> List any future enhancements or features planned for the project.

## Conclusion

> Summarize the key points of the development documentation and provide any additional resources or references for developers working on the project.