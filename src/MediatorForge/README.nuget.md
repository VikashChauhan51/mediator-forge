# MediatorForge

## Overview

`MediatorForge` is a robust library for integrating validation, authorization, logging, and mediator behaviors in .NET applications. It streamlines the implementation of common application patterns, improving readability and maintainability.

## Features

- **Command and Query Handling**: Simplify the handling of commands and queries using custom interfaces.
- **Event Notifications**: Manage event-driven architectures effectively with custom event notification interfaces.
- **Pipeline Behaviors**: Simplify the handling of commands and queries behaviors using custom interfaces.


## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge
```

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

### Pipeline Behaviors Interfaces

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

Define and handle event notifications using custom event notification interfaces.

#### Example:

```csharp

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


public class UserRegisteredEventNotification : IEventNotification<UserRegisteredEvent>
{
    public UserRegisteredEventNotification(UserRegisteredEvent @event)
    {
        Event = @event;
    }

    public UserRegisteredEvent Event { get; }
}


public class UserRegisteredEventHandler : IEventNotificationHandler<UserRegisteredEvent>
{
    public Task Handle(IEventNotification<UserRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        var @event = notification.Event;
        Console.WriteLine($"User registered: {@event.UserName} at {@event.RegisteredAt}");
        return Task.CompletedTask;
    }
}

```

### License

This project is licensed under the MIT License.
