using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Data;

public static class ControlTypeDefinitions
{
    private static readonly Lazy<List<ControlType>> _unixControlTypes = new(LoadUnixControlTypes);
    private static readonly Lazy<List<ControlType>> _windowsControlTypes = new(LoadWindowsControlTypes);

    public static List<ControlType> UnixControlTypes => _unixControlTypes.Value;

    public static List<ControlType> WindowsControlTypes => _windowsControlTypes.Value;

    private static List<ControlType> LoadUnixControlTypes()
    {
        return LoadControlTypesFromYaml("unix-control-types.yaml");
    }

    private static List<ControlType> LoadWindowsControlTypes()
    {
        return LoadControlTypesFromYaml("windows-control-types.yaml");
    }

    private static List<ControlType> LoadControlTypesFromYaml(string fileName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"YamlGenerator.Core.Data.Localization.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Console.WriteLine($"Resource not found: {resourceName}");
                return new List<ControlType>();
            }

            using var reader = new StreamReader(stream);
            var yamlContent = reader.ReadToEnd();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            // Десериализуем YAML в словарь, где ключ - это Id контрола, а значение - словарь с локализациями
            var rawData = deserializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(yamlContent);

            var controlTypes = new List<ControlType>();

            foreach (var entry in rawData)
            {
                var controlType = new ControlType
                {
                    Id = entry.Key,
                    Names = new Dictionary<string, string>(),
                    Descriptions = new Dictionary<string, string>()
                };

                // Заполняем локализованные имена и описания
                foreach (var locale in entry.Value)
                {
                    string language = locale.Key;
                    if (locale.Value.TryGetValue("name", out var name))
                    {
                        controlType.Names[language] = name;
                    }
                    if (locale.Value.TryGetValue("description", out var description))
                    {
                        controlType.Descriptions[language] = description;
                    }
                }

                controlTypes.Add(controlType);
            }

            return controlTypes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading control types from YAML: {ex.Message}");
            return new List<ControlType>();
        }
    }

}