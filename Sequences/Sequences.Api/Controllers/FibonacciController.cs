using Microsoft.AspNetCore.Mvc;
using Sequences.Contract;
using Sequences.Exception;
using Sequences.Service;

namespace Sequences.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly ILogger<FibonacciController> _logger;
        private readonly IFibonacciService _fibonacciService;
        private readonly IMemoryChecker _memoryChecker;
        private CancellationTokenSource _cancellationTokenSource;

        public FibonacciController(
            ILogger<FibonacciController> logger,
            IFibonacciService fibonacciService,
            IMemoryChecker memoryChecker)
        {
            // validate
            _logger = logger;
            _fibonacciService = fibonacciService;
            _memoryChecker = memoryChecker;
        }

        [HttpPost]
        [Route("subsequence")]
        public async Task<ActionResult<FibonacciSubSequence>> GetFibonacciSubSequenceAsync([FromBody] FibonacciQuery query)
        {
            FibonacciSubSequence subSequence;
            bool isSequenceCompleted = false;
            _cancellationTokenSource = new CancellationTokenSource();

            _logger.LogInformation("Process started");
            try
            {
                List<Task> tasks = new List<Task>
                {
                    _memoryChecker.StartAsync(_cancellationTokenSource,
                        query.MemoryLimitInBytes),
                    _fibonacciService.GenerateAsync(query.ToCriteria(),
                        _cancellationTokenSource.Token)
                };

                _cancellationTokenSource.CancelAfter(query.ExecutionTimeLimitInMilliseconds);
                Task.WaitAny(tasks.ToArray());

                _cancellationTokenSource.Cancel(true);
                isSequenceCompleted = true;
            }
            catch (ProcessTimeOutException ex)
            {
                isSequenceCompleted = false;
                _logger.LogError($"{ex.Message}");
            }
            catch (ProcessMemoryOutException ex)
            {
                isSequenceCompleted = false;
                _logger.LogError($"{ex.Message}");
            }
            finally
            {
                subSequence = _fibonacciService.GetCurrentSequence()
                    .ToFibonacciSubSequence(query.StartIndex, query.EndIndex);
                if (_cancellationTokenSource.IsCancellationRequested && !isSequenceCompleted)
                {
                    subSequence.ErrorMessage = "Sequence generated partially due to timeout error";
                }

                _cancellationTokenSource.Dispose();
            }

            return subSequence;
        }
    }
}
