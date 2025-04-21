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
            { "windows_file_info", "YamlGenerator.Core.Data.ControlTypes.windows.windows_file_info.aue_windows_file_info.yaml" },
            { "windows_file_integrity_check", "YamlGenerator.Core.Data.ControlTypes.windows.windows_file_integrity_check.aue_windows_shell_hash.yaml" },
            { "windows_command_result_check", "YamlGenerator.Core.Data.ControlTypes.windows.windows_command_result_check.aue_windows_shell.yaml" },
            { "unix_file_content_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_file_content_check.aue_unix_shell_hash.yaml" },
            { "unix_file_integrity_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_file_integrity_check.aue_unix_shell_hash.yaml" },
            { "unix_command_result_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_command_result_check.aue_unix_shell.yaml" },
            // Можно легко добавить новые маппинги здесь
        };
    }

    public string GenerateYaml(CollectorConfig config)
    {
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

    public string LoadAssemblyFile(string resourceName, CollectorConfig? config = null)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
        {
            throw new FileNotFoundException($"Template resource not found: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        string result = reader.ReadToEnd();

        if (config != null)
        {
            result = ProcessTemplate(result, config);
        }


        return result;
    }

    public byte[] GenerateZipConfiguration(CollectorConfig config)
    {
        
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                // Генерируем YAML для включения в ZIP
                //string yamlContent = GenerateYaml(config);
                string standardContent = LoadAssemblyFile("YamlGenerator.Core.Data.Templates.standard.yaml");
                string i18nContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.i18n.yaml");
                string dataRequirementsParametersContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.DataRequirementsParameters.yaml", config);
                string ccruleContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.User.Check.ccrule.xml", config);

                
                // Здесь можно добавить дополнительные файлы в архив
                // Например, README.txt
                var readmeEntry = archive.CreateEntry("README.txt");
                using (var entryStream = readmeEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write("This is a configuration package for data collection.");
                }

                // Добавляем YAML файл стандарта
                var standardEntry = archive.CreateEntry("standard.yaml");
                using (var entryStream = standardEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(standardContent);
                }
                
                var ccruleEntry = archive.CreateEntry("Requirements/User.Check/User.Check.ccrule.xml");
                using (var entryStream = ccruleEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(ccruleContent);
                }

                var dataRequirementsParametersEntry = archive.CreateEntry("Requirements/User.Check/DataRequirementsParameters.yaml");
                using (var entryStream = dataRequirementsParametersEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(dataRequirementsParametersContent);
                }

                var i18nEntry = archive.CreateEntry("Requirements/User.Check/i18n.yaml");
                using (var entryStream = i18nEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(i18nContent);
                }
            }
            
            return memoryStream.ToArray();
        }
    }
    
}