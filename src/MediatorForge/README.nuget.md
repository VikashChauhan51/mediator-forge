# MediatorForge

## Overview

`MediatorForge` is a lightweight, dependency injection‑friendly library for implementing CQRS and event‑driven patterns in .NET applications. It provides:

- **Command & Query dispatching** with pipeline behaviors (logging, validation, etc.)
- **Event publishing** to multiple handlers
- **Separate dispatcher interfaces** for commands, queries, and events, plus a combined interface
- **Assembly scanning** for automatic registration of handlers and behaviors

## Features

- **Command and Query Handling** – `ICommand<TResponse>` / `IQuery<TResponse>` with dedicated handlers.
- **Event Notifications** – `IEventNotification<TEvent>` and `IEventNotificationHandler<TEvent>` for pub/sub.
- **Pipeline Behaviors** – Chainable behaviors (`IPipelineBehavior<TRequest, TResponse>`) for cross‑cutting concerns.
- **Multiple Dispatcher Abstractions** – `ICommandDispatcher`, `IQueryDispatcher`, `IEventDispatcher`, and combined `IDispatcher`.
- **Assembly Scanner** – Auto‑register handlers and behaviors from any assembly.

## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge
```

## Usage

### 1. Define Your Messages

#### Command (with response)

```csharp
public class CreateUserCommand : ICommand<UserResponse>
{
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class UserResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
}
```

#### Command (void / no response)

```csharp
public class DeleteUserCommand : ICommand
{
    public Guid UserId { get; set; }
}
```

#### Query

```csharp
public class GetUserQuery : IQuery<UserResponse>
{
    public string UserName { get; set; }
}
```

#### Event

```csharp
public class UserRegisteredEvent
{
    public string UserName { get; set; }
    public DateTime RegisteredAt { get; set; }
}
```

### 2. Implement Handlers

#### Command Handler (with response)

```csharp
public class CreateUserCommandHandler : IHandler<CreateUserCommand, UserResponse>
{
    public Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken ct)
    {
        var response = new UserResponse
        {
            UserName = command.UserName,
            Email = command.Email,
            Status = "User Created Successfully"
        };
        return Task.FromResult(response);
    }
}
```

#### Command Handler (void)

```csharp
public class DeleteUserCommandHandler : IHandler<DeleteUserCommand, Unit>
{
    public Task<Unit> HandleAsync(DeleteUserCommand command, CancellationToken ct)
    {
        // deletion logic
        return Task.FromResult(Unit.Value);
    }
}
```

#### Query Handler

```csharp
public class GetUserQueryHandler : IHandler<GetUserQuery, UserResponse>
{
    public Task<UserResponse> HandleAsync(GetUserQuery query, CancellationToken ct)
    {
        var response = new UserResponse
        {
            UserName = query.UserName,
            Email = $"{query.UserName}@example.com",
            Status = "User Retrieved Successfully"
        };
        return Task.FromResult(response);
    }
}
```

#### Event Handler

```csharp
public class UserRegisteredEventHandler : IEventNotificationHandler<UserRegisteredEvent>
{
    public Task HandleAsync(IEventNotification<UserRegisteredEvent> notification, CancellationToken ct)
    {
        var eventData = notification.Event;
        Console.WriteLine($"User registered: {eventData.UserName} at {eventData.RegisteredAt}");
        return Task.CompletedTask;
    }
}
```

### 3. Pipeline Behaviors (Optional)

Behaviors wrap the handler execution. They implement `IPipelineBehavior<TRequest, TResponse>` and are executed in the order they are registered (last registered runs first, wrapping the innermost handler).

#### Example: Logging Behavior

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        Debug.WriteLine($"Handling {requestName}");

        var response = await next();

        Debug.WriteLine($"Handled {requestName}");
        return response;
    }
}
```

### 4. Dependency Injection Setup

In your `Startup` or `Program`:

