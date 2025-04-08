using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de Reviews en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewBusiness _ReviewBusiness;
        private readonly ILogger<ReviewController> _logger;

        ///<summary>
        ///Constructor del controlador de Reviews
        ///</summary>
        ///<param name="ReviewBusiness">Capa de negocio de Reviews</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public ReviewController(ReviewBusiness ReviewBusiness, ILogger<ReviewController> logger)
        {
            _ReviewBusiness = ReviewBusiness;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los Reviews del sistema
        ///</summary>
        ///<returns>Lista de Reviews</returns>
        ///<response code="200">Retorna la lista de Reviews</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReviewDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _ReviewBusiness.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Reviews");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Obtiene un Review específico por su ID
        ///</summary>
        ///<param name="id">ID del Review</param>
        ///<returns>Review solicitado</returns>
        ///<response code="200">Retorna el Review solicitado</response>
        ///<response code="400">ID proporcionado no válido</response>
        ///<response code="404">Review no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{reviewId}/{productId}")]
        [ProducesResponseType(typeof(ReviewDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewByIdAsync(int reviewId, int productId)
        {
            try
            {
                var review = await _ReviewBusiness.GetReviewByIdAsync(reviewId, productId);
                return Ok(review);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el Review con ID: {ReviewId}", reviewId, productId);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Review no encontrado con ID: {ReviewId}", reviewId, productId);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener Review con ID: {ReviewId}", reviewId, productId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        ///Crea un nuevo Review en el sistema
        ///</summary>
        ///<param name="ReviewDto">Datos del Review a crear</param>
        ///<returns>Review creado</returns>
        ///<response code="201">Retorna el Review creado</response>
        ///<response code="400">Datos del Review no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost("{productId}/{reviewId}")]
        [ProducesResponseType(typeof(ReviewDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReview(int productId, int reviewId, [FromBody] ReviewDto reviewDto)
        {
            try
            {
                var createdReview = await _ReviewBusiness.CreateReviewAsync(productId, reviewId, reviewDto);
                return CreatedAtAction(nameof(GetReviewByIdAsync), new { Id = createdReview }, createdReview);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear Review");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear Review");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina un Review físicamente del sistema
        ///</summary>
        ///<param name="id">ID del Review a eliminar</param>
        ///<response code="204">Review eliminado exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Review no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpDelete("{reviewId}/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePersistenceAsync(int reviewId, int productId)
        {
            try
            {
                await _ReviewBusiness.DeletePersistenceAsync(reviewId, productId);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar review con ID: {ReviewId}", reviewId, productId);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Review no encontrado con ID: {ReviewId}", reviewId, productId);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar review con ID: {ReviewId}", reviewId, productId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente un Review (cambio de estado)
        ///</summary>
        ///<param name="id">ID del Review a eliminar lógicamente</param>
        ///<response code="200">Review desactivado correctamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Review no encontrado</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPatch("eliminar-logico/{reviewId}/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicAsync(int reviewId, int productId)
        {
            try
            {
                await _ReviewBusiness.DeleteLogicAsync(reviewId, productId);
                return Ok(new { message = "Review eliminado lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente review con ID: {ReviewId}", reviewId, productId);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Review no encontrado con ID: {ReviewId}", reviewId, productId);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente review con ID: {ReviewId}", reviewId, productId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

