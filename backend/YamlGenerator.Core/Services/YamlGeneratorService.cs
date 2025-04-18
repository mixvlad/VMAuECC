using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class YamlGeneratorService
{
    private readonly ISerializer _serializer;
    private readonly Dictionary<string, string> _templateResourceMap;
    private const string DefaultTemplateResource = "YamlGenerator.Core.Data.Templates.unix_shell.yaml";

    public YamlGeneratorService()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        // Инициализация словаря маппинга типов контроля на имена ресурсов шаблонов
        _templateResourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "windows_file_info", "YamlGenerator.Core.Data.Templates.windows_file_info.yaml" },
            { "file_integrity_check_windows", "YamlGenerator.Core.Data.Templates.windows_shell_hash.yaml" },
            { "file_content_check", "YamlGenerator.Core.Data.Templates.unix_file_content_check.yaml" },
            { "file_integrity_check", "YamlGenerator.Core.Data.Templates.unix_shell_hash.yaml" },
            { "windows_command_result_check", "YamlGenerator.Core.Data.Templates.windows_shell.yaml" },
            { "unix_command_result_check", "YamlGenerator.Core.Data.Templates.unix_shell.yaml" },
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
        
        return ProcessTemplate(templateContent, config);
    }

    // В YamlGeneratorService.cs
    private string ProcessTemplate(string template, CollectorConfig config)
    {
        // Заменяем переменные из параметров
        foreach (var param in config.Parameters)
        {
            template = template.Replace($"{{{{{param.Key}}}}}", param.Value);
        }
        
        return template;
    }

    
    private string SelectTemplateResourceName(string controlTypeId)
    {
        // Использование словаря для маппинга типа контроля на имя ресурса шаблона
        if (_templateResourceMap.TryGetValue(controlTypeId, out string? resourceName))
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

    // Метод для загрузки всех доступных типов контроля
    private Dictionary<string, ControlType> LoadControlTypes()
    {
        var controlTypes = new Dictionary<string, ControlType>();
        var assembly = Assembly.GetExecutingAssembly();
        
        // Ищем все YAML файлы в подпапках ControlTypes
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith("YamlGenerator.Core.Data.ControlTypes.") && 
                           name.EndsWith(".yaml"));
        
        foreach (var resourceName in resourceNames)
        {
            // Извлекаем ID типа контроля из имени файла
            string controlTypeId = Path.GetFileNameWithoutExtension(resourceName);
            
            // Если файл находится в подпапке, извлекаем только имя файла
            if (controlTypeId.Contains("."))
            {
                controlTypeId = controlTypeId.Split('.').Last();
            }
            
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var yaml = reader.ReadToEnd();
                        var controlType = DeserializeControlType(yaml, controlTypeId);
                        controlTypes[controlTypeId] = controlType;
                    }
                }
                else
                {
                    // Handle the case where the resource stream is null
                    Console.WriteLine($"Warning: Resource not found: {resourceName}");
                }
            }
        }
        
        return controlTypes;
    }
    private ControlType DeserializeControlType(string yamlContent, string controlTypeId)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            
        // Deserialize the YAML content directly to a ControlType object
        var controlType = deserializer.Deserialize<ControlType>(yamlContent);
        
        // Set the ID from the parameter
        controlType.Id = controlTypeId;

        return controlType;
    }

    public byte[] GenerateZipConfiguration(CollectorConfig config)
    {
        // Генерируем YAML для включения в ZIP
        string yamlContent = GenerateYaml(config);
        
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                // Добавляем основной YAML файл
                var yamlEntry = archive.CreateEntry("config.yaml");
                using (var entryStream = yamlEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(yamlContent);
                }
                
                // Здесь можно добавить дополнительные файлы в архив
                // Например, README.txt
                var readmeEntry = archive.CreateEntry("README.txt");
                using (var entryStream = readmeEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write("This is a configuration package for data collection.");
                }
                
                // Создаем подпапку scripts
                var scriptEntry = archive.CreateEntry("scripts/run.sh");
                using (var entryStream = scriptEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write("#!/bin/bash\necho \"Running configuration script\"");
                }
            }
            
            return memoryStream.ToArray();
        }
    }
    
}