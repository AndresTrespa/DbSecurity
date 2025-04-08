using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Consumers en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ConsumerController : ControllerBase
    {
        private readonly ConsumerBusiness _consumerBusiness;
        private readonly ILogger<ConsumerController> _logger;

        ///<summary>
        ///Constructor del controlador de Consumers
        ///</summary>
        ///<param name="consumerBusiness">Capa de negocio de Consumers</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public ConsumerController(ConsumerBusiness consumerBusiness, ILogger<ConsumerController> logger)
        {
            _consumerBusiness = consumerBusiness;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los Consumers del Sistema
        ///</summary>
        ///<returns>Lista de Consumers</returns>
        ///<response code="200">Retorna la lista de Consumers</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConsumerDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllConsumers()
        {
            try
            {
                var consumers = await _consumerBusiness.GetAllAsync();
                return Ok(consumers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Consumers");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtiene un Consumer específico por su ID
        ///</summary>
        ///<returns>Consumer solicitado</returns>
        ///<response code="200">Retorna el Consumer solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">Consumer no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConsumerDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetConsumerById(int id)
        {
            try
            {
                var consumer = await _consumerBusiness.GetByIdAsync(id);
                return Ok(consumer);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el Consumer con ID: {ConsumerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Consumer no encontrado con ID: {ConsumerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Consumer con ID: {ConsumerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Crea un nuevo Consumer en el sistema
        ///</summary>
        ///<param name="consumerDto">Datos del Consumer a crear</param>
        ///<returns>Consumer creado</returns>
        ///<response code="201">Retorna el Consumer creado</response>
        ///<response code="400">Datos del Consumer no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ConsumerDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateConsumer([FromBody] ConsumerDto consumerDto)
        {
            try
            {
                var createdConsumer = await _consumerBusiness.CreateAsync(consumerDto);
                return CreatedAtAction(nameof(GetConsumerById), new { id = createdConsumer.Id }, createdConsumer);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear Consumer");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Consumer");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina un Consumer físicamente del sistema
        ///</summary>
        ///<param name="id">ID del Consumer a eliminar</param>
        ///<response code="204">Consumer eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Consumer no encontrado</response>
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
                await _consumerBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar Consumer con ID: {ConsumerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Consumer no encontrado con ID: {ConsumerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar Consumer con ID: {ConsumerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un Consumer (cambio de estado)
        ///</summary>
        ///<param name="id">ID del Consumer a eliminar lógicamente</param>
        ///<response code="200">Consumer desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Consumer no encontrado</response>
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
                await _consumerBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Consumer eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente Consumer con ID: {ConsumerId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Consumer no encontrado con ID: {ConsumerId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente Consumer con ID: {ConsumerId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
