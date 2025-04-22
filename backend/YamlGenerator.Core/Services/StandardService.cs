using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class StandardService : BaseTemplateService
{
    private const string StandardTemplateResource = "YamlGenerator.Core.Data.Templates.standard.yaml";
    
    public StandardService() : base()
    {
    }
    
    public string GetStandardTemplate()
    {
        return LoadAssemblyFile(StandardTemplateResource);
    }
    
    public string GetStandardTemplate(CollectorConfig config)
    {
        return LoadAssemblyFile(StandardTemplateResource, config);
    }
    
    public string GenerateStandard(CollectorConfig config)
    {
        // Load the standard template and process it with the provided configuration
        return LoadAssemblyFile(StandardTemplateResource, config);
    }
    
    public string DownloadStandard(CollectorConfig config)
    {
        // For download, we're returning the same content as for generation
        // but it will be handled differently in the controller (as a file download)
        return GenerateStandard(config);
    }
}
