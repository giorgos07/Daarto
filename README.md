# Daarto

The main purpose of this repository is to demonstrate a custom implementation of 
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) stores by using SQL Server and 
[Dapper](https://github.com/StackExchange/Dapper), in case you do not want to use the default implementation provided by Entity Framework.
The sample application uses ASP.NET Core 3.1 and is built by using [Visual Studio 2019 Community Edition](https://www.visualstudio.com/vs/community/) 
and SQL Local DB.

## Getting Started

The solution contains a [project](https://github.com/giorgos07/Daarto/tree/master/src/AspNetCore.Identity.Dapper) which implements the core interfaces
of ASP.NET Core Identity framework by using plain T-SQL. You can use the project as a base for your own implementation but you can also include it in
your project as a [nuget package](https://www.nuget.org/packages/AspNetCore.Identity.DapperOrm).

Please refer to the [wiki](https://github.com/giorgos07/Daarto/wiki) section if you want to learn more how to use the package and the capabilities it offers.