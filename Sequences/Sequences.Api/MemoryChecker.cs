using Sequences.Exception;
using System.Diagnostics;

namespace Sequences.Api
{
    public interface IMemoryChecker
    {
        Task StartAsync(CancellationTokenSource cancellationTokenSource, int memoryLimit);
    }

    public class MemoryChecker : IMemoryChecker
    {
        private readonly ILogger<MemoryChecker> _logger;

        public MemoryChecker(
            ILogger<MemoryChecker> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationTokenSource cancellationTokenSource, int memoryLimit)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                throw new ProcessMemoryOutException("Operation cancelled");
            }

            await Task.Run(() => ExecuteAsync(cancellationTokenSource, memoryLimit),
                cancellationTokenSource.Token);
        }

        private async Task ExecuteAsync(CancellationTokenSource cancellationTokenSource, int memoryLimit)
        {
            try
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    throw new ProcessMemoryOutException("Operation cancelled");
                }

                _logger.LogInformation("Memory check initiated");

                while (true)
                {
                    try
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            throw new ProcessMemoryOutException("Operation cancelled");
                        }

                        ImposeHighMemoryUsage();

                        using (var currentProcess = Process.GetCurrentProcess())
                        {
                            var currentProcessWorkingSet64 = currentProcess.WorkingSet64;
                            _logger.LogInformation($"Memory check running, current memory {currentProcessWorkingSet64}");
                            if (currentProcessWorkingSet64 > memoryLimit)
                            {
                                _logger.LogInformation($"Memory limit exceeded, current memory {currentProcessWorkingSet64}");
                                cancellationTokenSource.Cancel(true);
                                throw new ProcessMemoryOutException("Memory limit exceeded");
                            }
                        }
                        
                        await Task.Delay(1000, cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogInformation("Memory check cancelled");
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError("Exception during memory check.", e);
                throw;
            }
        }

        private static void ImposeHighMemoryUsage()
        {
            var list = Enumerable.Range(0, 10000).Select(i => i.ToString()).ToList();
            foreach (var item in list)
            {
                var newItem = item + "___" + item;
            }
        }
    }
}
