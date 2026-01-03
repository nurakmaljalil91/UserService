#nullable enable
namespace Application.Common.Interfaces;

/// <summary>
/// Provides methods for interacting with a Redis cache.
/// </summary>
public interface IRedisCacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The cached value, or null if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Gets a cached value by key, or sets it using the provided function if not found.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="func">A function to generate the value if not found.</param>
    /// <param name="expiry">The expiration time for the cache entry.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The cached or newly set value.</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiry">The expiration time for the cache entry.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a value from the cache by key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a cache entry exists for the given key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the cache entry exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes cache entries matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match cache keys.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}
#nullable restore
