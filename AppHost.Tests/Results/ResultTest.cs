using Inventory.Common.Results;
using JetBrains.Annotations;
using static AppHost.Tests.Results.TestResultDataGenerators;
using static Inventory.Common.Results.Result;

namespace AppHost.Tests.Results;

[TestSubject(typeof(Result))]
public class ResultTest
{
    [Fact]
    public void Result_Constructor_InitializesEmptyErrors()
    {
        // Act
        var result = new Result();

        // Assert
        Assert.Empty(result.Errors);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Result_Failure_WithError_InitializesErrors()
    {
        // Arrange
        var error = new Error("Test", "Test description", ErrorType.Problem);

        // Act
        var result = Failure(error);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Result_Failure_WithErrors_InitializesErrors()
    {
        // Arrange
        Errors errors =
        [
            new("Test", "Test description", ErrorType.Problem),
            new("Test 2", "Test description 2", ErrorType.Problem)
        ];

        // Act
        var result = Failure(errors);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Result_Failure_FromCodeDescription_InitializesError()
    {
        // Act
        var result = Failure("Test", "Test description", ErrorType.Problem);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    public void GenericResult_Failure_FromCodeDescription_InitializesError()
    {
        // Act
        var result = Failure<int>("Test", "Test description", ErrorType.Problem);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Match_ReturnsCorrectValue()
    {
        // Arrange
        var successResult = Success();
        var failureResult = Failure(new("Test", "Test description", ErrorType.Problem));

        // Act
        var successValue = successResult.Match(() => "Success", errors => "Failure");
        var failureValue = failureResult.Match(() => "Success", errors => "Failure");

        // Assert
        Assert.Equal("Success", successValue);
        Assert.Equal("Failure", failureValue);
    }

    [Fact]
    public void Match_ExecutesCorrectAction()
    {
        // Arrange
        var successResult = Success();
        var failureResult = Failure(new("Test", "Test description", ErrorType.Problem));

        // Act & Assert
        successResult.Match(
            () => Assert.True(true),
            errors => Assert.Fail("This should not be hit")
        );

        failureResult.Match(
            () => Assert.Fail("This should not be hit"),
            errors => Assert.True(true)
        );
    }

    [Fact]
    public void Map_ReturnsTransformedValue()
    {
        // Arrange
        var result = Success();
        const string expected = "Mapped value";

        // Act
        var mappedResult = result.Map(() => expected);

        // Assert
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(expected, mappedResult.Value);
    }

    [Fact]
    public void Tap_ReturnsSameResult()
    {
        // Arrange
        var result = Success();

        // Act
        var tappedResult = result.Tap(() => { });

        // Assert
        Assert.Equal(result, tappedResult);
    }

    [Fact]
    public void WithError_AddsErrorToResult()
    {
        // Arrange
        var result = Success();
        var error = new Error("New Error", "description", ErrorType.Problem);

        // Act
        var resultWithError = result.WithError(error);

        // Assert
        Assert.True(resultWithError.IsFailure);
        Assert.Equal(error, resultWithError.Errors.First());
    }

    [Fact]
    public void WithErrors_AddsErrorsToResult()
    {
        // Arrange
        var result = Success();
        var errors = new Errors { new("Error 1", "description", ErrorType.Problem), new("Error 2", "description", ErrorType.Problem) };

        // Act
        var resultWithErrors = result.WithErrors(errors);

        // Assert
        Assert.True(resultWithErrors.IsFailure);
        Assert.Equivalent(errors, resultWithErrors.Errors);
    }

    [Fact]
    public void ToResult_ReturnsNewResultWithNewValue()
    {
        // Arrange
        var result = Success();
        const int newValue = 42;

        // Act
        var newResult = result.ToResult(newValue);

        // Assert
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newValue, newResult.Value);
    }

    [Fact]
    public void Merge_ReturnsMergedResult()
    {
        // Arrange
        var result1 = Success();
        var result2 = Failure(new("Test", "Test description", ErrorType.Problem));
        var result3 = Failure(new("Test 2", "Test description 2", ErrorType.Problem));

        // Act
        var mergedResult = Merge(result1, result2, result3);

        // Assert
        Assert.True(mergedResult.IsFailure);
        Assert.Contains(mergedResult.Errors, e => e.Code == "Test");
        Assert.Contains(mergedResult.Errors, e => e.Code == "Test 2");
    }

    [Theory]
    [MemberData(nameof(MergeWithValueTestData))]
    public void MergeWithValue_ReturnsMergedResult<T>(
        List<Result<T>> results,
        IEnumerable<T> expectedValues,
        Errors expectedErrors,
        bool expectedSuccess
    )
    {
        // Act
        var mergedResult = Merge(results);

        // Assert
        Assert.Equal(expectedSuccess, mergedResult.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValues, mergedResult.Value);
        }
        else
        {
            Assert.Empty(mergedResult.Value);
        }

        if (!expectedSuccess)
        {
            Assert.Equal(expectedErrors.Distinct(), mergedResult.Errors);
        }
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailureResult()
    {
        // Arrange
        var error = new Error("Implicit Error", "description", ErrorType.Problem);

        // Act
        Result result = error;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equivalent(error, result.Errors.First());
    }

    [Fact]
    public void ImplicitConversion_FromErrors_ReturnsFailureResult()
    {
        // Arrange
        Errors errors =
        [
            new("Implicit Error", "description", ErrorType.Problem),
            new("Implicit Error 2", "description 2", ErrorType.Problem)
        ];

        // Act
        Result result = errors;

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equivalent(errors, result.Errors);
    }

    public static IEnumerable<TheoryDataRow> MergeWithValueTestData()
    {
        yield return GenerateMergeWithValueTestData(3, 0, [1, 2, 3]);
        yield return GenerateMergeWithValueTestData(1, 1, [1]);
        yield return GenerateMergeWithValueTestData(0, 2, new List<int>());
    }

    public static TheoryDataRow GenerateMergeWithValueTestData<T>(
        int successCount,
        int failureCount,
        List<T> successValues)
    {
        var results = new List<Result<T>>();
        var expectedValues = new List<T>();
        var expectedErrors = new Errors();
        bool expectedSuccess = failureCount == 0;

        // Add successful results
        for (int i = 0; i < successCount; i++)
        {
            var value = successValues[i];
            results.Add(Success(value));
            expectedValues.Add(value);
        }

        // Add failure results
        for (int i = 0; i < failureCount; i++)
        {
            var error = new Error($"Error{i + 1}", $"Error description {i + 1}", ErrorType.Problem);
            expectedErrors.Add(error);
            results.Add(Failure<T>(error));
        }

        return new(results, expectedValues, expectedErrors, expectedSuccess);
    }

    [Theory]
    [MemberData(nameof(BindTestData))]
    public void Bind_Sync_Success(Result initialResult, Func<Result> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindTestData))]
    public void Bind_Sync_Failure(Result initialResult, Func<Result> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindAsyncTestData))]
    public async Task Bind_Async_Success(Result initialResult, Func<Task<Result>> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = await initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindAsyncTestData))]
    public async Task Bind_Async_Failure(Result initialResult, Func<Task<Result>> bindFunc, bool expectedSuccess)
    {
        // Act
        var result = await initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(BindGenericTestData))]
    public void Bind_Generic_Sync_Success<TValue>(
        Result initialResult,
        Func<Result<TValue>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericTestData))]
    public void Bind_Generic_Sync_Failure<TValue>(
        Result initialResult,
        Func<Result<TValue>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericAsyncTestDataInt))]
    public async Task Bind_Generic_Async_Success<TValue>(
        Result initialResult,
        Func<Task<Result<TValue>>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = await initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    [Theory]
    [MemberData(nameof(BindGenericAsyncTestDataInt))]
    public async Task Bind_Generic_Async_Failure<TValue>(
        Result initialResult,
        Func<Task<Result<TValue>>> bindFunc,
        bool expectedSuccess,
        TValue expectedValue
    )
    {
        // Act
        var result = await initialResult.Bind(bindFunc);

        // Assert
        Assert.Equal(expectedSuccess, result.IsSuccess);
        if (expectedSuccess)
        {
            Assert.Equal(expectedValue, result.Value);
        }
    }

    public static IEnumerable<TheoryDataRow> BindTestData()
    {
        yield return new(Success(), GenerateSuccessFunc(), true);
        yield return new(GenerateSingleFailure(1), GenerateSuccessFunc(), false);
    }

    public static IEnumerable<TheoryDataRow> BindAsyncTestData()
    {
        yield return new (Success(), GenerateSuccessTaskFunc(), true);
        yield return new (GenerateSingleFailure(1), GenerateSuccessTaskFunc(), false);
    }

    public static IEnumerable<TheoryDataRow> BindGenericTestData()
    {
        yield return new (Success(), GenerateSuccessFunc(42), true, 42);
        yield return new (GenerateSingleFailure(1), GenerateSuccessFunc(42), false, 42);
    }

    public static IEnumerable<TheoryDataRow> BindGenericAsyncTestDataInt()
    {
        return BindGenericAsyncTestData(42);
    }

    public static IEnumerable<TheoryDataRow> BindGenericAsyncTestData<TValue>(TValue value)
    {
        yield return new (Success(), GenerateSuccessTaskFunc(value), true, value);
        yield return new(GenerateSingleFailure(1), GenerateSuccessTaskFunc(value), false, value);
    }


}
