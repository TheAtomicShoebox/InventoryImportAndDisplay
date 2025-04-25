namespace Inventory.Common.Results;

public static partial class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task{Result}"/> (without a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="Task"/> that produces a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result"/>.</returns>
    public static async Task<Result> Bind<TOld>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, Task<Result>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task{Result}"/> (without a value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result"/>.</returns>
    public static async Task<Result> Bind<TOld>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, Result> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    // FYI: This is left commented out. ValueTask should only be used if performance concerns determine it necessary
    //  See: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=net-9.0
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="ValueTask{Result}"/> (without a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The value task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="ValueTask"/> that produces a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result"/>.</returns>
    public static async ValueTask<Result> Bind<TOld>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld, ValueTask<Result>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }

    // FYI: This is left commented out. ValueTask should only be used if performance concerns determine it necessary
    //  See: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=net-9.0
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="ValueTask{Result}"/> (without a value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The value task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result"/>.</returns>
    public static async ValueTask<Result> Bind<TOld>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld, Result> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async Task<Result<TNew>> Bind<TOld, TNew>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, Task<Result<TNew>>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async Task<Result<TNew>> Bind<TOld, TNew>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, Result<TNew>> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new value of type <see cref="TNew"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    /// <remarks>This method assumes that the <paramref name="bind" /> parameter is always returning a successful result,
    /// thus, it cannot be used in contexts where the result of the bind function may fail. The compiler should protect against this
    /// </remarks>
    public static async Task<Result<TNew>> Bind<TOld, TNew>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, TNew> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="ValueTask"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new value of type <see cref="TNew"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    /// <remarks>This method assumes that the <paramref name="bind" /> parameter is always returning a successful result,
    /// thus, it cannot be used in contexts where the result of the bind function may fail. The compiler should protect against this
    /// </remarks>
    public static async ValueTask<Result<TNew>> Bind<TOld, TNew>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld, TNew> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="Task"/> that produces a new value of type <see cref="TNew"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    /// <remarks>This method assumes that the <paramref name="bind"    /> parameter is always returning a successful result,
    /// thus, it cannot be used in contexts where the result of the bind function may fail. The compiler should protect against this
    /// </remarks>
    public static async Task<Result<TNew>> Bind<TOld, TNew>(
        this Task<Result<TOld>> resultTask,
        Func<TOld, Task<TNew>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="ValueTask"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="Task"/> that produces a new value of type <see cref="TNew"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    /// <remarks>This method assumes that the <paramref name="bind"    /> parameter is always returning a successful result,
    /// thus, it cannot be used in contexts where the result of the bind function may fail. The compiler should protect against this
    /// </remarks>
    public static async ValueTask<Result<TNew>> Bind<TOld, TNew>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld, Task<TNew>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
    
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    /// <see cref="ValueTask"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The value task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async ValueTask<Result<TNew>> Bind<TOld, TNew>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld?, ValueTask<Result<TNew>>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result{TOld}"/> (with a value) to a
    ///<see cref="Task"/> producing a <see cref="Result{TNew}"/> (with another value) that may fail.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(r => NextMethod());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TOld">The type of the value in the original <see cref="Result"/>.</typeparam>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The value task that produces the original <see cref="Result{TOld}"/>.</param>
    /// <param name="bind">A function that takes the value from the original <see cref="Result"/>
    /// and returns a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async ValueTask<Result<TNew>> Bind<TOld, TNew>(
        this ValueTask<Result<TOld>> resultTask,
        Func<TOld?, Result<TNew>> bind
    )
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result"/> (without a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(() => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The task that produces the original <see cref="Result"/>.</param>
    /// <param name="bind">A function that returns a <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async Task<Result<TNew>> Bind<TNew>(
        this Task<Result> resultTask,
        Func<Task<Result<TNew>>> bind
        )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result"/> (without a value) to a
    /// <see cref="Task"/> producing a <see cref="Result{TNew}"/> (with a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(() => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="TNew">The type of the value in the new <see cref="Result"/>.</typeparam>
    /// <param name="resultTask">The value task that produces the original <see cref="Result"/>.</param>
    /// <param name="bind">A function that returns a <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result{TNew}"/>.</returns>
    public static async ValueTask<Result<TNew>> Bind<TNew>(
        this ValueTask<Result> resultTask,
        Func<ValueTask<Result<TNew>>> bind
    )
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }

    /// <summary>
    /// Converts a <see cref="Task"/> producing a <see cref="Result"/> (without a value) to a
    /// <see cref="Task{Result}"/> (without a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(() => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="resultTask">The task that produces the original <see cref="Result"/>.</param>
    /// <param name="bind">A function that returns a <see cref="Task"/> that produces a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="Task"/> that produces a new <see cref="Result"/>.</returns>
    public static async Task<Result> Bind(this Task<Result> resultTask, Func<Task<Result>> bind)
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
    
    /// <summary>
    /// Converts a <see cref="ValueTask"/> producing a <see cref="Result"/> (without a value) to a
    /// <see cref="ValueTask{Result}"/> (without a value) that may fail asynchronously.
    /// <example>
    /// <code>
    /// var finalResult = await AsyncCallWhichMayFail()
    ///                         .Bind(() => AsyncCallWhichMayFail2());
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="resultTask">The value task that produces the original <see cref="Result"/>.</param>
    /// <param name="bind">A function that returns a <see cref="ValueTask"/> that produces a new <see cref="Result"/>.</param>
    /// <returns>A <see cref="ValueTask"/> that produces a new <see cref="Result"/>.</returns>
    public static async ValueTask<Result> Bind(this ValueTask<Result> resultTask, Func<ValueTask<Result>> bind)
    {
        var result = await resultTask;
        return await result.Bind(bind);
    }
}