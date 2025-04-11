using YamlGenerator.Core.Models;

namespace YamlGenerator.Core.Services;

public class ControlTypeService
{
    private readonly LocalizationService _localizationService;

    public ControlTypeService(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public OsControlTypes GetControlTypesByOs(string osType, string language = "en")
    {
        return osType.ToLower() switch
        {
            "unix" or "linux" => new OsControlTypes
            {
                Os = "unix",
                ControlTypes = _localizationService.GetControlTypes("unix", language)
            },
            "windows" => new OsControlTypes
            {
                Os = "windows",
                ControlTypes = _localizationService.GetControlTypes("windows", language)
            },
            _ => throw new ArgumentException($"Unsupported OS type: {osType}. Supported types are 'unix' and 'windows'.")
        };
    }

    public List<OsControlTypes> GetAllControlTypes(string language = "en")
    {
        return new List<OsControlTypes>
        {
            new OsControlTypes
            {
                Os = "unix",
                ControlTypes = _localizationService.GetControlTypes("unix", language)
            },
            new OsControlTypes
            {
                Os = "windows",
                ControlTypes = _localizationService.GetControlTypes("windows", language)
            }
        };
    }
      public ControlTypeWithParameters GetControlTypeWithParameters(string osType, string controlTypeId, string language = "en")
      {
          // Get the basic control type info
          var controlTypes = osType.ToLower() switch
          {
              "unix" or "linux" => _localizationService.GetControlTypes("unix", language),
              "windows" => _localizationService.GetControlTypes("windows", language),
              _ => throw new ArgumentException($"Unsupported OS type: {osType}. Supported types are 'unix' and 'windows'.")
          };

          var controlType = controlTypes.FirstOrDefault(ct => ct.Id == controlTypeId) 
              ?? throw new KeyNotFoundException($"Control type with ID '{controlTypeId}' not found for OS '{osType}'");

          // For now, all control types have the same parameter (Options)
          return new ControlTypeWithParameters
          {
              Id = controlType.Id,
              Name = controlType.Name,
              Description = controlType.Description,
              Parameters = new List<ControlTypeParameter>
              {
                  new ControlTypeParameter
                  {
                      Name = "options",
                      DisplayName = "Options",
                      Description = "Configuration options for this control type",
                      Type = "string",
                      Required = true,
                      DefaultValue = ""
                  }
              }
          };
      }
  }