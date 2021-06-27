using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core.Store
{
    /// <summary>
    /// Data store for persisting rate limiting records
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRateLimitStore<T>
    {
        /// <summary>
        /// Returns a record by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> GetAsync(string id, CancellationToken cancellationToken = default);       

        /// <summary>
        ///  Create or update a record with optional expiry 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="expiry"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveAsync(string id, T item, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    }
}
