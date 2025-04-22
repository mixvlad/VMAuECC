using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public abstract class BaseTemplateService
{
    protected readonly ISerializer _serializer;
    
    protected BaseTemplateService()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }
    
    protected string LoadAssemblyFile(string resourceName, CollectorConfig? config = null)
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
    
    protected string ProcessTemplate(string template, CollectorConfig config)
    {
        // Replace variables from parameters
        foreach (var param in config.Parameters)
        {
            template = template.Replace($"{{{{{param.Key}}}}}", param.Value);
        }
        
        return template;
    }
}
