using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bookshelf.Api.Helper;

public static class IsbnRangesHelper
{
    private static readonly Lock Lock = new();

    public static IsbnRangeMessage IsbnData
    {
        get
        {
            lock (Lock)
            {
                if (field == null)
                {
                    var jsonPath = "IsbnRanges.json";

                    if (File.Exists(jsonPath))
                    {
                        var json = File.ReadAllText(jsonPath);
                        field = (IsbnRangeMessage?)JsonSerializer.Deserialize(json, typeof(IsbnRangeMessage), IsbnServiceJsonContext.Default) ?? new IsbnRangeMessage();
                    }

                    if (field == null)
                        throw new Exception("Could not load ISBN range data.");
                }
            }

            return field;
        }
    }
}

[JsonSerializable(typeof(IsbnRangeMessage))]
internal partial class IsbnServiceJsonContext : JsonSerializerContext;

public class IsbnRangeMessage
{
    public string? MessageSource { get; set; }
    public List<EanUccPrefix>? EanUccPrefixes { get; set; }
    public List<RegistrationGroup>? RegistrationGroups { get; set; }
}

public class EanUccPrefix
{
    public string? Prefix { get; set; }
    public string? Agency { get; set; }
    public List<Rule>? Rules { get; set; }
}

public class RegistrationGroup
{
    public string? Prefix { get; set; }
    public string? Agency { get; set; }
    public List<Rule>? Rules { get; set; }
}

public class Rule
{
    public string? Range { get; set; }
    public int Length { get; set; }
}