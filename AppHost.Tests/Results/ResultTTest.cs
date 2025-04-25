using Inventory.Common.Results;

namespace AppHost.Tests.Results;

public class ResultTTest
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        var result = Result<int>.Success(42);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equivalent(errors, result.Errors);
    }

    [Fact]
    public void Match_ShouldReturnOnSuccessValue()
    {
        var result = Result<int>.Success(42);
        var matchResult = result.Match(
            value => value.ToString(),
            errors => "Failure"
        );
        Assert.Equal("42", matchResult);
    }

    [Fact]
    public void Match_ShouldReturnOnFailureValue()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        var matchResult = result.Match(
            value => value.ToString(),
            errors => "Failure"
        );
        Assert.Equal("Failure", matchResult);
    }

    [Fact]
    public void Map_ShouldMapSuccessValue()
    {
        var result = Result<int>.Success(42);
        var mappedResult = result.Map(value => value.ToString());
        Assert.True(mappedResult.IsSuccess);
        Assert.Equal("42", mappedResult.Value);
    }

    [Fact]
    public void Map_ShouldNotMapFailure()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        var mappedResult = result.Map(value => value.ToString());
        Assert.True(mappedResult.IsFailure);
        Assert.Equivalent(errors, mappedResult.Errors);
    }

    [Fact]
    public void Tap_ShouldExecuteActionOnSuccess()
    {
        var result = Result<int>.Success(42);
        var tapped = false;
        result.Tap(value => tapped = true);
        Assert.True(tapped);
    }

    [Fact]
    public void Tap_ShouldNotExecuteActionOnFailure()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        var tapped = false;
        result.Tap(value => tapped = true);
        Assert.False(tapped);
    }

    [Fact]
    public void WithError_ShouldAddError()
    {
        var result = Result<int>.Success(42);
        var error = new Error("Error", "Description", ErrorType.Problem);
        result = result.WithError(error);
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void WithErrors_ShouldAddErrors()
    {
        var result = Result<int>.Success(42);
        var errors = new List<Error> { new("Error1", "Description1", ErrorType.Problem), new("Error2", "Description2", ErrorType.Problem) };
        result = result.WithErrors(errors);
        Assert.True(result.IsFailure);
        Assert.Contains(errors[0], result.Errors);
        Assert.Contains(errors[1], result.Errors);
    }

    [Fact]
    public void WithValue_ShouldUpdateValue()
    {
        var result = Result<int>.Success(42);
        result = result.WithValue(100);
        Assert.Equal(100, result.Value);
    }

    [Fact]
    public void ToResult_ShouldConvertToNewResultType()
    {
        var result = Result<int>.Success(42);
        var newResult = result.ToResult(value => value.ToString());
        Assert.True(newResult.IsSuccess);
        Assert.Equal("42", newResult.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldConvertToResultObject()
    {
        var result = Result<int>.Success(42);
        Result<object> newResult = result;
        Assert.True(newResult.IsSuccess);
        Assert.Equal(42, newResult.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldConvertFromResult()
    {
        var result = Result.Success();
        Result<int> newResult = result;
        Assert.True(newResult.IsSuccess);
    }

    [Fact]
    public void ImplicitConversion_ShouldConvertFromValue()
    {
        Result<int> result = 42;
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldConvertFromError()
    {
        var error = new Error("Error", "Description", ErrorType.Problem);
        Result<int> result = error;
        Assert.True(result.IsFailure);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void ImplicitConversion_ShouldConvertFromErrors()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        Result<int> result = errors;
        Assert.True(result.IsFailure);
        Assert.Equivalent(errors, result.Errors);
    }

    [Fact]
    public void Bind_ShouldBindSuccessResult()
    {
        var result = Result<int>.Success(42);
        var boundResult = result.Bind(value => Result<string>.Success(value.ToString()));
        Assert.True(boundResult.IsSuccess);
        Assert.Equal("42", boundResult.Value);
    }

    [Fact]
    public void Bind_ShouldNotBindFailureResult()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        var boundResult = result.Bind(value => Result<string>.Success(value.ToString()));
        Assert.True(boundResult.IsFailure);
        Assert.Equivalent(errors, boundResult.Errors);
    }

    [Fact]
    public async Task BindAsync_ShouldBindSuccessResult()
    {
        var result = Result<int>.Success(42);
        var boundResult = await result.Bind(value => Task.FromResult(Result<string>.Success(value.ToString())));
        Assert.True(boundResult.IsSuccess);
        Assert.Equal("42", boundResult.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldNotBindFailureResult()
    {
        var errors = new Errors();
        errors.Add(new("Error", "Description", ErrorType.Problem));
        var result = Result<int>.Failure(errors);
        var boundResult = await result.Bind(value => Task.FromResult(Result<string>.Success(value.ToString())));
        Assert.True(boundResult.IsFailure);
        Assert.Equivalent(errors, boundResult.Errors);
    }
}