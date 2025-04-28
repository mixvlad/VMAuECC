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

    private string LoadI18n(CollectorConfig config)
    {
        string result;
        try
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.i18n.yaml", config);
        }
        catch (FileNotFoundException)
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.Templates.{config.OsType}.Requirement.i18n.yaml", config);
        }
        return result;
    }

    private string LoadDataRequirementsParameters(CollectorConfig config)
    {
        string result;
        try
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.DataRequirementsParameters.yaml", config);
        }
        catch (FileNotFoundException)
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.Templates.{config.OsType}.Requirement.DataRequirementsParameters.yaml", config);
        }
        return result;
    }
    
    private string LoadCcrule(CollectorConfig config)
    {
        string result;
        try
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.ControlTypes.{config.OsType}.{config.ControlTypeId}.Requirement.User.Check.ccrule.xml", config);
        }
        catch (FileNotFoundException)
        {
            result = LoadAssemblyFile($"YamlGenerator.Core.Data.Templates.{config.OsType}.Requirement.User.Check.ccrule.xml", config);
        }
        return result;
    }
    
    public string GenerateRequirementPreview(CollectorConfig config)
    {
        // This method returns a JSON representation of the requirement files
        // for preview purposes, not the actual ZIP file
        
        // Load the necessary files
        string i18nContent = LoadI18n(config);
        string dataRequirementsParametersContent = LoadDataRequirementsParameters(config);
        string ccruleContent = LoadCcrule(config);
        
        // Create a dictionary with file contents
        var fileContents = new Dictionary<string, string>
        {
            { "i18n.yaml", i18nContent },
            { "DataRequirementsParameters.yaml", dataRequirementsParametersContent },
            { $"User.{config.Parameters["AddPropertiesName"]}.ccrule.xml", ccruleContent }
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
                
                // Add ccrule file
                string ccruleContent = LoadCcrule(config);
                var ccruleEntry = archive.CreateEntry($"UserRequirements/User.{config.Parameters["AddPropertiesName"]}/User.{config.Parameters["AddPropertiesName"]}.ccrule.xml");
                using (var entryStream = ccruleEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(ccruleContent);
                }

                // Add DataRequirementsParameters file
                string dataRequirementsParametersContent = LoadDataRequirementsParameters(config);
                var dataRequirementsParametersEntry = archive.CreateEntry($"UserRequirements/User.{config.Parameters["AddPropertiesName"]}/DataRequirementsParameters.yaml");
                using (var entryStream = dataRequirementsParametersEntry.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(dataRequirementsParametersContent);
                }

                // Add i18n file
                string i18nContent = LoadI18n(config);
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
