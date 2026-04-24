using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Length < 1)
{
    Console.WriteLine("Usage: IsbnRangeConverter <xml_file_path> [json_file_path]");
    return;
}

string xmlPath = args[0];
string jsonPath = args.Length > 1 ? args[1] : Path.ChangeExtension(xmlPath, ".json");

if (!File.Exists(xmlPath))
{
    Console.WriteLine($"Error: File '{xmlPath}' not found.");
    return;
}

try
{
    Console.WriteLine($"Reading '{xmlPath}'...");
    XDocument doc = XDocument.Load(xmlPath);
    XElement root = doc.Root ?? throw new Exception("Invalid XML: Root element not found.");

    var message = new IsbnRangeMessage
    {
        MessageSource = (string?)root.Element("MessageSource"),
        MessageSerialNumber = (string?)root.Element("MessageSerialNumber"),
        MessageDate = (string?)root.Element("MessageDate"),
        EanUccPrefixes = root.Element("EAN.UCCPrefixes")?.Elements("EAN.UCC")
            .Select(e => new EanUcc
            {
                Prefix = (string?)e.Element("Prefix"),
                Agency = (string?)e.Element("Agency"),
                Rules = e.Element("Rules")?.Elements("Rule")
                    .Select(r => new Rule
                    {
                        Range = (string?)r.Element("Range"),
                        Length = (int)r.Element("Length")!
                    }).ToList()
            }).ToList(),
        RegistrationGroups = root.Element("RegistrationGroups")?.Elements("Group")
            .Select(g => new RegistrationGroup
            {
                Prefix = (string?)g.Element("Prefix"),
                Agency = (string?)g.Element("Agency"),
                Rules = g.Element("Rules")?.Elements("Rule")
                    .Select(r => new Rule
                    {
                        Range = (string?)r.Element("Range"),
                        Length = (int)r.Element("Length")!
                    }).ToList()
            }).ToList()
    };

    Console.WriteLine($"Writing to '{jsonPath}'...");
    string json = JsonSerializer.Serialize(message, new JsonSerializerOptions 
    { 
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = IsbnJsonContext.Default
    });
    File.WriteAllText(jsonPath, json);

    Console.WriteLine("Conversion completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

[JsonSerializable(typeof(IsbnRangeMessage))]
internal partial class IsbnJsonContext : JsonSerializerContext
{
}

public class IsbnRangeMessage
{
    public string? MessageSource { get; set; }
    public string? MessageSerialNumber { get; set; }
    public string? MessageDate { get; set; }
    public List<EanUcc>? EanUccPrefixes { get; set; }
    public List<RegistrationGroup>? RegistrationGroups { get; set; }
}

public class EanUcc
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
