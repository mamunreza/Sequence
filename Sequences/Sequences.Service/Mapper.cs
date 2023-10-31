using Sequences.Contract;

namespace Sequences.Service
{
    public static class Mapper
    {
        public static Criteria ToCriteria(this FibonacciQuery query)
        {
            return new Criteria
            {
                StartIndex = query.StartIndex,
                EndIndex = query.EndIndex,
                UseCache = query.UseCache,
                ExecutionTimeLimitInMilliseconds = query.ExecutionTimeLimitInMilliseconds,
                MemoryLimitInBytes = query.MemoryLimitInBytes
            };
        }

        public static FibonacciSubSequence ToFibonacciSubSequence(this Sequence sequence, int startIndex, int endIndex)
        {
            if (sequence == null)
            {
                return GetBlankFibonacciSubSequence();
            }
            var elementCount = sequence.Elements.Count < endIndex ? sequence.Elements.Count : endIndex;

            return new FibonacciSubSequence
            {
                Elements = sequence.Elements.Any()
                     ? sequence.Elements.GetRange(startIndex, elementCount)
                    : new List<ulong>()
            };
        }

        public static FibonacciSubSequence ToFibonacciSubSequence(this Sequence sequence, Criteria criteria)
        {
            return new FibonacciSubSequence
            {
                Elements = sequence.Elements.GetRange(criteria.StartIndex, criteria.EndIndex - criteria.StartIndex + 1)
            };
        }

        private static FibonacciSubSequence GetBlankFibonacciSubSequence()
        {
            return new FibonacciSubSequence
            {
                Elements = new List<ulong>(),
                ErrorMessage = null
            };
        }
    }
}
