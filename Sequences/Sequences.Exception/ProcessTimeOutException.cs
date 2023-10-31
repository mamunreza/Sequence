
namespace Sequences.Exception
{
    public class ProcessTimeOutException : System.Exception
    {
        public ProcessTimeOutException()
        {
        }

        public ProcessTimeOutException(string message) : base(message)
        {
        }

        public ProcessTimeOutException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}