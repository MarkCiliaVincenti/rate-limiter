using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core.Lock
{
    /// <summary>
    /// Provides locks based on unique keys to prevent multiple threads from accessing same code concurrently.
    /// </summary>
    public class AsyncLocker
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locker = new ConcurrentDictionary<string, SemaphoreSlim>();
                 
        private class Unlocker : IDisposable
        {
            private readonly string _lockerKey;

            public Unlocker(string lockerKey)
            {
                _lockerKey = lockerKey;
            }

            /// <summary>
            /// This will be automatically invoked if <see cref="GetLockAsync(string)"/> is obtained with `using` statement 
            /// </summary>
            public void Dispose()
            {
                if (_locker.TryGetValue(_lockerKey, out SemaphoreSlim semaphore))
                {
                    semaphore.Release();
                }
            }
        }

        /// <summary>
        /// Obtains a lock with specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns a <see cref="IDisposable"/> to release lock</returns>
        public async Task<IDisposable> GetLockAsync(string key) 
        {
            await _locker.GetOrAdd(key, new SemaphoreSlim(1, 1)).WaitAsync();
            return new Unlocker(key);
        }        
    }
}
