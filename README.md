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