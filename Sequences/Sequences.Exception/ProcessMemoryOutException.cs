namespace Sequences.Exception
{
    public class ProcessMemoryOutException : System.Exception
    {
        public ProcessMemoryOutException()
        {
        }

        public ProcessMemoryOutException(string message) : base(message)
        {
        }

        public ProcessMemoryOutException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
