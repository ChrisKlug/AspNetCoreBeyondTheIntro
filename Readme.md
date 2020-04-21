# ASP.NET Core Beyond the Intro

This repo contains the code for my "deep dive" talk about ASP.NET Core.

The EnterpriseEmployeeManagementInc application requires you to log in. This can be done using the username `grace` and password `password`.

## Running the demos

There are some demos that require some set up to get working. Below you can find some documentation on what it does, and in some cases how to get it to work...

### URL Rewriting

If you look in the AwesomeSauceCompanyLtd project's `Startup.cs` you can see that it adds a custom middleware to the request pipeline using `app.UseNameRouting()`. The implementation of this middleware looks at the incoming path, and rewrites it to the internal path needed to the MVC to work. 

### Content negotiation

The AwesomeSauceCompanyLtd application has implemented content negotiation using the Accept header. This allows us to query the API for a user by sending a GET request to https://localhost:44302/api/users/1. By default, it returns a full user object formatted using JSON.

However, by adding a `XmlSerializerOutputFormatter` to the output formatters collection in the `AddControllersWithViews` configuration callback

```csharp

services.AddControllersWithViews(options => {
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
})
```

We can pass in an Accept header value of `text/xml` to have it formatted as XML.

To take that even further, the application contains custom formatting code that allows us to pass in a custom Accept header value and have that be responsible for what MVC action is being called. This can be done using an `AcceptHeaderAttribute` on the action.

```csharp
[HttpGet("{userId}")]
[AcceptHeader("application/vnd.user")]
public async Task<ActionResult<BasicUser>> GetBasicUser(int userId) {
    ...
}
```

With this in place, we can now request this endpoint, which returns a smaller user object, by passing in an Accept header with the value `application/vnd.user`. By default, it comes back as JSON, but is you want XML, you can just change the header to `application/vnd.user+xml`

### Custom model binding

The AwesomeSauceCompanyLtd application implements a custom model binder that binds a User instance as an action parameter instead of just the ID of the user. This allows us to write actions that look like this

```csharp
public ActionResult<BasicUser> GetBasicUser(User user) { ... }
```

This implementation just assumes that there will be a value called `userId` passed along in the request. It then maps that to the actual user for us.

__Note:__ Actually is expects the passed in value to be the name of the parameter (_user_ in the above action) postfixed with "Id". So that ends up with `userId` for this specific example, but it really depends on the parameter name.

### Antiforgery Tokens

To demo the use of antiforgery tokens, you need to use a browser other than Chrome. As Chrome has changed its cookie handling, it will not enable us to do simple cross sit request forgery, as authentication cookies are not sent when requesting a site from another domain. 

To demo antiforgery token usage, start by removing the antiforgery token attribute in the global MVC filters collection. This is done by commenting out line 18 in EnterpriseEmployeeManagementInc/Startup.cs

```csharp
services.AddControllersWithViews(options =>
{
    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
```

Next, start the EnterpriseEmployeeManagementInc application and sign in as Grace (see above comment about logging in). Once you are logged in, start the EvilHaxxorSite. As soon as the EvilHaxxorSite is opened in your browser, it will send a POST request to the EnterpriseEmployeeManagementInc application and add a new user.

To thwart this attack, stop the EnterpriseEmployeeManagementInc and put the `AutoValidateAntiforgeryTokenAttribute` back into the global filters list. Restart, the application and try reloading the EvilHaxxorSite. As you can see, antiforgery tokens are now being used, and the evil haxxor cannot add users using cross site request forgery.

### Background tasks

Background tasks in ASP.NET Core allows us to run code asynchrounously in the background in our application. This is done by registering classes that implement IHostedService in the DI container.

In this case, the EnterpriseEmployeeManagementInc uses this feature to resize images that are being uploaded. For more detail, have a look at the `ThumbnailGenerator` class that gets registered in the DI container in `Startup.ConfigureServices()`

### Extending applications using HostingStartupAttribute and IHostingStartup

To demo this, we need to create a NuGet package that contains an `IHostingStartup` implementation, and a `HostingStartupAttribute` assembly attiribute. The implementation for this is in the RequestDiagnostics class library project. 

__Note:__ The RequestDiagnostics project also contains a `Program.cs`, which is normaly not the case for a class library. This is only there to be used by the `manifest.csproj` during the runtime store creation.

To extend the EnterpriseEmployeeManagementInc web application using the `DiagnosticsHostingStartup` class, we need to create a _runtime store_, and an additional deps.json file (located in the right place...). To do this we need to do the following.

First we need to generate a NuGet package from the RequestDiagnostics project. This can easily be done by running the following command in the RequestDiagnostics folder

```bash
dotnet pack .\RequestDiagnostics.csproj -o deployment/packages
```

