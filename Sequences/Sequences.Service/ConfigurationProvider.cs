using Microsoft.Extensions.Options;

namespace Sequences.Service
{
    public interface IConfigurationProvider
    {
        int GenerationTimeOfEachFibonacciNumber();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly ArtificialProcessDelayOptions _options;

        public ConfigurationProvider(IOptions<ArtificialProcessDelayOptions> options)
        {
            _options = options.Value;
        }

        public int GenerationTimeOfEachFibonacciNumber()
        {
            return _options.GenerationOfEachFibonacciNumberInMilliseconds;
        }
    }
}
