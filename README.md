# Introduction 
This is a showcase .NET 6.0 Web API solution, for displaying on how service should be created using step by step commits.
If you want to see how this service evolves, you should follow from the first commit, where the service is very basic, to the last commit,
where the service is a bit more advanced and structured in a nice way with using EF.

## Testing
This is a disclaimer about testing in this project - as for this moment there will be no tests, because this solution is a display of practises used to create a service. Basically, there would be two test projects - UnitTests and IntegrationTests. That will come in the later lectures, and might be included here as new commits.

## Disclaimer
This project is for the sake of example, so it might contain bugs, and in many places proper validation.

## First commit - basic application
In our first commit we can see basic web api set up with swagger support.
At this point we have no error handling, no database, simply a controller that returns hardcoded value.
Install Swashbuckle.AspNetCore and Microsoft.OpenApi.Models via Nuget package manager to be able to use swagger.
You will also need Newtonsoft.Json.Converters package to convert enums to strings.
In startup class you must add:
In ConfigureServices method:

```csharp
services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Your Service Name API", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.Converters.Add(new StringEnumConverter()));

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
.NET dependency injection.
We also set up handling of the errors by using middleware - this is one of the top advantages for developers when using .NET, to be able to write down your own custom middleware, which is executed each time you perform a request to your web api, so for errors - look at ErrorHandlingMiddleware.cs.
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

## Third commit - adding more structure to the solution

In this commit we want to start shaping our solution, so that it would have responsible parts for serving data for us.
First thing we want to do is to add project called YourNamespace.Services.

YourNamespace.Services project is responsible for business logic - so in here we want to create services, that will fetch, filter and modify data.
If we would have integrations to other APIs, then here we would create our ServiceAgents.
ServiceAgents - are simple proxies, that does only one thing - calls external API and returns the response. If you want to handle status code coming back from the API you're calling, you should not be doing that in ServiceAgent class - you should be doing that either in middleware, or an aspect (see more here for aspects https://www.codeproject.com/Articles/1083684/Aspect-Oriented-Programming-AOP-in-Csharp-using-Ca). It goes without saying, but for each different API that you're caliling you need to have different ServiceAgent, and then it has to be invoked via Service class, which will do the mapping and modifying of the data for your controller.

What else can go to YourNamespace.Services? Repository, DataAgents, Mappers, EventHandlers. We will see why in upcoming commits.

Also, my personal preference is to keep everything grouped in folders - this way it is much easier for people who are new with your code to get around, and it is easier for myself to get around after couple of weeks of not looking into the solution. So for example in the Services project, I always, always create a folder named Services, and put there my classes and interfaces. If I have more than one Service - then I create a folder for each service and interface.

Second thing to do - create YourNamespace.Domain project.

YourNamespace.Domain - should contain your internal models, that you will return from services, service agents, data agents, if you're using messaging then it also should contain your events. We will see later on, that this project will also contain our entities, because we want to have lose coupling, and implement adapter pattern.
What is the difference between model and entity? Well, there is quite a big difference, but it is easy to understand and great explanation is provided here - https://www.c-sharpcorner.com/blogs/entity-vs-model-vs-viewmodel-vs-datamodel .

Alright, so at this point we have our domain, where we keep model that is returned from Service. Next step is to integrate that Service into our controller and map it to our Contract. My flow of doing that:
* register Service in autofac with 
```csharp
 containerBuilder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerDependency(); 
 ```
* inject your service to controller via constructor
* call injected service
* map response to dto

And here we have to add explanations: mapping to dto is a MUST. You should always separate you domain objects from your contract, so that you would have lose coupling, and you could change you domain object as you like. If the objects are similar, then one of the easiest ways to map is by using AutoMapper - Nuget, that maps it for you.
It is very easy to use, and saves a lot of time, and this is how you should implement it -> https://medium.com/ps-its-huuti/how-to-get-started-with-automapper-and-asp-net-core-2-ecac60ef523f.
But, for the sake of example, I want to show you my favorite mapping technique - extension methods! It is nothing fancy, but it just looks great.
So extension methods (as the name implies) allows you to extend your type with additional methods, and in this case, we will add a method called ToDto.

Lets go back to Services project and add folder called Mapper, inside that folder lets add YourDomainObjectMapper.cs
Syntax for creating an extension method is simple:
```csharp
public static class YourDomainObjectMapper
    {
        public static YourOutgoingDto ToDto(this YourDomainObject from)
        {
            return new YourOutgoingDto { Property1 = from.Property1, Property2 = from.Property2 };
        }        
    }
```

Now in your controller you can simply do this
```csharp
var yourOutgoingDtoVar = _service.MethodToGetData(params).ToDto();
```

Alright, so for this commit we already have done a lot of structuring, but one last tip - to the controller method, that returns your newly created dto object you should add this attribute:
```csharp
        [ProducesResponseType(typeof(YourCreatedDto), 200)]
