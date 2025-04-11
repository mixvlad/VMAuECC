namespace YamlGenerator.Core.Models;

public class ControlType
{
    public string Id { get; set; } = string.Empty;

    public Dictionary<string, string> Names { get; set; } = new();

    public Dictionary<string, string> Descriptions { get; set; } = new();

    // Свойства для удобного доступа
    public string GetName(string language = "en") => GetLocalizedValue(Names, language);

    public string GetDescription(string language = "en") => GetLocalizedValue(Descriptions, language);

    private string GetLocalizedValue(Dictionary<string, string> values, string language)
    {
        if (values.TryGetValue(language, out var value))
        {
            return value;
        }

        // Возвращаем английский вариант по умолчанию
        return values.TryGetValue("en", out var defaultValue) ? defaultValue : string.Empty;
    }
}

public class OsControlTypes
{
    public string Os { get; set; } = string.Empty;

    public List<LocalizedControlType> ControlTypes { get; set; } = new();
}
