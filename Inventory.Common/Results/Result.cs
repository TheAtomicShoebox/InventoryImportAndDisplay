namespace Inventory.Common.Results;

public readonly partial record struct Result
{
    public Result()
    {
        Errors = [];
    }

    private Result(Errors? errors)
    {
        Errors = errors ?? [];
    }
    
    public Errors Errors { get; private init; }
    public bool IsSuccess => !IsFailure;
    public bool IsFailure => Errors.Count != 0;

    public static Result CreateResult() => new();
    public static Result Success() => new();
    public static Result Failure(Errors errors) => new(errors);
    public static Result Failure(Error error) => new(error);
    public static Result Failure(string code, string description, ErrorType errorType) => new Error(code, description, errorType);

    public static Result<TValue> CreateResult<TValue>(TValue value) => Result<TValue>.Success(value);
    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);
    public static Result<TValue> Failure<TValue>(Errors errors) => Result<TValue>.Failure(errors);
    public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.Failure(error);
    public static Result<TValue> Failure<TValue>(string code, string description, ErrorType errorType) => Result<TValue>.Failure(code, description, errorType);

    public TValue Match<TValue>(Func<TValue> onSuccess, Func<Errors, TValue> onFailure) =>
        IsSuccess ?
            onSuccess() :
            onFailure(Errors);

    public void Match(Action onSuccess, Action<Errors> onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(Errors);
        }
    }

    public Result<TValue> Map<TValue>(Func<TValue> map) =>
        IsSuccess ?
            Success(map()) :
            Failure(Errors);

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>A <see cref="Result"/> (without a value)</returns>
    public Result Tap(Action action)
    {
        if (IsSuccess)
        {
            action();
        }

        return this;
    }

    public Result WithError(Error error)
    {
        Errors.Add(error);
        return this;
    }

    public Result WithErrors(IEnumerable<Error> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    public Result<TNewValue> ToResult<TNewValue>(TNewValue newValue)
    {
        var newResult = new Result<TNewValue>();

        newResult = IsSuccess ?
            newResult.WithValue(newValue) :
            newResult.WithErrors(Errors);

        return newResult;
    }
    

    public static Result Merge(params Result[] results)
    {
        return ResultHelper.Merge(results);
    }

    public static Result<IEnumerable<TValue>> Merge<TValue>(params IEnumerable<Result<TValue>> results)
    {
        return ResultHelper.Merge(results);
    }

    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }

    public static implicit operator Result(Errors errors)
    {
        return Failure(errors);
    }
}