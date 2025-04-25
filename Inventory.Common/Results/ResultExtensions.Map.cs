namespace Inventory.Common.Results;

public static partial class ResultExtensions
{
    /// <summary>
    /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
    /// </summary>
    public static async Task<Result<TNewValue>> Map<TOldValue, TNewValue>(this Task<Result<TOldValue>> resultTask, Func<TOldValue, TNewValue> valueConverter)
    {
        var result = await resultTask;
        return result.Map(valueConverter);
    }

    // FYI: This is left commented out. ValueTask should only be used if performance concerns determine it necessary
    //  See: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=net-9.0
    
    /// <summary>
    /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
    /// </summary>
    public static async ValueTask<Result<TNewValue>> Map<TOldValue, TNewValue>(this ValueTask<Result<TOldValue>> resultTask, Func<TOldValue, TNewValue> valueConverter)
    {
        var result = await resultTask;
        return result.Map(valueConverter);
    }
}