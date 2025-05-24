using QuebrixClient.Response.Dtos;

namespace QuebrixClient;


/// <summary>
/// Interface for interacting with the Quebrix distributed cache system.
/// Provides functionality for authentication, CRUD operations on cache entries,
/// cluster management, and utility methods to enhance data handling across clusters.
/// </summary>
public interface IQuebrixCacheProvider
{
    /// <summary>
    /// Authenticates a user against the Quebrix API.
    /// </summary>
    /// <param name="userName">The username to authenticate.</param>
    /// <param name="password">The password for the user.</param>
    /// <returns>A <see cref="CacheResponse{T}"/> containing a token or error message.</returns>
    Task<CacheResponse<string>> AuthenticateAsync(string userName, string password);

    /// <summary>
    /// Retrieves all existing cluster names from the Quebrix cache.
    /// </summary>
    /// <returns>A list of cluster names wrapped in a <see cref="CacheResponse{T}"/>.</returns>
    Task<CacheResponse<List<string>>> GetAllClustersAsync();

    /// <summary>
    /// Clears all key-value pairs in a specified cluster.
    /// </summary>
    /// <param name="cluster">The name of the cluster to clear.</param>
    /// <returns>True if successful, otherwise false.</returns>
    Task<bool> ClearCluster(string cluster);

    /// <summary>
    /// Retrieves all keys for a specified cluster.
    /// </summary>
    /// <param name="clusterName">The cluster to fetch keys from.</param>
    /// <returns>A list of keys inside the cluster wrapped in a <see cref="CacheResponse{T}"/>.</returns>
    Task<CacheResponse<List<string>>> GetKeysOfClusterAsync(string clusterName);

    /// <summary>
    /// Ensures a cluster exists by creating it if it doesn't already.
    /// </summary>
    /// <param name="cluster">The name of the cluster to set or create.</param>
    /// <returns>True if the cluster is set successfully.</returns>
    Task<bool> SetCluster(string cluster);

    /// <summary>
    /// Gets a cached value for a given key and cluster.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="cluster">Cluster name.</param>
    /// <param name="key">Key for the cached value.</param>
    /// <returns>The cached data wrapped in a <see cref="CacheResponse{T}"/>.</returns>
    Task<CacheResponse<T>> GetAsync<T>(string cluster, string key);

    /// <summary>
    /// Sets a value in the cache with an optional TTL.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="cluster">Cluster to store the value in.</param>
    /// <param name="key">Key for the cached value.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expireTime">Optional expiration time in milliseconds.</param>
    /// <returns>True if the operation was successful.</returns>
    Task<CacheResponse<bool>> SetAsync<T>(string cluster, string key, T value, long? expireTime = null);

    /// <summary>
    /// Deletes a key-value entry from a specific cluster.
    /// </summary>
    /// <param name="cluster">Cluster name.</param>
    /// <param name="key">The key to delete.</param>
    /// <returns>True if deleted successfully.</returns>
    Task<CacheResponse<bool>> Delete(string cluster, string key);

    /// <summary>
    /// Copies all key-value pairs from one cluster to another.
    /// </summary>
    /// <param name="srcCluster">Source cluster name.</param>
    /// <param name="destCluster">Destination cluster name.</param>
    /// <returns>True if the operation was successful.</returns>
    Task<CacheResponse<bool>> CopyCluster(string srcCluster, string destCluster);
}

public interface IQuebrixEFSharder
{
   public Task<int> GetShardingKey();
}

