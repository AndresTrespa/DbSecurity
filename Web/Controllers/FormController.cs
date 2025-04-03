using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    public class FormsController
    {
        [Route("api/[controller]")]
        [ApiController]
        [Produces("application/json")]
        public class FormController : ControllerBase
        {
            private readonly FormBusiness _FormBussines;
            private readonly ILogger<FormController> _logger;

            public FormController(FormBusiness FormBussines, ILogger<FormController> logger)
            {
                _FormBussines = FormBussines;
                _logger = logger;
            }

            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetAllForms()
            {
                try
                {
                    var Forms = await _FormBussines.GetFormsAsync();
                    return Ok(Forms);
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permisos");
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpGet("{id}")]
            [ProducesResponseType(typeof(FormDto), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetFormById(int id)
            {
                try
                {
                    var Form = await _FormBussines.GetFormByIdAsync(id);
                    return Ok(Form);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {FormId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogInformation(ex, "Permiso no encontrado con ID: {FormId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permiso con ID: {FormId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpPost]
            [ProducesResponseType(typeof(FormDto), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> CreateForm([FromBody] FormDto FormDto)
            {
                try
                {
                    var createdForm = await _FormBussines.CreateFormAsync(FormDto);
                    return CreatedAtAction(nameof(GetFormById), new { id = createdForm.Id }, createdForm);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al crear permiso");
                    return BadRequest(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al crear permiso");
                    return StatusCode(500, new { message = ex.Message });
                }
            }
            
        }
    }
}
