using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class ModuleBusiness
    {
        private readonly ModuleData _modelData;
        private readonly ILogger<ModuleBusiness> _logger;
        public ModuleBusiness(ModuleData modeldata, ILogger<ModuleBusiness> logger)
        {
            _modelData = modeldata;
            _logger = logger;
        }
        public async Task<IEnumerable<ModuleDto>> GetAllModule()
        {
            try
            {
                var modules = await _modelData.GetAllModulesAsync();
                var moduleDto = MapToList(modules);
                return moduleDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los module");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de module", ex);
            }

        }
        public async Task<ModuleDto> GetModuleById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un module con ID inválido: {moduleId}", id);
                throw new ValidationException("id", "El ID del module debe ser mayor que cero");
            }

            try
            {
                var module = await _modelData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("module no encontrado con ID: {moduleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                return new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el module", ex);
            }
        }

        public async Task<ModuleDto> CreateModule(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var Module = new Module
                {
                    Name = moduleDto.Name,
                    Description = moduleDto.Description
                };

                var ModuleCreado = await _modelData.CreateAsync(Module);

                return new ModuleDto
                {
                    Id = ModuleCreado.Id,
                    Name = ModuleCreado.Name,
                    Description = ModuleCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Module: {ModuleName}", moduleDto?.Name ?? "Null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Module", ex);
            }
        }

        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new ValidationException("El objeto Module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Module con Name vacío");
                throw new ValidationException("Name del Module es obligatorio");
            }
        }

        // Eliminación física de un Module
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un Module con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del Module debe ser mayor que cero");
            }
            try
            {
                var Module = await _modelData.GetByIdAsync(id);
                if (Module == null)
                {
                    _logger.LogInformation("Module no encontrado con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                await _modelData.DeletePersistenceAsync(Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el Module con ID {id}", ex);
            }
        }
        // Eliminación lógica de un Module
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un Module con ID inválido: {ModuleId}", id);
                throw new ValidationException("id", "El ID del Module debe ser mayor que cero");
            }

            try
            {
                var Module = await _modelData.GetByIdAsync(id);
                if (Module == null)
                {
                    _logger.LogInformation("Module no encontrado con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }

                await _modelData.UpdateAsync(Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Module con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el Module con ID {id}", ex);
            }
        }

        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description
            };
        }

        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description
            };
        }

        private IEnumerable<ModuleDto> MapToList(IEnumerable<Module> modules)
        {
            var ModulesDTO = new List<ModuleDto>();
            foreach (var Module in modules)
            {
                ModulesDTO.Add(MapToDTO(Module));
            }
            return ModulesDTO;
        }
    }
}
