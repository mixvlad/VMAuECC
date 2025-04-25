using System.Reflection;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class AUEService : BaseTemplateService
{
    public AUEService() : base()
    {
       
    }

    public string GenerateAUE(CollectorConfig config)
    {
        string aueContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.aue_{config.ControlTypeId}.yaml", config);
        return aueContent;
    }
    
}
