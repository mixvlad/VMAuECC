using Microsoft.AspNetCore.Mvc;
using YamlGenerator.Core.Models;
using YamlGenerator.Core.Services;

namespace YamlGenerator.API.Controllers;

/// <summary>
/// Контроллер для работы с типами контроля
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ControlTypesController : ControllerBase
{
    private readonly ControlTypeService _controlTypeService;

    /// <summary>
    /// Конструктор контроллера ControlTypesController
    /// </summary>
    /// <param name="controlTypeService">Сервис для работы с типами контроля</param>
    public ControlTypesController(ControlTypeService controlTypeService)
    {
        _controlTypeService = controlTypeService;
    }

    /// <summary>
    /// Получает все типы контроля для всех ОС
    /// </summary>
    /// <param name="language">Язык для локализации (по умолчанию "en")</param>
    /// <returns>Список типов контроля по ОС</returns>
    [HttpGet]
    public ActionResult<List<OsControlTypes>> GetAllControlTypes([FromQuery] string language = "en")
    {
        try
        {
            var controlTypes = _controlTypeService.GetAllControlTypes(language);
            return Ok(controlTypes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
      /// <summary>
      /// Получает типы контроля для указанной ОС
      /// </summary>
      /// <param name="osType">Тип ОС (unix/windows)</param>
      /// <param name="language">Язык для локализации (по умолчанию "en")</param>
      /// <returns>Типы контроля для указанной ОС</returns>
      /// <response code="200">Возвращает типы контроля для указанной ОС</response>
      /// <response code="400">Если указан неподдерживаемый тип ОС</response>
      [HttpGet("{osType}")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status400BadRequest)]
      public ActionResult<OsControlTypes> GetControlTypesByOs(string osType, [FromQuery] string language = "en")
      {
          try
          {
              var controlTypes = _controlTypeService.GetControlTypesByOs(osType, language);
              return Ok(controlTypes);
          }
          catch (ArgumentException ex)
          {
              return BadRequest(ex.Message);
          }
          catch (Exception ex)
          {
              return StatusCode(500, ex.Message);
          }
      }

      /// <summary>
      /// Получает тип контроля по идентификатору с параметрами
      /// </summary>
      /// <param name="controlTypeId">Идентификатор типа контроля</param>
      /// <param name="osType">Тип ОС (unix/windows)</param>
      /// <param name="language">Язык для локализации (по умолчанию "en")</param>
      /// <returns>Тип контроля с параметрами</returns>
      /// <response code="200">Возвращает тип контроля с параметрами</response>
      /// <response code="404">Если тип контроля не найден</response>
      [HttpGet("detail/{controlTypeId}")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      public ActionResult<ControlTypeWithParameters> GetControlType(string controlTypeId, [FromQuery] string osType, [FromQuery] string language = "en")
      {
          try
          {
              var controlType = _controlTypeService.GetControlTypeWithParameters(osType, controlTypeId, language);
              return Ok(controlType);
          }
          catch (KeyNotFoundException ex)
          {
              return NotFound(ex.Message);
          }
          catch (ArgumentException ex)
          {
              return BadRequest(ex.Message);
          }
      }
  }
    