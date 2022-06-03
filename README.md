# ITblog v0.1
This is a .net 6.0 blog application with using webAPI + consuming in webApp (consuming nod ended yet).


## Technologies used
- .NET 6.0 WebAPI (REST) and WebApp
- JWT Authentication
- Entity Framework
- Identity

## Installation
For local installation you must create databases from context.

In command prompt (for local usage) you must go to the directory with project and write:
```
dotnet ef database update --context AppIdentityDbContext
dotnet ef database update --context ApplicationDbContext
```
For implement this project on web hosting like Azure you can check [This tutorial](https://docs.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app?tabs=azure-portal%2Cvisualstudio-deploy%2Cdeploy-instructions-azure-portal%2Cazure-portal-logs%2Cazure-portal-resources)

If you are using Visual studio, you must set up multiple startup projects. 
Right-click on Solution>set startup projects>many startup projects>ITblogAPI and ITblogWeb set to start-up projects.


## License
[MIT](https://choosealicense.com/licenses/mit/)

