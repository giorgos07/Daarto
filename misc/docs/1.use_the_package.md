# How to use the package

As mentioned in the [README.md](https://github.com/giorgos07/Daarto/blob/master/README.md) you can include this package in your own project by using the
Nuget package manager - [here](https://www.nuget.org/packages/AspNetCore.Identity.DapperOrm/). The name of the package is 
`AspNetCore.Identity.DapperOrm`.

Once you install the package the only thing you have to do is call the `AddDapperStores` method which comes as an 
[extension method](https://github.com/giorgos07/Daarto/blob/master/src/AspNetCore.Identity.Dapper/IdentityBuilderExtensions.cs#L21) on the
`IdentityBuilder` type.

As explained in the official documentation in order to setup the ASP.NET Core Identity, you need to call the `AddDefaultIdentity` method inside the `ConfigureServices` 
method of your `Startup.cs` file as shown below
```csharp
public void ConfigureServices(IServiceCollection services) {
    // Other services registration.
    services.AddDefaultIdentity<IdentityUser>()
            .AddDapperStores();
    // More services registration.
}
```
This is the simplest form of using the library.