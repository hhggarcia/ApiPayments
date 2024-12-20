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
            _logger.LogInformation("Setting working key in cache.");
            _memoryCache.Set(CacheWorkingKey, workingKey);
        }

        public string GetWorkingKey()
        {
            _logger.LogInformation("Getting working key from cache.");
            _memoryCache.TryGetValue(CacheWorkingKey, out string workingKey);
            if (workingKey == null)
            {
                _logger.LogWarning("Working key not found in cache.");
            }
            return workingKey;
        }
    }
}
