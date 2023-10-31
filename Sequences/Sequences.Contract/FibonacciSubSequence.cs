using System.Collections.Generic;

namespace Sequences.Contract
{
    public class FibonacciSubSequence : Response
    {
        public List<ulong> Elements { get; set; }
        public string ErrorMessage { get; set; }
    }
}
