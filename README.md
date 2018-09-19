# Daarto

The main purpose of the project is to demonstrate a custom implementation of [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) by using SQL Server and [Dapper](https://github.com/StackExchange/Dapper), in case you are not a great fan of [Entity Framework](https://docs.microsoft.com/en-us/ef/core/) (like me).
The application uses ASP.NET Core 2.1 and is built by using [Visual Studio 2017 Community Edition](https://www.visualstudio.com/vs/community/) and SQL Local DB.

## Getting Started

In order to successfully run the application, the only thing you have to do is re-create the database used, in your local machine. Inside **misc/scripts** folder you will find a file called [create_database_schema.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/create_database_schema.sql).
Execute this T-SQL script in SQL Server Management Studio Query Window and all the required tables, views and stored procedures will be created automatically in a database called **DaartoDb**. Then, inside the same folder run the following scripts in the order displayed below.

* [generate_roles_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_roles_table_data.sql)
* [generate_users_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_users_table_data.sql)
* [generate_users_roles_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_users_roles_table_data.sql)

When running the application use the following credentials to use an administrator's account

* **email:** admin@daarto.com
* **password:** abc!37KW!

Logging in with the administrator role will give you the ability to access the admin dashboard.

I will try to add more features as i develop the application. If you like my work, feel free to use this project as a starting point for your own applications.