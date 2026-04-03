# mediator-forge

[![NuGet Version](https://img.shields.io/nuget/v/MediatorForge.svg?style=flat-square)](https://www.nuget.org/packages/MediatorForge/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MediatorForge.svg?style=flat-square)](https://www.nuget.org/packages/MediatorForge/)
[![Build Status](https://github.com/VikashChauhan51/mediator-forge/actions/workflows/build.yml/badge.svg)](https://github.com/VikashChauhan51/mediator-forge/actions)
[![License](https://img.shields.io/github/license/VikashChauhan51/mediator-forge.svg?style=flat-square)](https://github.com/VikashChauhan51/mediator-forge/blob/main/LICENSE)


MediatorForge is a robust library designed to streamline the implementation of CQRS (Command Query Responsibility Segregation) patterns in .NET applications. It provides:
- Custom dispatcher for commands, queries, and event notifications
- Integration for validation, authorization, logging, and pipeline behaviors
- Clean separation of concerns and enhanced code maintainability


## Features

- **Custom Dispatcher**: Central class for dispatching commands, queries, and publishing events using dependency injection.
- **Command, Query, and Event Notification Handling**: Strongly-typed interfaces for all CQRS operations.
- **Pipeline Behaviors**: Middleware for cross-cutting concerns (logging, validation, etc.).
- **Fluent Validation**: Seamless integration with FluentValidation for request validation.
- **Authorization Logic**: Secure access control with customizable authorization behaviors.
- **Logging**: Monitor and record request processing and performance metrics.
- **Result and Option Types**: Effective handling of operation outcomes.

## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge
```

## Usage


## Architecture Overview

MediatorForge implements CQRS with the following core interfaces:
- `IHandler<TRequest, TResponse>`: Handles commands and queries.
- `IPipelineBehavior<TRequest, TResponse>`: Pipeline middleware for cross-cutting concerns.
- `IEventNotification<TEvent>` / `IEventNotificationHandler<TEvent>`: For event publishing and handling (one-to-many).
- `IDispatcher`: Central interface for dispatching commands, queries, and events.

## Usage

### Command Interfaces

Define and handle commands using custom command interfaces.

#### Example:

```csharp
 public class CreateUserCommand : ICommand<UserResponse>
{
    public string UserName { get; set; }
    public string Email { get; set; }

    public CreateUserCommand(string userName, string email)
    {
        UserName = userName;
        Email = email;
    }
}

public class UserResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    public Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Simulate user creation logic
        var response = new UserResponse
        {
            UserName = request.UserName,
            Email = request.Email,
            Status = "User Created Successfully"
        };

        return Task.FromResult(response);
    }
}

```


### Query Interfaces

Define and handle queries similarly to commands.

#### Example:

```csharp
 public class GetUserQuery : IQuery<UserResponse>
{
    public string UserName { get; set; }

    public GetUserQuery(string userName)
    {
        UserName = userName;
    }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserResponse>
{
    public Task<UserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Simulate getting user logic
        var response = new UserResponse
        {
            UserName = request.UserName,
            Email = $"{request.UserName}@example.com",
            Status = "User Retrieved Successfully"
        };

        return Task.FromResult(response);
    }
}

```


### Pipeline Behaviors

Define and handle pipeline behaviors using custom behavior interfaces.

#### Example:

```csharp
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Logging Behavior Implementation.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;
        Debug.WriteLine($"Handling {requestName}");

        var response = await next();

        Debug.WriteLine($"Handled {requestName}");

        return response;
    }
}

```

#### Query Behavior Example:

```csharp
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Query Logging Behavior Implementation.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class QueryLoggingBehavior<TQuery, TResponse> : IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TQuery).Name;
        Debug.WriteLine($"Handling query {requestName}");

        var response = await next();

        Debug.WriteLine($"Handled query {requestName}");

        return response;
    }
}

```



#### Command Behavior Example:

```csharp

using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Command Logging Behavior Implementation.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CommandLoggingBehavior<TCommand, TResponse> : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TCommand).Name;
        Debug.WriteLine($"Handling command {requestName}");

        var response = await next();

        Debug.WriteLine($"Handled command {requestName}");

        return response;
    }
}


```

### Event Notification Interfaces

Define and handle event notifications using custom event notification interfaces. Event notifications allow you to publish events to multiple handlers (one-to-many pattern).


#### Example:

```
public class UserRegisteredEvent
{
    public string UserName { get; set; }
    public DateTime RegisteredAt { get; set; }

    public UserRegisteredEvent(string userName, DateTime registeredAt)
    {
        UserName = userName;
        RegisteredAt = registeredAt;
    }
}

public class UserRegisteredEventHandler : IEventNotificationHandler<UserRegisteredEvent>
{
    public Task HandleAsync(IEventNotification<UserRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        var @event = notification.Event;
        Console.WriteLine($"User registered: {@event.UserName} at {@event.RegisteredAt}");
        return Task.CompletedTask;
    }
}
```

To publish an event:
```csharp
await dispatcher.PublishAsync(new UserRegisteredEvent("alice", DateTime.UtcNow));
```
```



### Register Services

Add and configure the necessary services in your `Startup.cs` or `Program.cs` file:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register all handlers, pipeline behaviors, and event notification handlers
    services.Scan(scan => scan
        .FromAssemblyOf<Program>()
        .AddClasses(classes => classes.AssignableTo(typeof(IHandler<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        .AddClasses(classes => classes.AssignableTo(typeof(IEventNotificationHandler<>)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );

    services.AddScoped<IDispatcher, Dispatcher>();
}
```

---

## Dispatcher

The `Dispatcher` class is the core entry point for sending commands, queries, and publishing events. It uses dependency injection to resolve handlers and pipeline behaviors. The main methods are:

- `SendAsync<TCommand, TResponse>(TCommand command, ...)` — Send a command and get a response.
- `SendAsync<TCommand>(TCommand command, ...)` — Send a command with no response (void/Unit).
- `QueryAsync<TQuery, TResponse>(TQuery query, ...)` — Send a query and get a response.
- `PublishAsync<TEvent>(TEvent eventData, ...)` — Publish an event to all registered handlers.

---

For more details, see the API documentation and source code.
