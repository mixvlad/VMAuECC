using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class LocalizationService
{
    private readonly IDeserializer _deserializer;
    private List<ControlType> _unixControlTypes = new();
    private List<ControlType> _windowsControlTypes = new();

    public LocalizationService()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        LoadControlTypes();
    }

    private void LoadControlTypes()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Загрузка типов контроля для Unix
            using var unixStream = assembly.GetManifestResourceStream("YamlGenerator.Core.Data.Localization.unix-control-types.yaml");
            if (unixStream != null)
            {
                using var reader = new StreamReader(unixStream);
                var yaml = reader.ReadToEnd();
                var rawData = _deserializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(yaml);

                // Convert to our model format
                _unixControlTypes = ConvertToControlTypes(rawData);
            }

            // Загрузка типов контроля для Windows
            using var windowsStream = assembly.GetManifestResourceStream("YamlGenerator.Core.Data.Localization.windows-control-types.yaml");
            if (windowsStream != null)
            {
                using var reader = new StreamReader(windowsStream);
                var yaml = reader.ReadToEnd();
                var rawData = _deserializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(yaml);

                // Convert to our model format
                _windowsControlTypes = ConvertToControlTypes(rawData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading control types from YAML: {ex.Message}");
        }
    }

    private List<ControlType> ConvertToControlTypes(
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> rawData)
    {
        var result = new List<ControlType>();

        foreach (var (controlId, languages) in rawData)
        {
            var controlType = new ControlType()
            {
                Id = controlId
            };


            foreach (var (language, properties) in languages)
            {
                if (properties.TryGetValue("name", out var name))
                {
                    controlType.Names[language] = name;
                }

                if (properties.TryGetValue("description", out var description))
                {
                    controlType.Descriptions[language] = description;
                }
            }

            result.Add(controlType);
        }

        return result;
    }


    public List<LocalizedControlType> GetControlTypes(string osType, string language = "en")
    {
        var localizedControlTypes = new List<LocalizedControlType>();
        var controlTypes = osType.ToLower() == "unix" ? _unixControlTypes : _windowsControlTypes;

        foreach (var controlType in controlTypes)
        {
            localizedControlTypes.Add(new LocalizedControlType
                {
                    Id = controlType.Id,
                    Name = controlType.GetName(language),
                    Description = controlType.GetDescription(language)
                });
            
        }

        return localizedControlTypes;
    }
}
