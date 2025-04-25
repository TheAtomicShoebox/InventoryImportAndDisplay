using static Inventory.Common.Results.Result;

namespace Inventory.Common.Results;

internal static class ResultHelper
{
    public static Result Merge(IEnumerable<Result> results)
    {
        return CreateResult().WithErrors(results.SelectMany(result => result.Errors).Distinct());
    }

    public static Result<IEnumerable<TValue>> Merge<TValue>(IEnumerable<Result<TValue>> results)
    {
        var resultList = results.ToList();

        var finalResult = CreateResult<IEnumerable<TValue>>([]).WithErrors(resultList.SelectMany(r => r.Errors));

        if (finalResult.IsSuccess)
        {
            finalResult = finalResult.WithValue(resultList.Select(r => r.Value).ToList());
        }

        return finalResult;
    }
}