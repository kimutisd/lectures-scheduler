# Introduction 
This is a showcase .NET Core 2.2 Web API solution, for displaying on how service should be created using step by step commits.
If you want to see how this service evolves, you should follow from the first commit, where the service is very basic, to the last commit,
where the service is a bit more advanced and structured in a nice way with using EF.

## First commit - basic application
In our first commit we can see basic web api set up with swagger support.
At this point we have no error handling, no database, simply a controller that returns hardcoded value.
Install Swashbuckle.AspNetCore via Nuget package manager to be able to use swagger.
In startup class you must add:
In ConfigureServices method:

```csharp
services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Title = "Your Service Name API", Version = "v1" });
                x.DescribeAllEnumsAsStrings();
            });

```
and in Configure method:

```csharp
 app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", ServiceName + " API V1");
            });

            app.Use(async (httpContext, next) =>
            {
                if (string.IsNullOrEmpty(httpContext.Request.Path) ||
                    httpContext.Request.Path == "/" ||
                    httpContext.Request.Path == "/api")
                {
                    httpContext.Response.Redirect(httpContext.Request.PathBase + "/swagger");
                    return;
                }

                await next();
            });

            app.UseHttpsRedirection();
```


## Second commit - adding autofac to handle our dependencies and handling errors gracefully

In this commit we want to start using Autofac for our dependency injection project, because Autofac has a bit more flexibility over default 
.Net Core dependency injection.
We also set up handling of the errors by using middleware - this is one of the top advantages for developers when using .Net Core, to be able to write down your own custom middleware, which is executed each time you perform a request to your web api, so for errors - look at ErrorHandlingMiddleware.cs.
If we have error handling, we obviously want to store them in the log - for that reason we are using Serilog.
Serilog is one of the top logger libraries for .NET and it is very easy to set up.
From Nuget you have to install Serilog, and then if tou want to log to a file - Serilog.Sinks.File.
After doing that, in Startup.cs please add the following:

```csharp
    Log.Logger = new LoggerConfiguration()
        .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
```

and then if you are using Autofac as I do, please add Serilog logger to the container, so that you could use it in your application.
In my case, it is done like this:
```csharp
    containerBuilder.Register(x => Log.Logger).SingleInstance(); 
```

And thats it! Now you can use Serilog ILogger in every class that you like by injecting it via constructor (or property, if you choose this way).