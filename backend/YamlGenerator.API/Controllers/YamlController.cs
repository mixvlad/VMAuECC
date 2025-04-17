using Microsoft.AspNetCore.Mvc;
using YamlGenerator.Core.Models;
using YamlGenerator.Core.Services;

namespace YamlGenerator.API.Controllers;

/// <summary>
/// Контроллер для генерации YAML-файлов
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class YamlController : ControllerBase
{
    private readonly YamlGeneratorService _yamlGenerator;

    /// <summary>
    /// Конструктор контроллера YamlController
    /// </summary>
    /// <param name="yamlGenerator">Сервис для генерации YAML</param>
    public YamlController(YamlGeneratorService yamlGenerator)
    {
        _yamlGenerator = yamlGenerator;
    }

    /// <summary>
    /// Генерирует YAML на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Сгенерированный YAML в текстовом формате</returns>
    /// <response code="200">Возвращает сгенерированный YAML</response>
    /// <response code="400">Если произошла ошибка при генерации YAML</response>
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [HttpPost("generateZip")]
    public IActionResult GenerateZip([FromBody] CollectorConfig config)
    {
        try
        {
            // Генерируем ZIP-архив
            byte[] zipBytes = _yamlGenerator.GenerateZipConfiguration(config);
            
            // Возвращаем ZIP-файл
            return File(zipBytes, "application/zip", "configuration.zip");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


}
