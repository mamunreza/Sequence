using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sequences.Service
{
    public interface IFibonacciCacheService
    {
        Sequence? GetSequence();
        void SetSequence(Sequence sequence);
    }

    public class FibonacciCacheService : IFibonacciCacheService
    {
        private readonly ILogger<FibonacciCacheService> _logger;
        private readonly IMemoryCache _cache;
        private readonly FibonacciMemoryCacheOptions _options;
        private const string CacheKey = "fibonacciCacheKey";

        public FibonacciCacheService(
            ILogger<FibonacciCacheService> logger,
            IMemoryCache cache,
            IOptions<FibonacciMemoryCacheOptions> options)
        {
            _logger = logger;
            _cache = cache;
            _options = options.Value;
        }

        public Sequence? GetSequence()
        {
            _logger.LogInformation("Getting fibonacci sequence from in memory cache");
            if (_cache.TryGetValue(CacheKey, out Sequence? sequence))
            {
                _logger.LogInformation($"Found fibonacci sequence of {sequence.Elements.Count} elements in cache");
                return sequence;
            }

            _logger.LogInformation("No fibonacci sequence found in cache");
            return null;
        }

        public void SetSequence(Sequence sequence)
        {
            _logger.LogInformation($"Setting in memory cache for fibonacci sequence up to {sequence.Elements.Count} elements");
            _cache.Remove(CacheKey);
            _cache.Set(CacheKey, sequence, GetInMemoryCacheEntryOptions());
        }

        private MemoryCacheEntryOptions GetInMemoryCacheEntryOptions() =>
            new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(_options.CacheExpirationSeconds))
                .SetPriority(CacheItemPriority.Normal);
    }
}
