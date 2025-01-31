namespace MediatorForge.Abstraction;

/// <summary>
/// Represents a cache interface for storing and retrieving data asynchronously.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="key">The key of the cached item.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item, or null if the item is not found.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Stores an item in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The item to cache.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value);
}
