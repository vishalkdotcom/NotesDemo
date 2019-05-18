# Notes API Demo

Notes API Demo: https://notesdemo.azurewebsites.net

API Docs: https://notesdemo.azurewebsites.net/swagger


## Sample Test Data

Download [Insomnia](https://insomnia.rest/download/) app

Import [sample data](https://gist.github.com/vishalkdotcom/09da2ac1ceaff4294770cb614f580bdb) in Insomnia to test

---

## Prerequisites:
    GIT
    VS 2017, VS Code
    .NET Core 2.2
    SQL Server 2012+

## Build & Run
Modify the database connection string as per your instance and authentication in `appsettings.Development.json` (for local) or in `appsettings.json` (for production).

    dotnet restore
    dotnet build
    dotnet ef database update
    dotnet run

## Test

Browse to http://localhost:5000.

Test the API using any tool, like Postman, Insomnia, Curl, etc.

## API Docs

Browse to http://localhost:5000/swagger for api documentation.

---