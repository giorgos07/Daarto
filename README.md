# Daarto

The main purpose of the project is to demonstrate a custom implementation of [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) by using SQL Server and [Dapper](https://github.com/StackExchange/Dapper), in case you do not want to use the default implementation provided by Entity Framework.
The application uses ASP.NET Core 2.1 and is built by using [Visual Studio 2017 Community Edition](https://www.visualstudio.com/vs/community/) and SQL Local DB.

## Getting Started

In order to successfully run the application, the only thing you have to do is re-create the database used, in your local machine. Inside **misc/scripts** folder you will find a file called [create_database_schema.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/create_database_schema.sql).
Execute this `T-SQL` script in SQL Server Management Studio Query Window and all the required tables will be created automatically in a database called **DaartoDb**. The schema is the default one, which is also used by ASP.NET Core Identity. Then run the following scripts in the order displayed below, to generate some demo data (they are random and created by using a very handy online tool called [mockaroo](https://www.mockaroo.com/)).

* [generate_roles_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_roles_table_data.sql)
* [generate_users_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_users_table_data.sql)
* [generate_users_roles_table_data.sql](https://github.com/giorgos07/Daarto/blob/master/misc/scripts/generate_users_roles_table_data.sql)

One last step is to run a `Gulp` task in order to copy some `npm` packages in the *wwwroot/lib*. The task we need to run is called `copy:libs`

> In Visual Studio navigate to View -> Other Windows -> Task Runner Explorer to be able to run this task easily.

When running the application use the following credentials to use an administrator's account

* **email:** admin@daarto.com
* **password:** abc!37KW!

Logging in with the administrator role will give you the ability to access the admin dashboard.

I will try to add more features as i develop the application. If you like my work, feel free to use this project as a starting point for your own applications.