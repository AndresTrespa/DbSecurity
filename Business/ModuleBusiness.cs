using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    class ModuleBusiness
    {
        private readonly ModuleData _modelData;
        private readonly ILogger _logger;
        public ModuleBusiness(ModuleData modeldata, ILogger logger)
        {
            _modelData = modeldata;
            _logger = logger;
        }
        public async Task<IEnumerable<ModuleDto>> GetAllModuleAsync()
        {
            try
            {
                var modules = await _modelData.GetAllModulesAsync();
                var ModuleDto = new List<ModuleDto>();

                foreach (var module in modules)
                    ModuleDto.Add(new ModuleDto
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Description = module.Description
                    });
                return ModuleDto;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        public async Task<ModuleDto> GetAllModulesAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un module con ID inválido: {ModelId}", id);
                throw new Utilities.Exeptions.ValidationException("id", "El ID del module debe ser mayor que cero");
            }

            try
            {
                var module = await _modelData.GetbyIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontró ningún Module con ID: {ModuleId}", id);
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
                _logger.LogError(ex, "Error al obtener el module con ID: {moduleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un module desde un DTO
        public async Task<ModuleDto> CreateModuleAsync(ModuleDto ModuleDto)
        {
            try
            {
                ValidateModule(ModuleDto);

                var module = new Module
                {
                    Name = ModuleDto.Name,
                    Description = ModuleDto.Description // Si existe en la entidad
                };

                var moduleCreado = await _modelData.CreateModuleAsync(module);

                return new ModuleDto
                {
                    Id = moduleCreado.Id,
                    Name = moduleCreado.Name,
                    Description = moduleCreado.Description // Si existe en la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", ModuleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateModule(ModuleDto ModuleDto)
        {
            if (ModuleDto == null)
            {
                throw new Utilities.Exeptions.ValidationException("El objeto module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ModuleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un module con Name vacío");
                throw new Utilities.Exeptions.ValidationException("Name", "El Name del rol es obligatorio");//trae el newspapers de "BussinesException"
            }
        }
        //Método para mapear de module a ModuleDto
        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description //si existe la entidad
            };
        }
        //Método para mapear de ModuleDto a Module
        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description
            };
        }
        //Método para mapear una lista de Module a una lista de RolDTO
        private IEnumerable<ModuleDto> MapToList(IEnumerable<Module> modules)
        {
            var ModuleDTO = new List<ModuleDto>();
            foreach (var module in modules)
            {
                ModuleDTO.Add(MapToDTO(module));
            }
            return ModuleDTO;
        }
    }
}
