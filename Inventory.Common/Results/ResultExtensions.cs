namespace Inventory.Common.Results;

public static partial class ResultExtensions
{
    /// <summary>
    /// Convert <see cref="Result"/> without value to a <see cref="Result{TValue}"/> containing a value
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <param name="resultTask">The current <see cref="Result"/>></param>
    /// <param name="value">Value to add to the new result</param>
    public static async Task<Result<TValue>> ToResult<TValue>(this Task<Result> resultTask, TValue value)
    {
        var result = await resultTask;
        return result.ToResult(value);
    }

    // FYI: This is left commented out. ValueTask should only be used if performance concerns determine it necessary
    //  See: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=net-9.0
    /*
    /// <summary>
    /// Convert <see cref="Result"/> without value to a <see cref="Result{TValue}"/> containing a value
    /// </summary>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <param name="resultTask">The current <see cref="Result"/>></param>
    /// <param name="value">Value to add to the new result</param>
    public static async Task<Result<TValue>> ToResult<TValue>(this ValueTask<Result> resultTask, TValue value)
    {
        var result = await resultTask;
        return result.ToResult(value);
    }*/

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="resultTask">A <see cref="Task"/> producing the existing <see cref="Result"/></param>
    /// <param name="action">The action to take</param>
    /// <returns>A <see cref="Task"/> producing the <see cref="Result"/></returns>
    public static async Task<Result> Tap(
        this Task<Result> resultTask,
        Action action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    
    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="resultTask">A <see cref="ValueTask"/> producing the existing <see cref="Result"/></param>
    /// <param name="action">The <see cref="Action{TValue}"/>> to take</param>
    /// <returns>A <see cref="ValueTask"/> producing the <see cref="Result"/></returns>
    public static async ValueTask<Result> Tap(
        this ValueTask<Result> resultTask,
        Action action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="resultTask">A <see cref="Task"/> producing the existing <see cref="Result{TValue}"/></param>
    /// <param name="action">The <see cref="Action{TValue}"/>> to take</param>
    /// <returns>A <see cref="Task"/> producing the <see cref="Result{TValue}"/></returns>
    public static async Task<Result<TValue>> Tap<TValue>(
        this Task<Result<TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="resultTask">A <see cref="Task"/> producing the existing <see cref="Result{TValue}"/></param>
    /// <param name="action">The <see cref="Action"/>> to take</param>
    /// <returns>A <see cref="Task"/> producing the <see cref="Result{TValue}"/></returns>
    public static async Task<Result<TValue>> Tap<TValue>(
        this Task<Result<TValue>> resultTask,
        Action action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    /// <summary>
    /// Takes an <see cref="Action"/> (side effect) and returns the existing result. <br />
    /// <b>Warning</b>: this breaks the no side effects intent of <see cref="Result"/>, use sparingly
    /// </summary>
    /// <param name="resultTask">A <see cref="ValueTask"/> producing the existing <see cref="Result{TValue}"/></param>
    /// <param name="action">The <see cref="Action{TValue}"/>> to take</param>
    /// <returns>A <see cref="ValueTask"/> producing the <see cref="Result{TValue}"/></returns>
    public static async ValueTask<Result<TValue>> Tap<TValue>(
        this ValueTask<Result<TValue>> resultTask,
        Action<TValue> action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    public static async Task<TNewValue> Match<TOldValue, TNewValue>(this Task<Result<TOldValue>> resultTask, Func<TOldValue, TNewValue> onSuccess, Func<Errors, TNewValue> onFailure)
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    public static Result<IEnumerable<TValue>> Merge<TValue>(this IEnumerable<Result<TValue>> results)
    {
        return ResultHelper.Merge(results);
    }

    public static Result<TValue> ToResult<TValue>(this TValue value) => Result<TValue>.Success(value);

    public static Task<Result<TValue>> ToTaskResult<TValue>(this TValue value) => Task.FromResult(Result<TValue>.Success(value));
}