```

That way, in Swagger documentation you will be able to see all properties exposed by this type. It is not a must, but always nice to have.

## Fourth commit - persitence

Adapter pattern and lose coupling - these are the two drivers that makes your solution re-usable.
Basically, adapter pattern says (and this is my interpretation of it) that the code that is integrating with third parties should be changable very easily, without changing code in your other projects.
So with database, it would be like this: if you're using MSSQL, and want to switch to user NoSql like MongoDB, then you should do you changes only in Persistence project, or create PersistenceNoSql project, and just plug it in into your application without changing anything else. Sound cool, right? But how do we achieve that?
Well, you actually have almost achieved it, if you've been following my commits. Our WebApi project gets data from Services, and Services operates with Domain objects - so it does not know about entities or database implementation (which is yet to come). So if we would create a persitence project, and place all the database specific code to that project, but expose it via repository, then we would be consuming repository, in our service via interface. And voilà - you are completly loosely coupled, and can change database to other, without putting other code at risk. Lets implement it!

First, you should create YourNamespace.Persistence project. In my example I will be using EF Core with code first migrations.
To get started please install Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools Nugets package to your project.

So first, lets go and create Entities folder in Domain project. In that project you should create you entity, and BaseEntity. Why do you need base class? There are some fields that has to go with each entity, such as Id, CreatedOn and so on, so there is no need to specify that in each class.
Also, in you base entity, you must override equality methods. Why? Because if you won't then equality will be checked as for every other object - by reference, and that is not good, because when you have an entity, you should always have a business key, or at least id.
This is an example, of how base entity could look like:
```csharp
    public class Entity
    {
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        protected virtual object Actual => this;

        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Actual.GetType() != other.Actual.GetType())
            {
                return false;
            }

            if (Id == default || other.Id == default)
            {
                return false;
            }

            return Id == other.Id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType().ToString() + Id).GetHashCode();
        }
    }
```

After that, you have to create you DatabaseContext.cs and IDatabaseContext.cs and you have to specify in interface your entity in a DbSet like this:
```csharp
DbSet<YourEntity> YourEntities { get; set; }
```

In DatabaseContext class you should add two methods, that will be responsible for setting CreatedOn and ModifiedOn fields, so that you wouldn't need to:
```csharp
        private void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Entity entity)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
        }

        private void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.NewState == EntityState.Modified && e.Entry.Entity is Entity entity)
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }
```

and then register them in constructor:
```csharp
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            ChangeTracker.Tracked += OnEntityTracked;
            ChangeTracker.StateChanged += OnEntityStateChanged;
        }
```

For setting up you entity, you should be using either fluent api or attributes, but NOT BOTH.

Now, so that you would not forget, lets go and register database context in the container by adding this to Startup.cs ConfigureServices method:
```csharp
var connectionString = Configuration.GetSection("DatabaseConnectionString").Value;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Connection string not found");
            }

            services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }, ServiceLifetime.Transient);
```

and also we have to add our connection string to appSettings.Development.json (for production version we should setup transformation in appSettings.json)"
```json
"DatabaseConnectionString": "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=YourDatabaseName;integrated security=True;MultipleActiveResultSets=True;App=YourApplicationNAme;;MultiSubnetFailover=true;Connect Timeout=30"
```

Now you are ready to add migrations. Open Package Manager Consoler and make sure, that your startup project is WebApi, DefaultProject is Persistence and PackageSource is set to nuget.org, and type in
```
Add-Migration "Initial"
```
After this command, you will see that in Persistence project there is Migrations folder created, with your Initial migration!

To execute it on the database that is specified in appsetings.Development.json, you should type in:
```
Update-Database
```

After this command, if it executes without errors you can go into you SQL manager (I am using Microsoft SQL Server Management Studio) and see that database is created.

And that is more or less it: for using it you have to create repository class with and interface under Services folder, and inject that reposiroty into Service that is called by controller. Basically, it is same actions that we already did, so I will not go into details, but you will be able to see it my commit.

## Fifth commit - updating database on startup

If you have plans of shipping your project, that means you will not be able to run Update-Database all the time, and even for local purposes, sometimes it gets annoying. Lets write some code that would run your pending migrations on startup of the application.

First - add to Persistence project folder named Extensions and add new class under that folder DatabaseMigrationExtensions.cs which should look like:
```csharp
public static class DatabaseMigrationExtensions
    {
        public static void MigrateDatabase<TDbContext>(this TDbContext dbContext) where TDbContext : DbContext
        {
            var db = dbContext.Database;           

            var migrate = false;
            foreach (var pendingMigration in db.GetPendingMigrations())
            {
                migrate = true;
                Console.WriteLine($"These are the migration that will be applied: {pendingMigration}.");
            }

            if (migrate)
            {
                Console.WriteLine("Applying DB migrations ...");
                db.SetCommandTimeout(3 * 60);
                db.Migrate();
            }
            else
            {
                Console.WriteLine("No pending DB migrations.");
            }
        }
    }
```

This class will be responsible for running migrations, if there are some.
Then you need to add the following code to Program.cs
```csharp
private static void MigrateDatabase(IWebHost host)
        {
            var scope = host.Services.CreateScope();
            using (scope)
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection("DatabaseConnectionString").Value;
                var baseOptions = scope.ServiceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>();
                var options = new DbContextOptionsBuilder<DatabaseContext>(baseOptions).UseSqlServer(connectionString);

                using (var db = new DatabaseContext(options.Options))
                {
                    db.MigrateDatabase();
                }

            }
        }
```

and refactor Main method:
```csharp
var host = CreateWebHostBuilder(args).Build();
            MigrateDatabase(host);
            host.Run();
```

After this, you application will be updating database, if there is something to update, everytime you will start it. Besides that, couple more refactorings are being done in this commit, but nothing more that is worth mentioning.

## Sixth commit - adding POST

With this commit POST request are implemented, to seed data to the database, also GetById is exposed. Nothing fancy is added, so it commit basically explains itself - enjoy!

## Further reading
.NET 6.0 has a great functionality for health checking, please refer to the documentation:https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
