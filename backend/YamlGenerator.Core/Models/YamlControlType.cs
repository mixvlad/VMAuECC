namespace YamlGenerator.Core.Models
{
    // This class structure matches the YAML file format
    public class YamlControlType
    {
        public Dictionary<string, LanguageStrings> Localization { get; set; } = new();
        public List<YamlParameterDefinition> Parameters { get; set; } = new();

        // Convert to our domain model ControlType
        public ControlType ToControlType(string id)
        {
            var controlType = new ControlType
            {
                Id = id
            };

            // Map localization
            foreach (var (language, strings) in Localization)
            {
                if (!string.IsNullOrEmpty(strings.Name))
                    controlType.Names[language] = strings.Name;
                
                if (!string.IsNullOrEmpty(strings.Description))
                    controlType.Descriptions[language] = strings.Description;
            }

            // Map parameters
            foreach (var yamlParam in Parameters)
            {
                var param = new ParameterDefinition
                {
                    Name = yamlParam.Name,
                    Type = yamlParam.Type,
                    Required = yamlParam.Required,
                    DefaultValue = yamlParam.DefaultValue
                };

                // Map parameter localization
                foreach (var (language, strings) in yamlParam.Localization)
                {
                    param.Localization[language] = new ParameterLocalization
                    {
                        DisplayName = strings.DisplayName,
                        Description = strings.Description
                    };
                }

                controlType.Parameters.Add(param);
            }

            return controlType;
        }
    }

    public class LanguageStrings
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class YamlParameterDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "string";
        public bool Required { get; set; }
        public string DefaultValue { get; set; } = string.Empty;
        public Dictionary<string, ParameterLanguageStrings> Localization { get; set; } = new();
    }

    public class ParameterLanguageStrings
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
