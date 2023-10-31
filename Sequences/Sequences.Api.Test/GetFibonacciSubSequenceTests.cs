using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sequences.Api.Controllers;
using Sequences.Contract;
using Sequences.Service;

namespace Sequences.Api.Test
{
    public class GetFibonacciSubSequenceAsync
    {
        private Mock<ILogger<FibonacciController>> _loggerMock;
        private Mock<IFibonacciService> _fibonacciServiceMock;
        private Mock<IMemoryChecker> _memoryCheckerMock;
        private FibonacciController sut;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<FibonacciController>>();
            _fibonacciServiceMock = new Mock<IFibonacciService>();
            _memoryCheckerMock = new Mock<IMemoryChecker>();

            sut = new FibonacciController(_loggerMock.Object, _fibonacciServiceMock.Object, _memoryCheckerMock.Object);
        }

        [Test]
        public async Task WhenStartAndEndIndexProvidedShouldReturnSubSequence()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });


            var query = GetTestQuery();
            query.StartIndex = 1;
            query.EndIndex = 4;
            var result = await sut.GetFibonacciSubSequenceAsync(query);

            Assert.IsInstanceOf<ActionResult<FibonacciSubSequence>>(result);
            Assert.AreEqual(4, result.Value.Elements.Count);
        }

        [Test]
        public async Task GetAllItemsShouldReturnFullSequence()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });

            var result = await sut.GetFibonacciSubSequenceAsync(GetTestQuery());

            Assert.IsInstanceOf<ActionResult<FibonacciSubSequence>>(result);
            Assert.AreEqual(6, result.Value.Elements.Count);
        }


        //Validations
        [Test]
        public async Task WrongStartIndexProvidedShouldReturnError()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });

            var query = GetTestQuery();
            query.StartIndex = -1;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.GetFibonacciSubSequenceAsync(query));
        }

        [Test]
        public async Task WrongEndIndexProvidedShouldReturnError()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });

            var query = GetTestQuery();
            query.EndIndex = -11;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.GetFibonacciSubSequenceAsync(query));
        }

        [Test]
        public async Task WrongExecutionTimeLimitProvidedShouldReturnError()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });

            var query = GetTestQuery();
            query.ExecutionTimeLimitInMilliseconds = -100;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.GetFibonacciSubSequenceAsync(query));
        }

        [Test]
        public async Task SmallExecutionTimeLimitProvidedShouldReturnPartialSequence()
        {
            _fibonacciServiceMock.Setup(x => x.GetCurrentSequence()).Returns(new Sequence
            {
                Elements = new List<ulong>
                {
                    0, 1, 1, 2, 3, 5, 8
                }
            });

            var query = GetTestQuery();
            query.ExecutionTimeLimitInMilliseconds = 100;
            var result = await sut.GetFibonacciSubSequenceAsync(GetTestQuery());

            Assert.IsInstanceOf<ActionResult<FibonacciSubSequence>>(result);
            Assert.AreEqual(6, result.Value.Elements.Count);
        }


        private static FibonacciQuery GetTestQuery()
        {
            return new FibonacciQuery
            {
                StartIndex = 0,
                EndIndex = 6,
                ExecutionTimeLimitInMilliseconds = 15000,
                MemoryLimitInBytes = 100000000,
                UseCache = false
            };
        }
    }
}