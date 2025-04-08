using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Rols en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : ControllerBase
    {
        private readonly RolBusiness _RolBusiness;
        private readonly ILogger<RolController> _logger;

        ///<summary>
        ///Constructor del controlador de Rols
        ///</summary>
        ///<param name="RolBusiness">Capa de negocio de Rols</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public RolController(RolBusiness RolBusiness, ILogger<RolController> logger)
        {
            _RolBusiness = RolBusiness;
            _logger = logger;
        }
        ///<summary>
        ///Obtiene todos los Rols del Sistema
        ///</summary>
        ///<returns>Lista de Rols</returns>
        ///<response code="200">Retorna la lista de Rols</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRols()
        {
            try
            {
                var Rols = await _RolBusiness.GetRolAllAsync();
                return Ok(Rols);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Rols");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        /// Obtiene un Rol específico por su ID
        ///</summary>
        ///<returns>Rol solicitado</returns>
        ///<response code="200">Retorna el Rol solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">Rol no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {
            try
            {
                var Rol = await _RolBusiness.GetRolByIdAsync(id);
                return Ok(Rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el Rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        ///Crea un nuevo Rol en el sistema
        ///</summary>
        ///<param name="RolDto">Datos del Rol a crear</param>
        ///<returns>Rol creado</returns>
        ///<response code="201">Retorna el Rol creado</response>
        ///<response code="400">Datos del Rol no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRol([FromBody] RolDto RolDto)
        {
            try
            {
                var createdRol = await _RolBusiness.CreateRolAsync(RolDto);
                return CreatedAtAction(nameof(GetRolById), new { id = createdRol.Id }, createdRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear Rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Rol");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        ///<summary>
        /// Elimina un rol físicamente del sistema
        ///</summary>
        ///<param name="id">ID del rol a eliminar</param>
        ///<response code="204">Rol eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Rol no encontrado</response>
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
                await _RolBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un rol (cambio de estado)
        ///</summary>
        ///<param name="id">ID del rol a eliminar lógicamente</param>
        ///<response code="200">Rol desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Rol no encontrado</response>
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
                await _RolBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Rol eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente rol con ID: {RolId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Rol no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente rol con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
