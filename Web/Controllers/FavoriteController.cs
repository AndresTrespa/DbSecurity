using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Favoritos en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FavoriteController : ControllerBase
    {
        private readonly FavoriteBusiness _favoriteBusiness;
        private readonly ILogger<FavoriteController> _logger;

        ///<summary>
        /// Constructor del controlador de Favoritos
        ///</summary>
        ///<param name="favoriteBusiness">Capa de negocio de Favoritos</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public FavoriteController(FavoriteBusiness favoriteBusiness, ILogger<FavoriteController> logger)
        {
            _favoriteBusiness = favoriteBusiness;
            _logger = logger;
        }

        ///<summary>
        /// Obtiene todos los Favoritos del sistema
        ///</summary>
        ///<returns>Lista de Favoritos</returns>
        ///<response code="200">Retorna la lista de Favoritos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FavoriteDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFavorites()
        {
            try
            {
                var favorites = await _favoriteBusiness.GetFavoriteAllAsync();
                return Ok(favorites);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener favoritos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtiene un Favorito específico por su ID
        ///</summary>
        ///<param name="id">ID del favorito</param>
        ///<returns>Favorito solicitado</returns>
        ///<response code="200">Retorna el favorito solicitado</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Favorito no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FavoriteDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFavoriteById(int id)
        {
            try
            {
                var favorite = await _favoriteBusiness.GetFavoriteByIdAsync(id);
                return Ok(favorite);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el favorito con ID: {FavoriteId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Favorito no encontrado con ID: {FavoriteId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener favorito con ID: {FavoriteId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Crea un nuevo Favorito en el sistema
        ///</summary>
        ///<param name="favoriteDto">Datos del Favorito a crear</param>
        ///<returns>Favorito creado</returns>
        ///<response code="201">Retorna el favorito creado</response>
        ///<response code="400">Datos del favorito no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(FavoriteDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFavorite([FromBody] FavoriteDto favoriteDto)
        {
            try
            {
                var createdFavorite = await _favoriteBusiness.CreateFavoriteAsync(favoriteDto);
                return CreatedAtAction(nameof(GetFavoriteById), new { id = createdFavorite.Id }, createdFavorite);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear favorito");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear favorito");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina un Favorito físicamente del sistema
        ///</summary>
        ///<param name="id">ID del Favorito a eliminar</param>
        ///<response code="204">Favorito eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Favorito no encontrado</response>
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
                await _favoriteBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar favorito con ID: {FavoriteId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Favorito no encontrado con ID: {FavoriteId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar favorito con ID: {FavoriteId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un Favorito (cambio de estado)
        ///</summary>
        ///<param name="id">ID del favorito a eliminar lógicamente</param>
        ///<response code="200">Favorito desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Favorito no encontrado</response>
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
                await _favoriteBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Favorito eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente favorito con ID: {FavoriteId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Favorito no encontrado con ID: {FavoriteId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente favorito con ID: {FavoriteId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
