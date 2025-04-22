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
    private readonly AUEService _aueService;
    private readonly RequirementService _requirementService;
    private readonly StandardService _standardService;

    /// <summary>
    /// Конструктор контроллера YamlController
    /// </summary>
    /// <param name="aueService">Сервис для генерации AUE</param>
    /// <param name="requirementService">Сервис для генерации требований</param>
    /// <param name="standardService">Сервис для генерации стандартов</param>
    public YamlController(AUEService aueService, RequirementService requirementService, StandardService standardService)
    {
        _aueService = aueService;
        _requirementService = requirementService;
        _standardService = standardService;
    }

    /// <summary>
    /// Генерирует YAML AUE на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Сгенерированный YAML в текстовом формате</returns>
    /// <response code="200">Возвращает сгенерированный YAML</response>
    /// <response code="400">Если произошла ошибка при генерации YAML</response>
    [HttpPost("generateAUE")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> GenerateAUE([FromBody] CollectorConfig config)
    {
        try
        {
            var yaml = _aueService.GenerateAUE(config);
            return Ok(yaml);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Скачивает YAML AUE на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Файл YAML для скачивания</returns>
    [HttpPost("downloadAUE")]
    public IActionResult DownloadAUE([FromBody] CollectorConfig config)
    {
        try
        {
            var yaml = _aueService.GenerateAUE(config);
            return File(System.Text.Encoding.UTF8.GetBytes(yaml), "text/plain", "AUE.yaml");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Генерирует YAML требований на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Сгенерированный YAML в текстовом формате</returns>
    /// <response code="200">Возвращает сгенерированный YAML</response>
    /// <response code="400">Если произошла ошибка при генерации YAML</response>
    [HttpPost("generateRequirement")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> GenerateRequirement([FromBody] CollectorConfig config)
    {
        try
        {
            // Теперь этот метод возвращает JSON с содержимым файлов
            var jsonContent = _requirementService.GenerateRequirementPreview(config);
            return Ok(jsonContent);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Скачивает ZIP-архив с требованиями на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>ZIP-файл для скачивания</returns>
    [HttpPost("downloadRequirement")]
    public IActionResult DownloadRequirement([FromBody] CollectorConfig config)
    {
        try
        {
            // Генерируем ZIP-архив
            byte[] zipBytes = _requirementService.GenerateRequirement(config);
            
            // Возвращаем ZIP-файл
            return File(zipBytes, "application/zip", "Requirement.zip");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Генерирует стандартный YAML на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Сгенерированный YAML в текстовом формате</returns>
    /// <response code="200">Возвращает сгенерированный YAML</response>
    /// <response code="400">Если произошла ошибка при генерации YAML</response>
    [HttpPost("generateStandard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> GenerateStandard([FromBody] CollectorConfig config)
    {
        try
        {
            var yaml = _standardService.GenerateStandard(config);
            return Ok(yaml);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Скачивает стандартный YAML на основе предоставленной конфигурации
    /// </summary>
    /// <param name="config">Конфигурация сборщика данных</param>
    /// <returns>Файл YAML для скачивания</returns>
    [HttpPost("downloadStandard")]
    public IActionResult DownloadStandard([FromBody] CollectorConfig config)
    {
        try
        {
            var yaml = _standardService.DownloadStandard(config);
            return File(System.Text.Encoding.UTF8.GetBytes(yaml), "text/plain", "Standard.yaml");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
