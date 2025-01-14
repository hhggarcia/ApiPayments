using Microsoft.Extensions.Caching.Memory;

namespace BncPayments.Services
{
    public class WorkingKeyServices
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WorkingKeyServices> _logger;
        private const string CacheWorkingKey = "";
        public WorkingKeyServices(IMemoryCache memoryCache, ILogger<WorkingKeyServices> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public void SetWorkingKey(string workingKey)
        {
            if (string.IsNullOrEmpty(workingKey))
            {
                _logger.LogWarning("Attempted to set an empty or null working key.");
                return;
            }

            _logger.LogInformation("Setting working key in cache.");
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(24)) // Expiración absoluta de 24 horas
                .SetSlidingExpiration(TimeSpan.FromHours(12)); // Expiración deslizante de 12 horas

            _memoryCache.Set(CacheWorkingKey, workingKey, cacheEntryOptions);
        }

        public string GetWorkingKey()
        {
            _logger.LogInformation("Getting working key from cache.");
            if (!_memoryCache.TryGetValue(CacheWorkingKey, out string workingKey))
            {
                _logger.LogWarning("Working key not found in cache.");
            }
            return workingKey;
        }
    }
}
