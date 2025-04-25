namespace Inventory.Common.Results;

public readonly partial record struct Result<TValue>//(TValue Value, Errors Errors)
{
    public Result()
    {
        Value = default!;
        Errors = [];
    }
    public Result(TValue value, Errors? errors)
    {
        Value = value;
        Errors = errors ?? [];
    }
    public TValue Value { get; private init; }// = Value;
    public Errors Errors { get; private init; }// = Errors;

    public bool IsSuccess => !IsFailure;
    public bool IsFailure => Errors.Count != 0;

    private Result(TValue value) : this(value, []) { }
    private Result(Errors errors) : this(default, errors) { }

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(Errors errors) => new(errors);
    public static Result<TValue> Failure(Error error) => new(error);
    public static Result<TValue> Failure(string code, string description, ErrorType errorType) => new(new Error(code, description, errorType));

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Errors, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Errors);

    public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue>? map = null)
    {
        var a = new Result<TNewValue>();
        var newResult = new Result<TNewValue>();
        if (IsSuccess)
        {
            if (map is null)
            {
                throw new ArgumentException("If result is success then valueConverter should not be null");
            }
            newResult = newResult.WithValue(map(Value));
        }

        newResult = newResult.WithErrors(Errors);

        return newResult;
    }

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>A <see cref="Result{TValue}"/> (with a value)</returns>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }

        return this;
    }

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>A <see cref="Result{TValue}"/> (with a value)</returns>
    public Result<TValue> Tap(Action action)
    {
        if (IsSuccess)
        {
            action();
        }

        return this;
    }

    public Result<TValue> WithError(Error error)
    {
        Errors.Add(error);
        return this;
    }

    public Result<TValue> WithErrors(IEnumerable<Error> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    public Result<TValue> WithValue(TValue value)
    {
        return this with
        {
            Value = value
        };
    }

    public Result<TNewValue> ToResult<TNewValue>(Func<TValue, TNewValue>? valueConverter = null)
    {
        return Map(valueConverter);
    }

    public static implicit operator Result<object>(Result<TValue> result)
    {
        return result.ToResult<object>(value => value);
    }

    public static implicit operator Result<TValue>(Result result)
    {
        return result.ToResult<TValue>(default);
    }

    public static implicit operator Result<TValue>(TValue value)
    {
        if(value is Result<TValue> r)
            return r;

        return Result.Success(value);
    }

    public static implicit operator Result<TValue>(Error error)
    {
        return Result.Failure(error);
    }

    public static implicit operator Result<TValue>(Errors errors)
    {
        return Result.Failure(errors);
    }
}