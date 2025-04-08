using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de RolUsers en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolUserController : ControllerBase
    {
        private readonly RolUserBusiness _RolUserBusiness;
        private readonly ILogger<RolUserController> _logger;

        ///<summary>
        ///Constructor del controlador de RolUsers
        ///</summary>
        ///<param name="RolUserBusiness">Capa de negocio de RolUsers</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public RolUserController(RolUserBusiness RolUserBusiness, ILogger<RolUserController> logger)
        {
            _RolUserBusiness = RolUserBusiness;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los RolUsers del Sistema
        ///</summary>
        ///<returns>Lista de RolUsers</returns>
        ///<response code="200">Retorna la lista de RolUsers</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var rolUsers = await _RolUserBusiness.GetRolUserAllAsync();
                return Ok(rolUsers);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener RolUsers");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtiene un RolUser específico por su ID
        ///</summary>
        ///<returns>RolUser solicitado</returns>
        ///<response code="200">Retorna el RolUser solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">RolUser no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                var rolUser = await _RolUserBusiness.GetRolUserByIdAsync(id);
                return Ok(rolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el RolUser con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolUser no encontrado con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener RolUser con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Crea un nuevo RolUser en el sistema
        ///</summary>
        ///<param name="RolUserDto">Datos del RolUser a crear</param>
        ///<returns>RolUser creado</returns>
        ///<response code="201">Retorna el RolUser creado</response>
        ///<response code="400">Datos del RolUser no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolUser([FromBody] RolUserDto RolUserDto)
        {
            try
            {
                var createdRolUser = await _RolUserBusiness.CreateRolUserAsync(RolUserDto);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear RolUser");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear RolUser");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina un RolUser físicamente del sistema
        ///</summary>
        ///<param name="id">ID del RolUser a eliminar</param>
        ///<response code="204">RolUser eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">RolUser no encontrado</response>
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
                await _RolUserBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar RolUser con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolUser no encontrado con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar RolUser con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un RolUser (cambio de estado)
        ///</summary>
        ///<param name="id">ID del RolUser a eliminar lógicamente</param>
        ///<response code="200">RolUser desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">RolUser no encontrado</response>
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
                await _RolUserBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "RolUser eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente RolUser con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolUser no encontrado con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente RolUser con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
