using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class LocalizationService
{
    private readonly IDeserializer _deserializer;
    private List<ControlType> _unixControlTypes = new();
    private List<ControlType> _windowsControlTypes = new();

    public LocalizationService()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        LoadControlTypes();
    }

    private void LoadControlTypes()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Загрузка типов контроля для Unix
            using var unixStream = assembly.GetManifestResourceStream("YamlGenerator.Core.Data.ControlTypes.unix-control-types.yaml");
            if (unixStream != null)
            {
                using var reader = new StreamReader(unixStream);
                var yaml = reader.ReadToEnd();
                var rawData = _deserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

                // Convert to our model format
                _unixControlTypes = ConvertToControlTypes(rawData);
            }

            // Загрузка типов контроля для Windows
            using var windowsStream = assembly.GetManifestResourceStream("YamlGenerator.Core.Data.ControlTypes.windows-control-types.yaml");
            if (windowsStream != null)
            {
                using var reader = new StreamReader(windowsStream);
                var yaml = reader.ReadToEnd();
                var rawData = _deserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yaml);

                // Convert to our model format
                _windowsControlTypes = ConvertToControlTypes(rawData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading control types from YAML: {ex.Message}");
        }
    }

    private List<ControlType> ConvertToControlTypes(Dictionary<string, Dictionary<string, object>> rawData)
    {
        var result = new List<ControlType>();

        foreach (var (controlId, properties) in rawData)
        {
            var controlType = new ControlType()
            {
                Id = controlId
            };

            // Process localization section
            if (properties.TryGetValue("localization", out var locObj) && locObj is Dictionary<object, object> localization)
            {
                foreach (var (language, langObj) in localization)
                {
                    if (langObj is Dictionary<object, object> langProps)
                    {
                        if (langProps.TryGetValue("name", out var nameObj))
                        {
                            controlType.Names[language.ToString()] = nameObj.ToString();
                        }

                        if (langProps.TryGetValue("description", out var descObj))
                        {
                            controlType.Descriptions[language.ToString()] = descObj.ToString();
                        }
                    }
                }
            }

            // Process parameters section
            if (properties.TryGetValue("parameters", out var paramsObj) && paramsObj is List<object> parameters)
            {
                foreach (var paramObj in parameters)
                {
                    if (paramObj is Dictionary<object, object> paramDict)
                    {
                        var parameter = new ParameterDefinition();
                        
                        if (paramDict.TryGetValue("name", out var nameObj))
                        {
                            parameter.Name = nameObj.ToString();
                        }
                        
                        if (paramDict.TryGetValue("type", out var typeObj))
                        {
                            parameter.Type = typeObj.ToString();
                        }
                        
                        if (paramDict.TryGetValue("required", out var reqObj) && bool.TryParse(reqObj.ToString(), out var reqBool))
                        {
                            parameter.Required = reqBool;
                        }
                        
                        if (paramDict.TryGetValue("defaultValue", out var defObj))
                        {
                            parameter.DefaultValue = defObj.ToString();
                        }
                        
                        // Process parameter localization
                        if (paramDict.TryGetValue("localization", out var paramLocObj) && paramLocObj is Dictionary<object, object> paramLoc)
                        {
                            foreach (var (language, langObj) in paramLoc)
                            {
                                if (langObj is Dictionary<object, object> langProps)
                                {
                                    var paramLocalization = new ParameterLocalization();
                                    
                                    if (langProps.TryGetValue("displayName", out var dispObj))
                                    {
                                        paramLocalization.DisplayName = dispObj.ToString();
                                    }
                                    
                                    if (langProps.TryGetValue("description", out var descObj))
                                    {
                                        paramLocalization.Description = descObj.ToString();
                                    }
                                    
                                    parameter.Localization[language.ToString()] = paramLocalization;
                                }
                            }
                        }
                        
                        controlType.Parameters.Add(parameter);
                    }
                }
            }

            result.Add(controlType);
        }

        return result;
    }

    public List<LocalizedControlType> GetControlTypes(string osType, string language = "en")
    {
        var localizedControlTypes = new List<LocalizedControlType>();
        var controlTypes = osType.ToLower() == "unix" ? _unixControlTypes : _windowsControlTypes;

        foreach (var controlType in controlTypes)
        {
            localizedControlTypes.Add(new LocalizedControlType
            {
                Id = controlType.Id,
                Name = controlType.GetName(language),
                Description = controlType.GetDescription(language)
            });
        }

        return localizedControlTypes;
    }

    // Add a method to convert ParameterDefinition to ControlTypeParameter
    private ControlTypeParameter ConvertToControlTypeParameter(ParameterDefinition paramDef, string language)
    {
        var result = new ControlTypeParameter
        {
            Name = paramDef.Name,
            Type = paramDef.Type,
            Required = paramDef.Required,
            DefaultValue = paramDef.DefaultValue
        };
        
        if (paramDef.Localization.TryGetValue(language, out var localization))
        {
            result.DisplayName = localization.DisplayName;
            result.Description = localization.Description;
        }
        else if (paramDef.Localization.TryGetValue("en", out var defaultLoc))
        {
            // Fallback to English
            result.DisplayName = defaultLoc.DisplayName;
            result.Description = defaultLoc.Description;
        }
        else
        {
            // Use name as fallback for display name
            result.DisplayName = paramDef.Name;
        }
        
        return result;
    }

    // Add a method to get control type with parameters
    public ControlTypeWithParameters GetControlTypeWithParameters(string osType, string controlTypeId, string language = "en")
    {
        var controlType = GetControlTypeById(osType, controlTypeId);
        
        var result = new ControlTypeWithParameters
        {
            Id = controlType.Id,
            Name = controlType.GetName(language),
            Description = controlType.GetDescription(language),
            Parameters = controlType.Parameters.Select(p => ConvertToControlTypeParameter(p, language)).ToList()
        };
        
        return result;
    }

    public ControlType GetControlTypeById(string osType, string controlTypeId)
    {
        var controlTypes = osType.ToLower() == "unix" ? _unixControlTypes : _windowsControlTypes;
        
        return controlTypes.FirstOrDefault(ct => ct.Id == controlTypeId) 
            ?? throw new KeyNotFoundException($"Control type with ID '{controlTypeId}' not found for OS '{osType}'");
    }
}