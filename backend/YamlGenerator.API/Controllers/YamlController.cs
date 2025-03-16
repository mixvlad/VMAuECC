using Microsoft.AspNetCore.Mvc;
using YamlGenerator.Core.Models;
using YamlGenerator.Core.Services;

namespace YamlGenerator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YamlController : ControllerBase
{
    private readonly YamlGeneratorService _yamlGenerator;

    public YamlController(YamlGeneratorService yamlGenerator)
    {
        _yamlGenerator = yamlGenerator;
    }

    [HttpPost("generate")]
    public ActionResult<string> GenerateYaml([FromBody] CollectorConfig config)
    {
        try
        {
            var yaml = _yamlGenerator.GenerateYaml(config);
            return Ok(yaml);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 