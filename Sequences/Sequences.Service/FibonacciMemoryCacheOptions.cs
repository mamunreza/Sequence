namespace Sequences.Service
{
    public class FibonacciMemoryCacheOptions
    {
        public const string FibonacciMemoryCache = "FibonaccMemoryCache";

        public int CacheExpirationSeconds { get; set; }
    }
}
