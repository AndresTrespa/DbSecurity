using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    ///<summary>
    /// Controlador para la gestión de órdenes en el sistema
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly OrderBusiness _orderBusiness;
        private readonly ILogger<OrderController> _logger;

        ///<summary>
        /// Constructor del controlador de órdenes
        ///</summary>
        ///<param name="orderBusiness">Capa de negocio de órdenes</param>
        ///<param name="logger">Logger para registro de eventos</param>
        public OrderController(OrderBusiness orderBusiness, ILogger<OrderController> logger)
        {
            _orderBusiness = orderBusiness;
            _logger = logger;
        }

        ///<summary>
        /// Obtiene todas las órdenes del sistema
        ///</summary>
        ///<returns>Lista de órdenes</returns>
        ///<response code="200">Retorna la lista de órdenes</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderBusiness.GetAllAsync();
                return Ok(orders);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Obtiene una orden específica por su ID
        ///</summary>
        ///<param name="id">ID de la orden</param>
        ///<returns>Orden solicitada</returns>
        ///<response code="200">Retorna la orden solicitada</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Orden no encontrada</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderBusiness.GetByIdAsync(id);
                return Ok(order);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para la orden con ID: {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Orden no encontrada con ID: {OrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener la orden con ID: {OrderId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Crea una nueva orden en el sistema
        ///</summary>
        ///<param name="orderDto">Datos de la orden a crear</param>
        ///<returns>Orden creada</returns>
        ///<response code="201">Retorna la orden creada</response>
        ///<response code="400">Datos no válidos</response>
        ///<response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            try
            {
                var createdOrder = await _orderBusiness.CreateAsync(orderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear orden");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear orden");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina físicamente una orden del sistema
        ///</summary>
        ///<param name="id">ID de la orden a eliminar</param>
        ///<response code="204">Orden eliminada exitosamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Orden no encontrada</response>
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
                await _orderBusiness.DeletePersistenceAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar orden con ID: {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Orden no encontrada con ID: {OrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar orden con ID: {OrderId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        ///<summary>
        /// Elimina lógicamente una orden (cambio de estado)
        ///</summary>
        ///<param name="id">ID de la orden a eliminar lógicamente</param>
        ///<response code="200">Orden eliminada lógicamente</response>
        ///<response code="400">ID no válido</response>
        ///<response code="404">Orden no encontrada</response>
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
                await _orderBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Orden eliminada lógicamente con éxito" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar lógicamente orden con ID: {OrderId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Orden no encontrada con ID: {OrderId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente orden con ID: {OrderId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
