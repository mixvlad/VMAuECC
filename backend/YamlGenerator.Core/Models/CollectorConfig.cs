namespace YamlGenerator.Core.Models;

public class CollectorConfig
{
    public List<string> CollectItems { get; set; } = new();
    public string TargetPath { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; } = 60;
    public bool IncludeSubdirectories { get; set; }
    public List<string> FilePatterns { get; set; } = new();
    public Dictionary<string, string> CustomParameters { get; set; } = new();
} 