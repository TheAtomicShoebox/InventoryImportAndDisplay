using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Inventory.Common.Results;

[method: JsonConstructor]
public readonly record struct Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new("None", "None", ErrorType.None);

    public static implicit operator Errors(Error error)
    {
        return new([error]);
    }

    public bool IsNone()
    {
        return this.Type == ErrorType.None;
    }
    
    /// <summary>
    /// Creates a new <see cref="Error"/> with the same code as the original but with a new description <br />
    /// This is meant to be used only when an <see cref="Error"/> cannot describe all context in a constant context
    /// <example>
    /// <code language="csharp">
    ///    var newError = HttpErrors.UnsuccessfulResponse.CreateFrom(desc => $"{desc} with reason: {reasonPhrase}");)
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="updateDescription"></param>
    /// <returns>The updated <see cref="Error"/></returns>
    public Error CreateFrom(Func<string, string> updateDescription)
    {
        return this with { Description = updateDescription(Description) };
    }

    public override string ToString()
    {
        return $"Code: \"{Code}\", Description: \"{Description}\"";
    }
}

[JsonConverter(typeof(StringEnumConverter<,,>))]
public enum ErrorType
{
    [EnumMember(Value = "none")]
    None = 0,
    [EnumMember(Value = "not-found")]
    NotFound = 1,
    [EnumMember(Value = "validation")]
    Validation = 2,
    [EnumMember(Value = "problem")]
    Problem = 3
}