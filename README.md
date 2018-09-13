# Daarto

The main purpose of the project is to demonstrate a custom implementation of [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) by using SQL Server and [Dapper](https://github.com/StackExchange/Dapper), in case you are not a great fan of [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) (like me).
The application uses ASP.NET Core 2.1 and is built by using [Visual Studio 2017 Community Edition](https://www.visualstudio.com/vs/community/) and SQL Local DB.

## Getting Started

In order to successfully run the application, the only thing you have to do is re-create the database used, in your local machine. Inside **Daarto.DataAccess** you will find a folder called **DatabaseScripts**. There you will see a file called [GenerateDatabase.sql](https://github.com/giorgos07/Daarto/blob/master/Daarto.DataAccess/DatabaseScripts/GenerateDatabase.sql).
Execute this T-SQL script in SQL Server Management Studio Query Window and all the required tables, views and stored procedures will be created automatically in a database called **DaartoDb**. Then, inside the same folder run the following scripts in the order displayed below.

* [GenerateRolesTableData.sql](https://github.com/giorgos07/Daarto/blob/master/Daarto.DataAccess/DatabaseScripts/GenerateRolesTableData.sql)
* [GenerateUsersTableData.sql](https://github.com/giorgos07/Daarto/blob/master/Daarto.DataAccess/DatabaseScripts/GenerateUsersTableData.sql)
* [GenerateUsersRolesTableData.sql](https://github.com/giorgos07/Daarto/blob/master/Daarto.DataAccess/DatabaseScripts/GenerateUsersRolesTableData.sql)

When running the application use the following credentials to use an administrator's account

* **email:** admin@daarto.com
* **password:** abc!37KW!

Logging in with the administrator role will give you the ability to access the admin dashboard.

I will try to add more features as i develop the application. If you like my work, feel free to use this project as a starting point for your own applications.