using YamlDotNet.Serialization;

namespace YamlGenerator.Core.Models;

public class ControlType
{
    public string Id { get; set; } = string.Empty;

    public Dictionary<string, LanguageStrings> Localization { get; set; } = new();

    [YamlIgnore]
    public Dictionary<string, string> Names { get; set; } = new();

    [YamlIgnore]
    public Dictionary<string, string> Descriptions { get; set; } = new();

    public List<ParameterDefinition> Parameters { get; set; } = new();

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
    
    // Метод для обработки данных после десериализации
    public void ProcessAfterDeserialization(string id)
    {
        Id = id;
        
        // Преобразуем локализацию из формата YAML в наш внутренний формат
        foreach (var (language, strings) in Localization)
        {
            if (!string.IsNullOrEmpty(strings.Name))
                Names[language] = strings.Name;
            
            if (!string.IsNullOrEmpty(strings.Description))
                Descriptions[language] = strings.Description;
        }
        
        // Обрабатываем параметры
        foreach (var param in Parameters)
        {
            param.ProcessAfterDeserialization();
        }
    }
}

public class LanguageStrings
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ParameterDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool Required { get; set; }
    public string DefaultValue { get; set; } = string.Empty;
    
    public Dictionary<string, ParameterLanguageStrings> Localization { get; set; } = new();
    
    public List<string> Options { get; set; } = new();
    
    public void ProcessAfterDeserialization()
    {
        // Здесь можно добавить дополнительную обработку параметров, если необходимо
    }
}

public class ParameterLanguageStrings
{
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ParameterLocalization
{
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class OsControlTypes
{
    public string Os { get; set; } = string.Empty;
    public List<LocalizedControlType> ControlTypes { get; set; } = new();
}

public class ControlTypeWithParameters
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ControlTypeParameter> Parameters { get; set; } = new List<ControlTypeParameter>();
}

public class ControlTypeParameter
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public bool Required { get; set; }
    public string DefaultValue { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
}    

public class LocalizedControlType
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}