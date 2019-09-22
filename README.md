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