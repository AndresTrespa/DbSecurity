using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los productores del sistema.
    /// </summary>
    public class ProducerBusiness
    {
        private readonly ProducerData _producerData;
        private readonly ILogger<ProducerBusiness> _logger;

        public ProducerBusiness(ProducerData producerData, ILogger<ProducerBusiness> logger)
        {
            _producerData = producerData;
            _logger = logger;
        }

        // Método para obtener todos los productores como DTOs
        public async Task<IEnumerable<ProducerDto>> GetProducerAllAsync()
        {
            try
            {
                var producers = await _producerData.GetAllAsync();
                return MapToList(producers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productores");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de productores", ex);
            }
        }

        // Método para obtener un productor por ID como DTO
        public async Task<ProducerDto> GetProducerByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un productor con ID inválido: {ProducerId}", id);
                throw new ValidationException("id", "El ID del productor debe ser mayor que cero");
            }

            try
            {
                var producer = await _producerData.GetByIdAsync(id);
                if (producer == null)
                {
                    _logger.LogInformation("No se encontró ningún productor con ID: {ProducerId}", id);
                    throw new EntityNotFoundException("Producer", id);
                }

                return MapToDTO(producer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el productor con ID: {ProducerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el productor con ID {id}", ex);
            }
        }

        // Método para crear un nuevo productor desde un DTO
        public async Task<ProducerDto> CreateProducerAsync(ProducerDto producerDto)
        {
            try
            {
                ValidateProducer(producerDto);

                var producer = new Producer
                {
                    Address = producerDto.Address
                };

                var creado = await _producerData.CreateAsync(producer);

                return MapToDTO(creado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo productor: {ProducerAddress}", producerDto?.Address ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el productor", ex);
            }
        }

        // Método para actualizar un productor
        public async Task UpdateProducerAsync(ProducerDto producerDto)
        {
            if (producerDto.Id <= 0)
            {
                _logger.LogWarning("ID inválido al intentar actualizar productor: {ProducerId}", producerDto.Id);
                throw new ValidationException("id", "El ID del productor debe ser mayor que cero");
            }

            try
            {
                ValidateProducer(producerDto);

                var existing = await _producerData.GetByIdAsync(producerDto.Id);
                if (existing == null)
                {
                    _logger.LogInformation("No se encontró ningún productor con ID: {ProducerId}", producerDto.Id);
                    throw new EntityNotFoundException("Producer", producerDto.Id);
                }

                existing.Address = producerDto.Address;
                await _producerData.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el productor con ID: {ProducerId}", producerDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el productor con ID {producerDto.Id}", ex);
            }
        }

        // Eliminación lógica de un productor
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un productor con ID inválido: {ProducerId}", id);
                throw new ValidationException("id", "El ID del productor debe ser mayor que cero");
            }

            try
            {
                var producer = await _producerData.GetByIdAsync(id);
                if (producer == null)
                {
                    _logger.LogInformation("No se encontró el productor con ID: {ProducerId}", id);
                    throw new EntityNotFoundException("Producer", id);
                }

                await _producerData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el productor con ID: {ProducerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el productor con ID {id}", ex);
            }
        }

        // Eliminación física de un productor
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un productor con ID inválido: {ProducerId}", id);
                throw new ValidationException("id", "El ID del productor debe ser mayor que cero");
            }

            try
            {
                var producer = await _producerData.GetByIdAsync(id);
                if (producer == null)
                {
                    _logger.LogInformation("No se encontró el productor con ID: {ProducerId}", id);
                    throw new EntityNotFoundException("Producer", id);
                }

                await _producerData.DeletePersistenceAsync(producer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el productor con ID: {ProducerId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el productor con ID {id}", ex);
            }
        }

        // Validación del DTO del productor
        private void ValidateProducer(ProducerDto dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto productor no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                _logger.LogWarning("Se intentó crear/actualizar un productor con dirección vacía");
                throw new ValidationException("Address", "La dirección del productor es obligatoria");
            }
        }

        // Mapear de entidad a DTO
        private ProducerDto MapToDTO(Producer producer)
        {
            return new ProducerDto
            {
                Id = producer.Id,
                Address = producer.Address
            };
        }

        // Mapear de DTO a entidad
        private Producer MapToEntity(Producer producer)
        {
            return new Producer
            {
                Id = producer.Id,
                Address = producer.Address
            };
        }

        // Mapear lista de entidades a lista de DTOs
        private IEnumerable<ProducerDto> MapToList(IEnumerable<Producer> producers)
        {
            var ProducerDto = new List<ProducerDto>();
            foreach (var producer in producers)
            {
                ProducerDto.Add(MapToDTO(producer));
            }
            return ProducerDto;
        }
    }
}
