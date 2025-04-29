// Add this to your existing ControlTypeParameter class
public class ControlTypeParameter
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public bool Required { get; set; }
    public string DefaultValue { get; set; }
    public List<string> Options { get; set; } // Add this property for select options
}