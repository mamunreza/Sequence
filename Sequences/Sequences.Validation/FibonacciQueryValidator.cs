using FluentValidation;
using Sequences.Contract;

namespace Sequences.Validation
{
    public class FibonacciQueryValidator :AbstractValidator<FibonacciQuery>
    {
        public FibonacciQueryValidator()
        {
            RuleFor(x => x.StartIndex)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.EndIndex)
                .GreaterThanOrEqualTo(0)
                .GreaterThanOrEqualTo(x=>x.StartIndex);
            RuleFor(x => x.UseCache)
                .Must(x => x == false || x);
            RuleFor(x => x.ExecutionTimeLimitInMilliseconds)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(x => x.MemoryLimitInBytes)
                .NotEmpty()
                .GreaterThan(0);
            
        }
    }
}