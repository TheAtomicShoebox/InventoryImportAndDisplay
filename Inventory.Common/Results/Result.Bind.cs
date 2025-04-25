namespace Inventory.Common.Results;

public readonly partial record struct Result
{
    public Result Bind(Func<Result> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ?
            newResult :
            newResult.WithErrors(bind().Errors);
    }

    public async Task<Result> Bind(Func<Task<Result>> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ?
            newResult :
            newResult.WithErrors((await bind()).Errors);
    }
    
    public async ValueTask<Result> Bind(Func<ValueTask<Result>> bind)
    {
        var newResult = new Result().WithErrors(Errors);

        return IsFailure ?
            newResult :
            newResult.WithErrors((await bind()).Errors);
    }

    public Result<TValue> Bind<TValue>(Func<Result<TValue>> bind)
    {
        var newResult = new Result<TValue>().WithErrors(Errors);

        var bound = bind();
        return IsFailure ?
            newResult :
            newResult.WithValue(bound.Value).WithErrors(bound.Errors);
    }

    public async Task<Result<TValue>> Bind<TValue>(Func<Task<Result<TValue>>> bind)
    {
        var newResult = new Result<TValue>().WithErrors(Errors);
        if (IsFailure)
        {
            return newResult;
        }

        var bound = await bind();
        return newResult.WithValue(bound.Value).WithErrors(bound.Errors);
    }
    
    public async ValueTask<Result<TValue>> Bind<TValue>(Func<ValueTask<Result<TValue>>> bind)
    {
        var newResult = new Result<TValue>().WithErrors(Errors);
        if (IsFailure)
        {
            return newResult;
        }

        var bound = await bind();
        newResult.WithValue(bound.Value).WithErrors(bound.Errors);

        return newResult;
    }
}