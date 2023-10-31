namespace Sequences.Service;

public class Criteria
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public bool UseCache { get; set; }
    public int ExecutionTimeLimitInMilliseconds { get; set; }
    public int MemoryLimitInBytes { get; set; }
}