using Microsoft.Extensions.Logging;
using Sequences.Exception;

namespace Sequences.Service
{
    public interface IFibonacciService
    {
        Sequence GetCurrentSequence();
        Task GenerateAsync(Criteria criteria, CancellationToken cancellationToken);
    }

    public class FibonacciService : IFibonacciService
    {
        private readonly ILogger<FibonacciService> _logger;
        private readonly IFibonacciCacheService _fibonacciCacheService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Sequence _fibonacciList;

        public FibonacciService(
            ILogger<FibonacciService> logger,
            IFibonacciCacheService fibonacciCacheService,
            IConfigurationProvider configurationProvider)
        {
            _logger = logger;
            _fibonacciCacheService = fibonacciCacheService;
            _configurationProvider = configurationProvider;
            _fibonacciList = new Sequence
            {
                Elements = new List<ulong>()
            };
        }

        public Sequence GetCurrentSequence()
        {
            return _fibonacciList;
        }

        public async Task GenerateAsync(Criteria criteria, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new ProcessTimeOutException("Operation cancelled");
            }

            if (criteria.UseCache)
            {
                _logger.LogInformation("Generating fibonacci sequence using cache");
                await GenerateUsingCacheAsync(criteria.EndIndex, cancellationToken,criteria);
            }
            else
            {
                _logger.LogInformation("Generating fibonacci sequence without using cache");
                await GenerateAsync(criteria.EndIndex, cancellationToken);
            }
            
        }

        private async Task GenerateAsync(int endIndex, CancellationToken cancellationToken)
        {

            if (cancellationToken.IsCancellationRequested)
            {
                throw new ProcessTimeOutException("Operation cancelled");
            }

            while (endIndex >= 0)
            {
                await GenerateAsync(endIndex - 1, cancellationToken);

                if (endIndex == 0 || endIndex == 1)
                {
                    _fibonacciList.Elements.Add((ulong)endIndex);
                    await DelayInStep(endIndex);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new ProcessTimeOutException("Operation cancelled");
                    }
                    return;
                }

                _fibonacciList.Elements.Add(_fibonacciList.Elements[endIndex - 1] + _fibonacciList.Elements[endIndex - 2]);
                await DelayInStep(endIndex);

                return;
            }
        }

        private async Task GenerateUsingCacheAsync(int endIndex, CancellationToken cancellationToken, Criteria criteria)
        {
            var sequenceInCache = _fibonacciCacheService.GetSequence();
            if (sequenceInCache == null || sequenceInCache.Elements.Count == 0)
            {
                await GenerateAsync(endIndex, cancellationToken);
            }
            else if(endIndex <= sequenceInCache.Elements.Count)
            {
                //if (cancellationToken.IsCancellationRequested)
                //{
                //    throw new ProcessTimeOutException("Operation time out");
                //}

                _fibonacciList.Elements.AddRange(sequenceInCache.Elements);
            }
            else
            {
                _fibonacciList.Elements.AddRange(sequenceInCache.Elements);

                for (int i = sequenceInCache.Elements.Count; i <= endIndex; i++)
                {
                    var next = _fibonacciList.Elements[i-1]+ _fibonacciList.Elements[i - 2];
                    _fibonacciList.Elements.Add(next);
                    await DelayInStep(i);
                }
            }
            
            _fibonacciCacheService.SetSequence(_fibonacciList);
        }

        private async Task DelayInStep(int endIndex)
        {
            var delay = _configurationProvider.GenerationTimeOfEachFibonacciNumber();
            _logger.LogInformation($"Delaying {delay} millisecond for step {endIndex}");
            await Task.Delay(delay);
        }
    }
}