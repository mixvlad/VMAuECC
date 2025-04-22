using System.Reflection;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class AUEService : BaseTemplateService
{
    private readonly Dictionary<string, string> _templateResourceMap;
    private const string DefaultTemplateResource = "YamlGenerator.Core.Data.Templates.unix_shell.yaml";

    public AUEService() : base()
    {
        // Initialize the mapping dictionary of control types to template resource names
        _templateResourceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "windows_file_info", "YamlGenerator.Core.Data.ControlTypes.windows.windows_file_info.aue_windows_file_info.yaml" },
            { "windows_file_integrity_check", "YamlGenerator.Core.Data.ControlTypes.windows.windows_file_integrity_check.aue_windows_shell_hash.yaml" },
            { "windows_command_result_check", "YamlGenerator.Core.Data.ControlTypes.windows.windows_command_result_check.aue_windows_shell.yaml" },
            { "unix_file_content_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_file_content_check.aue_unix_shell_hash.yaml" },
            { "unix_file_integrity_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_file_integrity_check.aue_unix_shell_hash.yaml" },
            { "unix_command_result_check", "YamlGenerator.Core.Data.ControlTypes.unix.unix_command_result_check.aue_unix_shell.yaml" },
            // Can easily add new mappings here
        };
    }

    public string GenerateAUE(CollectorConfig config)
    {
        // Select template based on control type
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
    
    private string SelectTemplateResourceName(string controlTypeId)
    {
        // Use dictionary to map control type to template resource name
        if (_templateResourceMap.TryGetValue(controlTypeId, out string? resourceName))
        {
            return resourceName;
        }
        
        // Return default template if mapping not found
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
}
