using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los consumidores del sistema.
    /// </summary>
    public class ConsumerBusiness
    {
        private readonly ConsumerData _consumerData;
        private readonly ILogger<ConsumerBusiness> _logger;

        public ConsumerBusiness(ConsumerData consumerData, ILogger<ConsumerBusiness> logger)
        {
            _consumerData = consumerData;
            _logger = logger;
        }

        // Obtener todos los consumidores como DTOs
        public async Task<IEnumerable<ConsumerDto>> GetAllAsync()
        {
            try
            {
                var consumers = await _consumerData.GetAllAsync();
                return MapToList(consumers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los consumidores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de consumidores", ex);
            }
        }

        // Obtener un consumidor por ID como DTO
        public async Task<ConsumerDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un consumidor con ID inválido: {ConsumerId}", id);
                throw new ValidationException("id", "El ID del consumidor debe ser mayor que cero");
            }

            try
            {
                var consumer = await _consumerData.GetByIdAsync(id);
                if (consumer == null)
                {
                    _logger.LogInformation("No se encontró ningún consumidor con ID: {ConsumerId}", id);
                    throw new EntityNotFoundException("Consumer", id);
                }

                return MapToDTO(consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el consumidor con ID: {ConsumerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el consumidor con ID {id}", ex);
            }
        }

        // Crear un nuevo consumidor desde un DTO
        public async Task<ConsumerDto> CreateAsync(ConsumerDto dto)
        {
            try
            {
                ValidateConsumer(dto);

                var consumer = new Consumer
                {
                    Active = dto.Active
                };

                var creado = await _consumerData.CreateAsync(consumer);

                return new ConsumerDto
                {
                    Id = creado.Id,
                    Active = creado.Active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo consumidor");
                throw new ExternalServiceException("Base de datos", "Error al crear el consumidor", ex);
            }
        }

        // Actualizar un consumidor existente
        public async Task<bool> UpdateAsync(ConsumerDto dto)
        {
            ValidateConsumer(dto);

            try
            {
                var consumer = await _consumerData.GetByIdAsync(dto.Id);
                if (consumer == null)
                {
                    _logger.LogInformation("No se encontró el consumidor con ID: {ConsumerId}", dto.Id);
                    throw new EntityNotFoundException("Consumer", dto.Id);
                }

                consumer.Active = dto.Active;

                await _consumerData.UpdateAsync(consumer);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar consumidor con ID: {ConsumerId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el consumidor con ID {dto.Id}", ex);
            }
        }

        // Eliminación lógica de un consumidor
        public async Task<bool> DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un consumidor con ID inválido: {ConsumerId}", id);
                throw new ValidationException("id", "El ID del consumidor debe ser mayor que cero");
            }

            try
            {
                var consumer = await _consumerData.GetByIdAsync(id);
                if (consumer == null)
                {
                    _logger.LogInformation("No se encontró el consumidor con ID: {ConsumerId}", id);
                    return false;
                }

                consumer.DeleteAt = DateTime.UtcNow;
                await _consumerData.UpdateAsync(consumer);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el consumidor con ID: {ConsumerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el consumidor con ID {id}", ex);
            }
        }

        // Eliminación física por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar físicamente un consumidor con ID inválido: {ConsumerId}", id);
                throw new ValidationException("id", "El ID del consumidor debe ser mayor que cero");
            }

            try
            {
                var consumer = await _consumerData.GetByIdAsync(id);
                if (consumer == null)
                {
                    _logger.LogInformation("No se encontró el consumidor con ID: {ConsumerId}", id);
                    return false;
                }

                await _consumerData.DeletePersistenceAsync(consumer);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente el consumidor con ID: {ConsumerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el consumidor con ID {id}", ex);
            }
        }

        // Eliminación física desde DTO
        public async Task<bool> DeletePersistenceAsync(ConsumerDto ConsumerDto)
        {
            if (ConsumerDto == null || ConsumerDto.Id <= 0)
            {
                throw new ValidationException("id", "El ID del consumidor debe ser mayor que cero");
            }

            return await DeletePersistenceAsync(ConsumerDto.Id);
        }

        // Validar DTO
        private void ValidateConsumer(ConsumerDto ConsumerDto)
        {
            if (ConsumerDto == null)
            {
                throw new ValidationException("El objeto consumidor no puede ser nulo");
            }

            // Puedes agregar más validaciones aquí según los campos de tu entidad
        }

        // Mapear entidad a DTO
        private ConsumerDto MapToDTO(Consumer consumer)
        {
            return new ConsumerDto
            {
                Id = consumer.Id,
                Active = consumer.Active
            };
        }

        // Mapear ConsumerDto de entidades a lista de DTOs
        private IEnumerable<ConsumerDto> MapToList(IEnumerable<Consumer> consumers)
        {
            var ConsumerDto = new List<ConsumerDto>();
            foreach (var item in consumers)
            {
                ConsumerDto.Add(MapToDTO(item));
            }
            return ConsumerDto;
        }
    }
}
