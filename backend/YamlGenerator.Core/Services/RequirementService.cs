using System.IO.Compression;
using System.Text.Json;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class RequirementService : BaseTemplateService
{
    private readonly StandardService _standardService;
    
    public RequirementService(StandardService standardService) : base()
    {
        _standardService = standardService;
    }
    
    public string GenerateRequirementPreview(CollectorConfig config)
    {
        // This method returns a JSON representation of the requirement files
        // for preview purposes, not the actual ZIP file
        
        // Load the necessary files
        string i18nContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.i18n.yaml");
        string dataRequirementsParametersContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.DataRequirementsParameters.yaml", config);
        string ccruleContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.User.Check.ccrule.xml", config);
        
        // Create a dictionary with file contents
        var fileContents = new Dictionary<string, string>
        {
            { "i18n.yaml", i18nContent },
            { "DataRequirementsParameters.yaml", dataRequirementsParametersContent },
            { "User.Check.ccrule.xml", ccruleContent }
        };
        
        // Serialize to JSON
        return JsonSerializer.Serialize(fileContents);
    }
    
    public byte[] GenerateRequirement(CollectorConfig config)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Generate standard YAML for inclusion in ZIP
                string standardContent = _standardService.GetStandardTemplate();
                
                // Load other required files
                string i18nContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.i18n.yaml", config);
                string dataRequirementsParametersContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.DataRequirementsParameters.yaml", config);
                string ccruleContent = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.User.Check.ccrule.xml", config);

                // Add README.txt to the archive
                // var readmeEntry = archive.CreateEntry("README.txt");
                // using (var entryStream = readmeEntry.Open())
                // using (var streamWriter = new StreamWriter(entryStream))
                // {
                //     streamWriter.Write("This is a configuration package for data collection.");
                // }

                // Add standard YAML file
                // var standardEntry = archive.CreateEntry("standard.yaml");
                // using (var entryStream = standardEntry.Open())
                // using (var streamWriter = new StreamWriter(entryStream))
                // {
                //     streamWriter.Write(standardContent);
                // }
                
                // Add ccrule file
                var ccruleEntry = archive.CreateEntry($"UserRequirements/User.{config.Parameters["AddPropertiesName"]}/User.{config.Parameters["AddPropertiesName"]}.ccrule.xml");
                using (var entryStream = ccruleEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(ccruleContent);
                }

                // Add DataRequirementsParameters file
                var dataRequirementsParametersEntry = archive.CreateEntry($"UserRequirements/User.{config.Parameters["AddPropertiesName"]}/DataRequirementsParameters.yaml");
                using (var entryStream = dataRequirementsParametersEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(dataRequirementsParametersContent);
                }

                // Add i18n file
                var i18nEntry = archive.CreateEntry($"UserRequirements/User.{config.Parameters["AddPropertiesName"]}/i18n.yaml");
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
