using Microsoft.Extensions.Caching.Memory;

namespace BncPayments.Services
{
    public class WorkingKeyServices
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheWorkingKey = "";
        public WorkingKeyServices(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetWorkingKey(string workingKey)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(24)); // Ajusta el tiempo según tus necesidades

            _memoryCache.Set(CacheWorkingKey, workingKey, cacheEntryOptions);
        }

        public string GetWorkingKey()
        {
            _memoryCache.TryGetValue(CacheWorkingKey, out string workingKey);
            return workingKey;
        }

    }
}
