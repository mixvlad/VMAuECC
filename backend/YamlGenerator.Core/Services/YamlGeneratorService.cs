using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class YamlGeneratorService
{
    private readonly ISerializer _serializer;

    public YamlGeneratorService()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public string GenerateYaml(CollectorConfig config)
    {
        // For now, just return the template without modifications
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "YamlGenerator.Core.Data.Templates.shell_template.yaml";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Template resource not found: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}