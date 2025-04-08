using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con la relación entre formularios y módulos.
    /// </summary>
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormModuleBusiness> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Método para obtener todas las relaciones FormModule
        public async Task<IEnumerable<FormModuleDto>> GetAllAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                var formModuleDto = MapToList(formModules);
                return formModuleDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones FormModule");
                throw new ExternalServiceException("Base de datos", "Error al recuperar las relaciones entre formularios y módulos", ex);
            }
        }

        // Método para obtener una relación específica por ID
        public async Task<FormModuleDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido para obtener FormModule: {Id}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var entity = await _formModuleData.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation("FormModule no encontrado con ID: {Id}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                return MapToDTO(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener FormModule con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar FormModule con ID {id}", ex);
            }
        }

        // Método para crear una nueva relación FormModule
        public async Task<FormModuleDto> CreateAsync(FormModuleDto dto)
        {
            try
            {
                Validate(dto);

                var entity = new FormModule
                {
                    FormId = dto.FormId,
                    ModuleId = dto.ModuleId
                };

                var created = await _formModuleData.CreateAsync(entity);

                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva relación FormModule");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación entre formulario y módulo", ex);
            }
        }

        // Eliminación física
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido para eliminar FormModule: {Id}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var entity = await _formModuleData.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation("FormModule no encontrado con ID: {Id}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                await _formModuleData.DeletePersistenceAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente FormModule con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar FormModule con ID {id}", ex);
            }
        }

        // Eliminación lógica
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido para eliminación lógica de FormModule: {Id}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var entity = await _formModuleData.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogInformation("FormModule no encontrado con ID: {Id}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }

                await _formModuleData.UpdateAsync(entity); // Suponiendo que UpdateAsync marca la eliminación lógica
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en eliminación lógica de FormModule con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente FormModule con ID {id}", ex);
            }
        }

        // Validaciones
        private void Validate(FormModuleDto FormModuleDto)
        {
            if (FormModuleDto == null)
                throw new ValidationException("El objeto FormModule no puede ser nulo");

            if (FormModuleDto.FormId <= 0)
            {
                _logger.LogWarning("FormId inválido en FormModule");
                throw new ValidationException("FormId", "FormId debe ser mayor que cero");
            }

            if (FormModuleDto.ModuleId <= 0)
            {
                _logger.LogWarning("ModuleId inválido en FormModule");
                throw new ValidationException("ModuleId", "ModuleId debe ser mayor que cero");
            }
        }

        // Mapeos
        private FormModuleDto MapToDTO(FormModule FormModules)
        {
            return new FormModuleDto
            {
                FormId = FormModules.FormId,
                ModuleId = FormModules.ModuleId
            };
        }
        private FormModule MapToEntity(FormModuleDto FormModuleDto)
        {
            return new FormModule
            {
                ModuleId = FormModuleDto.ModuleId,
                FormId = FormModuleDto.FormId
            };
        }
        private IEnumerable<FormModuleDto> MapToList(IEnumerable<FormModule> FormModules)
        {
            var FormModuleDto = new List<FormModuleDto>();
            foreach (var formModule in FormModules)
            {
                FormModuleDto.Add(MapToDTO(formModule));
            }
            return FormModuleDto;
        }

    }
}