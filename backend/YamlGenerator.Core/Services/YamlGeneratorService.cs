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
        return _serializer.Serialize(config);
    }

}