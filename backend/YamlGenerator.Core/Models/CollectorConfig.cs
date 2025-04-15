using System.Text.Json.Serialization;

namespace YamlGenerator.Core.Models;

public class CollectorConfig
{
    [JsonPropertyName("controlTypeId")]
    public string ControlTypeId { get; set; } = string.Empty;
    
    [JsonPropertyName("osType")]
    public string OsType { get; set; } = string.Empty;
    
    [JsonPropertyName("parameters")]
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    
    [JsonPropertyName("customParameters")]
    public Dictionary<string, string> CustomParameters { get; set; } = new Dictionary<string, string>();
}