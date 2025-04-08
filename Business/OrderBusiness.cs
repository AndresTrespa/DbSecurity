using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las órdenes de los consumidores.
    /// </summary>
    public class OrderBusiness
    {
        private readonly OrderData _orderData;
        private readonly ILogger<OrderBusiness> _logger;

        public OrderBusiness(OrderData orderData, ILogger<OrderBusiness> logger)
        {
            _orderData = orderData;
            _logger = logger;
        }

        // Obtener todas las órdenes como DTOs
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            try
            {
                var orders = await _orderData.GetAllAsync();
                var orderDtos = MapToList(orders);
                return orderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las órdenes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de órdenes", ex);
            }
        }

        // Obtener una orden por ID
        public async Task<OrderDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una orden con ID inválido: {OrderId}", id);
                throw new ValidationException("id", "El ID de la orden debe ser mayor que cero");
            }

            try
            {
                var order = await _orderData.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogInformation("Orden no encontrada con ID: {OrderId}", id);
                    throw new EntityNotFoundException("Order", id);
                }

                return MapToDTO(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden con ID: {OrderId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la orden con ID {id}", ex);
            }
        }

        // Crear una nueva orden
        public async Task<OrderDto> CreateAsync(OrderDto orderDto)
        {
            try
            {
                ValidateOrder(orderDto);

                var entity = new Order
                {
                    ConsumerId = orderDto.ConsumerId,
                    Status = orderDto.Status,
                    Note = orderDto.Note,
                    CreateAt = DateTime.UtcNow
                };

                var createdOrder = await _orderData.CreateAsync(entity);

                return MapToDTO(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva orden para el consumidor: {ConsumerId}", orderDto?.ConsumerId);
                throw new ExternalServiceException("Base de datos", "Error al crear la orden", ex);
            }
        }

        // Actualizar una orden
        public async Task<bool> UpdateAsync(OrderDto dto)
        {
            try
            {
                ValidateOrder(dto);

                var order = await _orderData.GetByIdAsync(dto.Id);
                if (order == null)
                {
                    _logger.LogInformation("No se encontró la orden con ID: {OrderId}", dto.Id);
                    throw new EntityNotFoundException("Order", dto.Id);
                }

                order.ConsumerId = dto.ConsumerId;
                order.Status = dto.Status;
                order.Note = dto.Note;

                return await _orderData.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la orden con ID: {OrderId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la orden con ID {dto.Id}", ex);
            }
        }

        // Eliminación física
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar físicamente una orden con ID inválido: {OrderId}", id);
                throw new ValidationException("id", "El ID de la orden debe ser mayor que cero");
            }

            try
            {
                var order = await _orderData.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogInformation("Orden no encontrada con ID: {OrderId}", id);
                    throw new EntityNotFoundException("Order", id);
                }

                await _orderData.DeletePersistenceAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente la orden con ID: {OrderId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar físicamente la orden con ID {id}", ex);
            }
        }

        // Eliminación lógica
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente una orden con ID inválido: {OrderId}", id);
                throw new ValidationException("id", "El ID de la orden debe ser mayor que cero");
            }

            try
            {
                var order = await _orderData.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogInformation("Orden no encontrada con ID: {OrderId}", id);
                    throw new EntityNotFoundException("Order", id);
                }

                order.DeleteAt = DateTime.UtcNow;
                await _orderData.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la orden con ID: {OrderId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente la orden con ID {id}", ex);
            }
        }

        // Validación de orden
        private void ValidateOrder(OrderDto dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto orden no puede ser nulo");

            if (dto.ConsumerId <= 0)
            {
                _logger.LogWarning("Orden con ConsumerId inválido: {ConsumerId}", dto.ConsumerId);
                throw new ValidationException("ConsumerId", "El ID del consumidor es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                _logger.LogWarning("Orden con estado vacío");
                throw new ValidationException("Status", "El estado de la orden es obligatorio");
            }
        }

        // Mapear de entidad a DTO
        private OrderDto MapToDTO(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                ConsumerId = order.ConsumerId,
                Status = order.Status,
                Note = order.Note,
                CreateAt = order.CreateAt,
                DeleteAt = order.DeleteAt
            };
        }

        // Mapear lista de entidades a lista de DTOs
        private IEnumerable<OrderDto> MapToList(IEnumerable<Order> orders)
        {
            var dtoList = new List<OrderDto>();
            foreach (var order in orders)
            {
                dtoList.Add(MapToDTO(order));
            }
            return dtoList;
        }
    }
}
