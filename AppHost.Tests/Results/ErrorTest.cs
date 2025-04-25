using System.Runtime.InteropServices.JavaScript;
using Inventory.Common.Results;

namespace AppHost.Tests.Results;

public class ErrorTest
{
    [Fact]
    public void Error_None_IsNone()
    {
        // Arrange
        var error = Error.None;

        // Act
        var isNone = error.IsNone();

        // Assert
        Assert.True(isNone);
    }
        
    [Fact]
    public void Error_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var error = new Error("TestCode", "TestDescription", ErrorType.Problem);

        // Act
        var result = error.ToString();

        // Assert
        Assert.Equal("Code: \"TestCode\", Description: \"TestDescription\"", result);
    }

    [Theory]
    [MemberData(nameof(ErrorEqualityTestData))]
    public void Error_Equality_Comparison(Error error1, Error error2, bool expected)
    {
        // Act
        var result = error1 == error2;

        // Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<TheoryDataRow> ErrorEqualityTestData()
    {
        yield return new(new Error("Code1", "Description1", ErrorType.Problem), new Error("Code1", "Description1", ErrorType.Problem), true);
        yield return new(new Error("Code1", "Description1", ErrorType.Problem), new Error("Code2", "Description2", ErrorType.Problem), false);
    }
}

public class ErrorsTest
{
    [Fact]
    public void Errors_Constructor_InitializesEmptyList()
    {
        // Act
        var errors = new Errors();

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void Errors_Add_AddsError()
    {
        // Arrange
        var errors = new Errors();
        var error = new Error("TestCode", "TestDescription", ErrorType.Problem);

        // Act
        errors.Add(error);

        // Assert
        Assert.Single(errors);
        Assert.Contains(error, errors);
    }

    [Fact]
    public void Errors_AddRange_AddsErrors()
    {
        // Arrange
        var errors = new Errors();
        var errorList = new List<Error>
        {
            new("TestCode1", "TestDescription1", ErrorType.Problem),
            new("TestCode2", "TestDescription2", ErrorType.Problem)
        };

        // Act
        errors.AddRange(errorList);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.Contains(errorList[0], errors);
        Assert.Contains(errorList[1], errors);
    }

    [Fact]
    public void Errors_Clear_RemovesAllErrors()
    {
        // Arrange
        var errors = new Errors();
        errors.Add(new Error("TestCode", "TestDescription", ErrorType.Problem));

        // Act
        errors.Clear();

        // Assert
        Assert.Empty(errors);
    }

    [Theory]
    [MemberData(nameof(ErrorsEqualityTestData))]
    public void Errors_Equality_Comparison(Errors errors, Error error, bool areEqual)
    {
        // Act
        var forwardResult = errors == error;
        var reverseResult = errors == error;

        // Assert
        Assert.Equal(areEqual, forwardResult);
        Assert.Equal(areEqual, reverseResult);
    }

    [Theory]
    [MemberData(nameof(ErrorsEqualityTestData))]
    public void Errors_InEquality_Comparison(Errors errors, Error error, bool areEqual)
    {
        // Act
        var forwardResult = errors != error;
        var reverseResult = errors != error;

        // Assert
        Assert.Equal(!areEqual, forwardResult);
        Assert.Equal(!areEqual, reverseResult);
    }

    public static IEnumerable<TheoryDataRow> ErrorsEqualityTestData()
    {
        yield return new(new Errors([new Error("Code1", "Description1", ErrorType.Problem)]), new Error("Code1", "Description1", ErrorType.Problem), true);
        yield return new(new Errors([new Error("Code1", "Description1", ErrorType.Problem)]), new Error("Code2", "Description2", ErrorType.Problem), false);
    }
}
