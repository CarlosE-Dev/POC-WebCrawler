using Microsoft.Extensions.Caching.Distributed;
using POC_WebCrawler.Domain.Interfaces;

namespace POC_WebCrawler.Infra.Redis
{
    public class RedisCacheRepository : IRedisCacheRepository
    {
        private readonly IDistributedCache _cache;

        public RedisCacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> Get(string key)
        {
            return await _cache.GetStringAsync(key);
        }

        public async Task Set(string key, string jsonValue)
        {
            await _cache.SetStringAsync(key, jsonValue);
        }

        public async Task Clean(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