Next, we need to create something called a _runtime store_. This is a folder containing NuGet packages that can be stored on a machine separate from an application, and then be used by an application without it having to bring the NuGet package on its own. A bit like a Global Assembly Cache from .NET Framrework.

To create a runtime store, we run the following command

```bash
dotnet store --manifest .\manifest.csproj --runtime win10-x64 --output ./deployment/store --skip-optimization
```

This will create a store based on the NuGet packages referenced in the `manifest.cproj` file. Basically, it creates a structured folder containing all the NuGet packages required by the references in the defined project file.

Once we have our store, this can be used by modifying your applications `.deps.json` file. However, in this case, we cant to extend the application without it knowing about it. So we need to extend the applications dependencies without it knowing about it. This can be done by something called `additionalDeps`.

To add additional dependencies like this, we need to create a `.deps.json` file. The easiest way to do this is to publish a Console app, which is why the `manifest.csproj` is defined as a Console app, and why there is a `Program.cs` file in the project. So to get a .deps.json file, you can run

```bash
dotnet publish manifest.csproj -o ./deployment/temp
```

The generated `./deployment/temp/manifest.deps.json` contains a reference to the project file, which it shouldn't in this case. So open up that file, and remove the reference to `manifest/1.0.0` in the `targets/.NETCoreApp,Version=v3.0` and `libraries` configurations

```
{
  "runtimeTarget": {
    "name": ".NETCoreApp,Version=v3.0",
    "signature": ""
  },
  "compilationOptions": {},
  "targets": {
    ".NETCoreApp,Version=v3.0": {
      // Remove from here
      "manifest/1.0.0": {
        "dependencies": {
          "RequestDiagnostics": "1.0.0"
        },
        "runtime": {
          "manifest.dll": {}
        }
      },
      // To here
      ...
  },
  "libraries": {
    // And from here
    "manifest/1.0.0": {
      "type": "project",
      "serviceable": false,
      "sha512": ""
    },
    // To here
  }
```

Next, that dependencies file needs to be placed in a very specific folder structure that looks like this `{ADD.DEPS PATH}/shared/{FRAMEWORK NAME}/{FRAMEWORK VERSION}/{ENHANCEMENT ASSEMBLY NAME}.deps.json`, which in our case means `{ADD.DEPS PATH}/shared/Microsoft.AspNetCore.App/3.1.0/RequestDiagnostics.deps.json` as we want to extend any application using `Microsoft.AspNetCore.App` version `3.1.*` with the assembly `RequestDiagnostics`.

The easiest way to set this up is by running 

```bash
	xcopy .\deployment\temp\manifest.deps.json \
    .\deployment\additionalDeps\shared\Microsoft.AspNetCore.App\3.1.0\RequestDiagnostics.deps.json* /y
```

The final part to do, is to set up the environment variables that are needed to get the application to load this assembly. For this demo, the easiest way is to just set up the environment variables in the `launchSettings.json` file in the `Properties` folder of the EnterpriseEmployeeManagementInc project.

The environemnt variables needed are

`ASPNETCORE_HOSTINGSTARTUPASSEMBLIES` which contains a comma separated list of all the assemblies we want to load. These will then be loaded and the `HostingStartupAttribute` checked to see what class to instantiate.

`DOTNET_SHARED_STORE` which contains the path to the runtime store folder.

`DOTNET_ADDITIONAL_DEPS` which contains the path to the folder containing the additional deps we want to load.

These are already available in the `launchSettings.json`. You just need to comment it back in.

After this has been done, you should be able to start the EnterpriseEmployeeManagementInc project and have the request diagnostics stuff added dynamically. You can check the functionality by browsing to https://localhost:44367/diagnostics.

__Note:__ The actual implementaion of the request diagnostics use not only `HostingStartupAttribute` and `IHostingStartup`. This is only used to hook into the application startup. At this point, it then registers an `IStartupFilter` implementation in the DI container to get it run when the request pipeline starts up, allowing it to add a middleware to the request pipeline.

### Accessing the HttpContext

In ASP.NET Core, the HttpContext is not readily availbale to us using `HttpContext.Current` as it was previously. Instead, it needs to be injected. This is shown in the EnterpriseEmployeeManagementInc project, where the HttpContext accessor is added to the DI container in `Startup.ConfigureServices()` by calling `services.AddHttpContextAccessor()`.

The `IHttpContextAccessor` is then used in the `Employees` class to support a multi-tenancy situation where the user has a "tenant id" added to its claims. This can be retrieved using the `IHttpContextAccessor.HttpContext.User`, and then used inside the `Employees` class to filter all the employee retrieval requests automatically.

To demonstrate this in action, you can switch between the user `grace` mentioned above, and the user `john` with password `password`.