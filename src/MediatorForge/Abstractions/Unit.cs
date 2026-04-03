
namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a unit type that can be used to indicate the absence of a value or a void return type in asynchronous operations.
/// </summary>
public readonly record struct Unit
{
    /// <summary>
    /// A singleton instance of the Unit type. This can be used to represent a void return type in asynchronous operations, allowing for a consistent way to indicate the absence of a value across different handlers and dispatchers.
    /// </summary>
    public static readonly Unit Value = new();

    /// <summary>
    /// A task that represents the completion of an asynchronous operation that returns a Unit value. This can be used in command handlers that do not return a response, allowing for a consistent way to indicate the completion of the operation without returning a value.
    ///  The Task property provides a convenient way to return a completed task with the Unit value, which can be used in command handlers that do not expect a response.
    /// </summary>
    public static Task<Unit> Task => System.Threading.Tasks.Task.FromResult(Value);
}
