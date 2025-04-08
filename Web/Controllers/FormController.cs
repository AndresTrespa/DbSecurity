using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Form en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormController : ControllerBase
    {

        private readonly FormBusiness _FormBussines;
        private readonly ILogger<FormController> _logger;

        ///<summary>
        ///Constructor del controlador de Form
        ///</summary>
        ///<param name="RolBusiness">Capa de negocio de Form</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public FormController(FormBusiness FormBussines, ILogger<FormController> logger)
        {
            _FormBussines = FormBussines;
            _logger = logger;
        }
        ///<summary>
        ///Obtiene todos los Form del Sistema
        ///</summary>
        ///<returns>Lista de Form</returns>
        ///<response code="200">Retorna la lista de Form</response>
        ///<response code="500">Error interno del servidor</response>
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
                _logger.LogError(ex, "Error al obtener Form");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        /// Obtiene un form específico por su ID
        ///</summary>
        ///<returns>form solicitado</returns>
        ///<response code="200">Retorna el form solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">form no encontrado</response>
        ///<response code="500">Error interno del servidor</response>

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
                _logger.LogWarning(ex, "Validación fallida para el form con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "form no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener form con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        ///Crea un nuevo form en el sistema
        ///</summary>
        ///<param name="RolDto">Datos del form a crear</param>
        ///<returns>form creado</returns>
        ///<response code="201">Retorna el form creado</response>
        ///<response code="400">Datos del form no válidos</response>
        ///<response code="500">Error interno del servidor</response>
  
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
                _logger.LogWarning(ex, "Validación fallida al crear form");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear form");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        /// Elimina físicamente un form del sistema
        ///</summary>
        ///<param name="id">ID del form a eliminar</param>
        ///<response code="204">form eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">form no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePersistenceAsync(int id)
        {
            try
            {
                await _FormBussines.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar form con ID: {formId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "form no encontrado con ID: {formId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar form con ID: {formId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un form (cambio de estado)
        ///</summary>
        ///<param name="id">ID del form a eliminar lógicamente</param>
        ///<response code="200">form desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">form no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicAsync(int id)
        {
            try
            {
                await _FormBussines.DeleteLogicAsync(id);
                return Ok(new { message = "form eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente form con ID: {formId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "form no encontrado con ID: {formId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente form con ID: {formId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