```csharp
using MediatorForge;
using MediatorForge.Abstractions;

var services = new ServiceCollection();

// Add core dispatcher (combined)
services.AddCqrs();

// Register handlers (commands, queries) from an assembly
services.AddHandlersFromAssemblyContaining<CreateUserCommandHandler>();

// Register event handlers from an assembly
services.AddEventHandlersFromAssemblyContaining<UserRegisteredEventHandler>();

// Register pipeline behaviors from an assembly
services.AddPipelineBehaviorsFromAssemblyContaining<LoggingBehavior<,>>();

// Build provider
var serviceProvider = services.BuildServiceProvider();
```

> **Note:** The `AddCqrs()` method registers the `Dispatcher` as `IDispatcher`, `ICommandDispatcher`, `IQueryDispatcher`, and `IEventDispatcher` simultaneously. You can inject any of these interfaces.

### 5. Dispatching

#### Using the combined `IDispatcher`

```csharp
public class SomeService
{
    private readonly IDispatcher _dispatcher;

    public SomeService(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task DoWork()
    {
        // Send command with response
        var user = await _dispatcher.SendAsync<CreateUserCommand, UserResponse>(
            new CreateUserCommand { UserName = "john", Email = "john@example.com" });

        // Send void command
        await _dispatcher.SendAsync(new DeleteUserCommand { UserId = Guid.NewGuid() });

        // Query
        var retrieved = await _dispatcher.QueryAsync<GetUserQuery, UserResponse>(
            new GetUserQuery { UserName = "john" });

        // Publish event
        await _dispatcher.PublishAsync(new UserRegisteredEvent("john", DateTime.UtcNow));
    }
}
```

#### Using segregated dispatchers (better separation of concerns)

```csharp
public class CommandService
{
    private readonly ICommandDispatcher _commandDispatcher;
    public CommandService(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;
    // only SendAsync methods visible
}

public class QueryService
{
    private readonly IQueryDispatcher _queryDispatcher;
    public QueryService(IQueryDispatcher queryDispatcher) => _queryDispatcher = queryDispatcher;
    // only QueryAsync visible
}

public class EventService
{
    private readonly IEventDispatcher _eventDispatcher;
    public EventService(IEventDispatcher eventDispatcher) => _eventDispatcher = eventDispatcher;
    // only PublishAsync visible
}
```

## Dispatcher Interfaces

| Interface | Methods | Purpose |
|-----------|---------|---------|
| `ICommandDispatcher` | `SendAsync<TCommand, TResponse>`<br/>`SendAsync<TCommand>` | One‑to‑one command handling |
| `IQueryDispatcher` | `QueryAsync<TQuery, TResponse>` | One‑to‑one query handling |
| `IEventDispatcher` | `PublishAsync<TEvent>` | One‑to‑many event notification |
| `IDispatcher` | All of the above | Combined convenience |

## How It Works

- The `Dispatcher` class resolves the appropriate handler from the DI container.
- Pipeline behaviors are chained in reverse registration order (last registered wraps the handler).
- For events, all handlers are invoked sequentially (order can be controlled by registration order).
- Commands and queries follow a one‑to‑one pattern; events follow a one‑to‑many pattern.

## Advanced Configuration

### Changing Handler/Behavior Lifetimes

By default, `AddHandlersFromAssemblyContaining`, `AddEventHandlersFromAssemblyContaining`, and `AddPipelineBehaviorsFromAssemblyContaining` register services as **Transient**. You can change the lifetime:

```csharp
services.AddHandlersFromAssemblyContaining<MyHandler>(ServiceLifetime.Scoped);
services.AddEventHandlersFromAssemblyContaining<MyEventHandler>(ServiceLifetime.Singleton);
```

### Sequential vs. Parallel Event Handling

The default `Dispatcher.PublishAsync` processes event handlers **sequentially** (one after another). If you prefer parallel execution, modify the method:

```csharp
var tasks = handlers.Select(h => h.HandleAsync(notification, ct));
await Task.WhenAll(tasks);
```

## License

This project is licensed under the MIT License.
