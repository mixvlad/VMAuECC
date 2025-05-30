using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services
{
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
                // Загрузка типов контроля для Unix
                _unixControlTypes = LoadControlTypesForOS("unix");
                
                // Загрузка типов контроля для Windows
                _windowsControlTypes = LoadControlTypesForOS("windows");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading control types from YAML: {ex.Message}");
            }
        }
          private List<ControlType> LoadControlTypesForOS(string osType)
          {
              var controlTypes = new List<ControlType>();
              var assembly = Assembly.GetExecutingAssembly();
            
              // Get all embedded resources
              var resourceNames = assembly.GetManifestResourceNames();
            
              // Filter resources for the specific OS type
              // New pattern: YamlGenerator.Core.Data.ControlTypes.{osType}.{controlTypeId}.{controlTypeId}.yaml
              var controlTypeResources = resourceNames
                  .Where(r => r.StartsWith($"YamlGenerator.Core.Data.ControlTypes.{osType}.") && 
                              r.EndsWith(".yaml"))
                  .ToList();
            
              foreach (var resourceName in controlTypeResources)
              {
                  try
                  {
                      // Extract control type ID from the resource name
                      // Split the resource name by dots
                      var parts = resourceName.Split('.');
                    
                      // Find the index of the OS type in the parts array
                      var osTypeIndex = Array.IndexOf(parts, osType);
                    
                      if (osTypeIndex >= 0 && osTypeIndex + 1 < parts.Length)
                      {
                          // The control type ID is now the folder name after the OS type
                          var controlTypeId = parts[osTypeIndex + 1];
                        
                          // Verify this is a control type file (filename should match folder name)
                          // The pattern should be: osType.controlTypeId.controlTypeId.yaml
                          if (parts.Length >= osTypeIndex + 3 && 
                              parts[osTypeIndex + 1] == parts[osTypeIndex + 2])
                          {
                              using var stream = assembly.GetManifestResourceStream(resourceName);
                              if (stream != null)
                              {
                                  using var reader = new StreamReader(stream);
                                  var yaml = reader.ReadToEnd();
                                
                                  try {
                                      // Deserialize to a ControlType
                                      var controlType = DeserializeControlType(yaml, controlTypeId);
                                      controlTypes.Add(controlType);
                                  }
                                  catch (Exception ex) {
                                      Console.WriteLine($"Error deserializing control type {controlTypeId}: {ex.Message}");
                                  }
                              }
                              else
                              {
                                  Console.WriteLine($"Warning: Resource not found: {resourceName}");
                              }
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine($"Error processing resource {resourceName}: {ex.Message}");
                  }
              }
            
              return controlTypes;
        }

        private ControlType DeserializeControlType(string yamlContent, string controlTypeId)
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var controlType = deserializer.Deserialize<ControlType>(yamlContent);
                
                // Обработка после десериализации
                controlType.ProcessAfterDeserialization(controlTypeId);
                
                return controlType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing control type {controlTypeId}: {ex.Message}");
                // Return a minimal control type with just the ID to avoid null references
                return new ControlType { Id = controlTypeId };
            }
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
            
            
            Console.WriteLine(result.Parameters);
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


        public ControlType GetControlTypeById(string osType, string controlTypeId)
        {
            var controlTypes = osType.ToLower() == "unix" ? _unixControlTypes : _windowsControlTypes;
            
            return controlTypes.FirstOrDefault(ct => ct.Id == controlTypeId) 
                ?? throw new KeyNotFoundException($"Control type with ID '{controlTypeId}' not found for OS '{osType}'");
        }
    }
}
