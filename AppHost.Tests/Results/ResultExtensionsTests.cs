using Inventory.Common.Results;
using static AppHost.Tests.Results.TestResultDataGenerators;
using static Inventory.Common.Results.Result;

namespace AppHost.Tests.Results;

public class ResultExtensionsTests
{
    [Fact]
    public async Task Convert_Result_ToValue()
    {
        //Arrange
        var resultTask = Task.FromResult(CreateResult());

        //Act
        var actual = await resultTask.ToResult(3);
        
        //Assert
        Assert.Equal(3, actual.Value);
    }

    [Fact]
    public async Task Tap_Result_NoActionInput()
    {
        //Arrange
        var resultTask = Task.FromResult(CreateResult());

        //Act & Assert
        var actual = await resultTask.Tap(() => Assert.True(true));

        //Assert
        Assert.Equivalent(CreateResult(), actual);
    }

    [Fact]
    public async Task Tap_GenericResult_NoActionInput()
    {
        //Arrange
        var expected = Success(3);
        var resultTask = Task.FromResult(Success(3));

        //Act & Assert
        var actual = await resultTask.Tap(() => Assert.True(true));

        //Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task Tap_GenericResult()
    {
        //Arrange
        var expected = 3;
        var resultTask = Task.FromResult(Success(3));

        //Act & Assert
        var actual = await resultTask.Tap(actual => Assert.Equal(expected, actual));

        //Assert
        Assert.Equal(expected, actual.Value);
    }

    [Theory]
    [MemberData(nameof(BindTestData))]
    public async Task Bind_Task_Result_Success(Task<Result> initialResultTask, Func<Task<Result>> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindTestToGenericData))]
    public async Task Bind_Task_Result_ToGeneric<TValue>(
        Task<Result> initialResultTask,
        Func<Task<Result<TValue>>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericToSyncTestData))]
    public async Task Bind_Task_Result_GenericToSync<TValue>(
        Task<Result<TValue>> initialResultTask,
        Func<TValue, Result<TValue>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericToSyncNoValueTestData))]
    public async Task Bind_Task_Result_GenericToSyncNoValue<TValue>(
        Task<Result<TValue>> initialResultTask,
        Func<TValue, Result> bindFunc,
        bool expectedSuccess
    )
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindGenericToNoValueTestData))]
    public async Task Bind_Task_Result_GenericToNoValue<TValue>(
        Task<Result<TValue>> initialResultTask,
        Func<TValue, Task<Result>> bindFunc,
        bool expectedSuccess
    )
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindGenericTestData))]
    public async Task Bind_Task_Result_GenericTask<TValue>(Task<Result<TValue>> initialResultTask, Func<TValue, Task<Result<TValue>>> bindFunc, bool expectedSuccess, TValue expectedValue)
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericSyncTestData))]
    public async Task Bind_Task_Result_Generic_Sync<TValue>(Task<Result<TValue>> initialResultTask, Func<TValue, Result<TValue>> bindFunc, bool expectedSuccess, TValue expectedValue)
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    /*[Theory]
    [MemberData(nameof(BindValueTaskTestData))]
    public async Task Bind_ValueTask_Result_Success(ValueTask<Result> initialResultTask, Func<ValueTask<Result>> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindValueTaskTestData))]
    public async Task Bind_ValueTask_Result_Failure(ValueTask<Result> initialResultTask, Func<ValueTask<Result>> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = await initialResultTask.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }*/

    [Theory]
    [MemberData(nameof(MatchTestData))]
    public async Task Match_Task_Result<TOldValue, TNewValue>(
        Task<Result<TOldValue>> resultTask,
        Func<TOldValue, TNewValue> onSuccess,
        Func<Errors, TNewValue> onFailure,
        bool expectedSuccess,
        TNewValue expectedSuccessValue,
        TNewValue expectedFailureValue)
    {
        // Act
        var actual = await resultTask.Match(onSuccess, onFailure);

        // Assert
        if (expectedSuccess)
        {
            Assert.Equal(expectedSuccessValue, actual);
            return;
        }
        Assert.Equal(expectedFailureValue, actual);
    }

    [Theory]
    [MemberData(nameof(MapTestData))]
    public async Task Map_Task_Result<TValue, TNewValue>(
        Task<Result<TValue>> initialResultTask,
        Func<TValue, TNewValue> mapFunc,
        bool expectedSuccess,
        TNewValue expectedValue)
    {
        // Act
        var result = await initialResultTask.Map(mapFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MergeTestData))]
    public void Merge_IEnumerable_Extension(IEnumerable<Result<int>> results, Result<IEnumerable<int>> expected)
    {
        // Act
        var actual = results.Merge();

        // Assert
        Assert.Equivalent(expected, actual);
    }

    public static IEnumerable<TheoryDataRow> BindTestData()
    {
        yield return new(Task.FromResult(Success()), () => Task.FromResult(Success()), true);
        yield return new(Task.FromResult(GenerateSingleFailure(1)), () => Task.FromResult(Success()), false);
    }

    public static IEnumerable<TheoryDataRow> BindTestToGenericData()
    {
        yield return new(Task.FromResult(Success()), () => Task.FromResult(Success(42)), true, 42);
        yield return new(Task.FromResult(GenerateSingleFailure(1)), () => Task.FromResult(Success(42)), false, 42);
    }

    public static IEnumerable<TheoryDataRow> BindGenericTestData()
    {
        yield return new(Task.FromResult(Success(42)), (int value) => Task.FromResult(Success(value)), true, 42);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), (int value) => Task.FromResult(Success(value)), false, 42);
    }

    public static IEnumerable<TheoryDataRow> BindGenericToSyncTestData()
    {
        yield return new(Task.FromResult(Success(42)), Success<int>, true, 42);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), Success<int>, false, 42);
    }

    public static IEnumerable<TheoryDataRow> BindGenericToNoValueTestData()
    {
        var a = (int _) => Task.FromResult(Success());
        yield return new(Task.FromResult(Success(42)), (int _) => Task.FromResult(Success()), true);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), (int _) => Task.FromResult(Success()), false);
    }

    public static IEnumerable<TheoryDataRow> BindGenericToSyncNoValueTestData()
    {
        yield return new(Task.FromResult(Success(42)), (int _) => Success(), true);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), (int _) => Success(), false);
    }

    public static IEnumerable<TheoryDataRow> BindGenericSyncTestData()
    {
        yield return new(Task.FromResult(Success(42)), Success<int>, true, 42);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), Success<int>, false, 42);
    }

    public static IEnumerable<TheoryDataRow> BindValueTaskTestData()
    {
        yield return new(new ValueTask<Result>(Success()), GenerateSuccessValueTaskFunc(), true);
        yield return new(new ValueTask<Result>(GenerateSingleFailure(1)), GenerateSuccessValueTaskFunc(), false);
    }

    public static IEnumerable<TheoryDataRow> MapTestData()
    {
        yield return new(Task.FromResult(Success(42)), (int value) => value.ToString(), true, "42");
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), (int value) => value.ToString(), false, "42");
    }

    public static IEnumerable<TheoryDataRow> MatchTestData()
    {
        yield return new(Task.FromResult(Success(42)), (int value) => value + 1, (Errors errors) => 0, true, 43, 0);
        yield return new(Task.FromResult(GenerateSingleFailure<int>(1)), (int value) => value + 1, (Errors errors) => 42, false, 9999, 42);
    }

    public static TheoryData<IEnumerable<Result<int>>, Result<IEnumerable<int>>> MergeTestData()
    {
        var td = new TheoryData<IEnumerable<Result<int>>, Result<IEnumerable<int>>>();
        td.Add([Success(1), Success(2), Success(3)], Success<IEnumerable<int>>([1, 2, 3]));
        
        var failure = GenerateSingleFailure<int>(1);
        td.Add([ Success(1), failure, Success(3)], Failure<IEnumerable<int>>(failure.Errors).WithValue([]));

        IEnumerable<Result<int>> failures = [
            GenerateMultipleFailures<int>(1, 3),
            GenerateMultipleFailures<int>(4, 3),
            GenerateMultipleFailures<int>(7, 3)
        ];

        Errors allErrors = new(failures.SelectMany(e => e.Errors).ToList());
        
        td.Add(failures, Failure<IEnumerable<int>>(allErrors).WithValue([]));

        return td;
    }
}
