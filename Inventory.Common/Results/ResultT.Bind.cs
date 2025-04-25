namespace Inventory.Common.Results;

public readonly partial record struct Result<TValue>
{
    public Result Bind(Func<TValue, Result> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ? newResult : newResult.WithErrors(bind(Value).Errors);
    }

    public async Task<Result> Bind(Func<TValue, Task<Result>> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ? newResult : newResult.WithErrors((await bind(Value)).Errors);
    }
    
    public async ValueTask<Result> Bind(Func<TValue, ValueTask<Result>> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ? newResult : newResult.WithErrors((await bind(Value)).Errors);
    }

    public Result<TNewValue> Bind<TNewValue>(Func<TValue, Result<TNewValue>> bind)
    {
        var newResult = new Result<TNewValue>().WithErrors(Errors);

        if (IsFailure)
        {
            return newResult;
        }

        var bound = bind(Value);
        return newResult
              .WithValue(bound.Value)
              .WithErrors(bound.Errors);
    }

    public Result<TNewValue> Bind<TNewValue>(Func<TValue, TNewValue> bind)
    {
        var newResult = new Result<TNewValue>().WithErrors(Errors);

        if (IsFailure)
        {
            return newResult;
        }

        return newResult
              .WithValue(bind(Value));
    }

    public async Task<Result<TNewValue>> Bind<TNewValue>(Func<TValue, Task<Result<TNewValue>>> bind)
    {
        var newResult = new Result<TNewValue>().WithErrors(Errors);

        if (IsFailure)
        {
            return newResult;
        }

        var bound = await bind(Value);
        return newResult
              .WithValue(bound.Value)
              .WithErrors(bound.Errors);
    }

    public async Task<Result<TNewValue>> Bind<TNewValue>(Func<TValue, Task<TNewValue>> bind)
    {
        var newResult = new Result<TNewValue>().WithErrors(Errors);

        if (IsFailure)
        {
            return newResult;
        }

        return newResult
           .WithValue(await bind(Value));
    }
    
    public async ValueTask<Result<TNewValue>> Bind<TNewValue>(Func<TValue, ValueTask<Result<TNewValue>>> bind)
    {
        var newResult = new Result<TNewValue>().WithErrors(Errors);

        if (IsFailure)
        {
            return newResult;
        }

        var bound = await bind(Value);
        return newResult
              .WithValue(bound.Value)
              .WithErrors(bound.Errors);
    }
}