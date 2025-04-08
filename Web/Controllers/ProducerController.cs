using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Producers en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProducerController : ControllerBase
    {
        private readonly ProducerBusiness _ProducerBusiness;
        private readonly ILogger<ProducerController> _logger;

        ///<summary>
        ///Constructor del controlador de Producers
        ///</summary>
        ///<param name="ProducerBusiness">Capa de negocio de Producers</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public ProducerController(ProducerBusiness ProducerBusiness, ILogger<ProducerController> logger)
        {
            _ProducerBusiness = ProducerBusiness;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los Producers del sistema
        ///</summary>
        ///<returns>Lista de Producers</returns>
        ///<response code="200">Retorna la lista de Producers</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProducerDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProducers()
        {
            try
            {
                var producers = await _ProducerBusiness.GetProducerAllAsync();
                return Ok(producers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Producers");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Obtiene un Producer específico por su ID
        ///</summary>
        ///<returns>Producer solicitado</returns>
        ///<response code="200">Retorna el Producer solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">Producer no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProducerDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProducerById(int id)
        {
            try
            {
                var producer = await _ProducerBusiness.GetProducerByIdAsync(id);
                return Ok(producer);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el Producer con ID: {ProducerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Producer no encontrado con ID: {ProducerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Producer con ID: {ProducerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Crea un nuevo Producer en el sistema
        ///</summary>
        ///<param name="ProducerDto">Datos del Producer a crear</param>
        ///<returns>Producer creado</returns>
        ///<response code="201">Retorna el Producer creado</response>
        ///<response code="400">Datos del Producer no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProducerDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProducer([FromBody] ProducerDto ProducerDto)
        {
            try
            {
                var createdProducer = await _ProducerBusiness.CreateProducerAsync(ProducerDto);
                return CreatedAtAction(nameof(GetProducerById), new { id = createdProducer.Id }, createdProducer);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear Producer");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Producer");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina un Producer físicamente del sistema
        ///</summary>
        ///<param name="id">ID del Producer a eliminar</param>
        ///<response code="204">Producer eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Producer no encontrado</response>
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
                await _ProducerBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Producer con ID: {ProducerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Producer no encontrado con ID: {ProducerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Producer con ID: {ProducerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un Producer (cambio de estado)
        ///</summary>
        ///<param name="id">ID del Producer a eliminar lógicamente</param>
        ///<response code="200">Producer desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Producer no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPatch("eliminar-logico/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicAsync(int id)
        {
            try
            {
                await _ProducerBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Producer eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente Producer con ID: {ProducerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Producer no encontrado con ID: {ProducerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente Producer con ID: {ProducerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
