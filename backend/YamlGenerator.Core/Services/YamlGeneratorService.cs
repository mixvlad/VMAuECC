using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class YamlGeneratorService
{
    private readonly ISerializer _serializer;
    private readonly Dictionary<string, string> _templateResourceMap;
    private const string DefaultTemplateResource = "YamlGenerator.Core.Data.Templates.shell_template.yaml";

    public YamlGeneratorService()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        // Инициализация словаря маппинга типов контроля на имена ресурсов шаблонов
        _templateResourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "windows_file_info", "YamlGenerator.Core.Data.Templates.windows_file_info.yaml" },
            { "file_content_check", "YamlGenerator.Core.Data.Templates.shell_template.yaml" },
            
            // Можно легко добавить новые маппинги здесь
        };
    }

    public string GenerateYaml(CollectorConfig config)
    {
        Console.WriteLine($"Generating YAML for Control Type ID: {config.ControlTypeId}");
        // Выбор шаблона на основе типа контроля
        string resourceName = SelectTemplateResourceName(config.ControlTypeId);
        
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
        {
            throw new FileNotFoundException($"Template resource not found: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        string templateContent = reader.ReadToEnd();
        
        // Здесь можно добавить обработку шаблона с параметрами из config
        // Например, заменить плейсхолдеры на значения из config.Parameters
        
        return templateContent;
    }
    
    private string SelectTemplateResourceName(string controlTypeId)
    {
        // Использование словаря для маппинга типа контроля на имя ресурса шаблона
        if (_templateResourceMap.TryGetValue(controlTypeId, out string resourceName))
        {
            return resourceName;
        }
        
        // Возврат дефолтного шаблона, если маппинг не найден
        return DefaultTemplateResource;
    }

    public List<string> GetAvailableTemplates()
    {
        // Get all template resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith("YamlGenerator.Core.Data.Templates.") && name.EndsWith(".yaml"))
            .Select(name => name.Replace("YamlGenerator.Core.Data.Templates.", "").Replace(".yaml", ""))
            .ToList();
            
        return resourceNames;
    }
}