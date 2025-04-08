using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Categorías en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryBusiness _CategoryBusiness;
        private readonly ILogger<CategoryController> _logger;

        ///<summary>
        ///Constructor del controlador de Categorías
        ///</summary>
        ///<param name="CategoryBusiness">Capa de negocio de Categorías</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public CategoryController(CategoryBusiness CategoryBusiness, ILogger<CategoryController> logger)
        {
            _CategoryBusiness = CategoryBusiness;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todas las Categorías del Sistema
        ///</summary>
        ///<returns>Lista de Categorías</returns>
        ///<response code="200">Retorna la lista de Categorías</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _CategoryBusiness.GetAllCategoryAsync();
                return Ok(categories);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Categorías");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtiene una Categoría específica por su ID
        ///</summary>
        ///<returns>Categoría solicitada</returns>
        ///<response code="200">Retorna la Categoría solicitada</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">Categoría no encontrada</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _CategoryBusiness.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la Categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Crea una nueva Categoría en el sistema
        ///</summary>
        ///<param name="CategoryDto">Datos de la Categoría a crear</param>
        ///<returns>Categoría creada</returns>
        ///<response code="201">Retorna la Categoría creada</response>
        ///<response code="400">Datos de la Categoría no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto CategoryDto)
        {
            try
            {
                var createdCategory = await _CategoryBusiness.CreateCategoryAsync(CategoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear Categoría");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Categoría");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina una categoría físicamente del sistema
        ///</summary>
        ///<param name="id">ID de la categoría a eliminar</param>
        ///<response code="204">Categoría eliminada exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Categoría no encontrada</response>
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
                await _CategoryBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente una categoría (cambio de estado)
        ///</summary>
        ///<param name="id">ID de la categoría a eliminar lógicamente</param>
        ///<response code="200">Categoría desactivada correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Categoría no encontrada</response>
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
                await _CategoryBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Categoría eliminada lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente categoría con ID: {CategoryId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Categoría no encontrada con ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente categoría con ID: {CategoryId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
