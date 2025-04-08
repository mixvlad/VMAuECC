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
}
