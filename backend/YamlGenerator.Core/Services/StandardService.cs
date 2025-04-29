using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class StandardService : BaseTemplateService
{
    private const string StandardTemplateResource = "YamlGenerator.Core.Data.Templates.standard.yaml";
    
    public string GenerateStandard(CollectorConfig config)
    {
        // Load the standard template and process it with the provided configuration
        return LoadAssemblyFile($"YamlGenerator.Core.Data.Templates.{config.OsType}.standard.yaml", config);
    }
    
    public string DownloadStandard(CollectorConfig config)
    {
        // For download, we're returning the same content as for generation
        // but it will be handled differently in the controller (as a file download)
        return GenerateStandard(config);
    }
}
