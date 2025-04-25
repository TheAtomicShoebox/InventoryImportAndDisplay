using System.Runtime.InteropServices.JavaScript;
using Inventory.Common.Results;
using static Inventory.Common.Results.Result;

namespace AppHost.Tests.Results;

public static class TestResultDataGenerators
{
    public static Func<Task<Result<TValue>>> GenerateSuccessTaskFunc<TValue>(TValue value) => () => Task.FromResult(Success(value));
    public static Func<ValueTask<Result<TValue>>> GenerateSuccessValueTaskFunc<TValue>(TValue value) => () => new(Success(value));
    public static Func<Result<TValue>> GenerateSuccessFunc<TValue>(TValue value) => () => Success(value);
    public static Func<Result> GenerateSuccessFunc() => Success;
    public static Func<Task<Result>> GenerateSuccessTaskFunc() => () => Task.FromResult(Success());
    public static Func<ValueTask<Result>> GenerateSuccessValueTaskFunc() => () => new(Success());
    public static Result GenerateSingleFailure(int failureNumber) => Failure(GenerateError(failureNumber));
    public static Result<TValue> GenerateSingleFailure<TValue>(int failureNumber) => Failure<TValue>(GenerateError(failureNumber));
    public static Result GenerateMultipleFailures(int failureNumberStart, int count) => GenerateErrors(failureNumberStart, count);
    public static Result<TValue> GenerateMultipleFailures<TValue>(int failureNumberStart, int count) => GenerateErrors(failureNumberStart, count);
    public static Error GenerateError(int errorNumber, ErrorType? errorType = ErrorType.Problem)
        => new($"Error {errorNumber}", $"Error message {errorNumber}", ErrorType.Problem);
    public static Errors GenerateErrors(int errorNumberStart, int count)
    {
        var errors = new Errors();
        for (int i = 0; i < count; i++)
        {
            errors.Add(GenerateError(errorNumberStart));
            errorNumberStart++;
        }
        return errors;
    }
}