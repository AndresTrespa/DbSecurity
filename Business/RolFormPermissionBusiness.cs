using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger<RolFormPermissionBusiness> _logger;

        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetAllAsync()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetAllAsync();
                var RolFormPermissionDto = MapToList(rolFormPermissions);
                return RolFormPermissionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones de RolFormPermission");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos de formulario por rol", ex);
            }
        }

        public async Task<RolFormPermissionDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID debe ser mayor que cero");

            try
            {
                var rolFormsPermissions = await _rolFormPermissionData.GetByIdAsync(id);
                if (rolFormsPermissions == null)
                    throw new EntityNotFoundException("RolFormPermission", id);

                return MapToDTO(rolFormsPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolFormPermission con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar RolFormPermission con ID {id}", ex);
            }
        }

        public async Task<RolFormPermissionDto> CreateAsync(RolFormPermissionDto rolFormPermissionDto)
        {
            try
            {
                ValidaterolFormPermission(rolFormPermissionDto);

                var rolFormsPermissions = new RolFormPermission
                {
                    RolId = rolFormPermissionDto.RolId,
                    PermissionId = rolFormPermissionDto.PermissionId
                };

                var rolFormsPermissionscreated = await _rolFormPermissionData.CreateAsync(rolFormsPermissions);

                return new RolFormPermissionDto
                {
                    Id = rolFormsPermissionscreated.Id,
                    RolId = rolFormsPermissionscreated.RolId,
                    PermissionId = rolFormsPermissionscreated.PermissionId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear RolFormPermission");
                throw new ExternalServiceException("Base de datos", "Error al crear RolFormPermission", ex);
            }
        }

        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID debe ser mayor que cero");

            try
            {
                var rolFormsPermissions = await _rolFormPermissionData.GetByIdAsync(id);
                if (rolFormsPermissions == null)
                    throw new EntityNotFoundException("RolFormPermission", id);

                await _rolFormPermissionData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar borrado lógico de RolFormPermission con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente RolFormPermission con ID {id}", ex);
            }
        }

        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID debe ser mayor que cero");

            try
            {
                var rolFormsPermissions = await _rolFormPermissionData.GetByIdAsync(id);
                if (rolFormsPermissions == null)
                    throw new EntityNotFoundException("RolFormPermission", id);

                await _rolFormPermissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar borrado físico de RolFormPermission con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar RolFormPermission con ID {id}", ex);
            }
        }

        private void ValidaterolFormPermission(RolFormPermissionDto RolFormPermissionDto)
        {
            if (RolFormPermissionDto == null)
                throw new ValidationException("El objeto RolFormPermission no puede ser nulo");

            if (RolFormPermissionDto.RolId <= 0)
                throw new ValidationException("RolId", "El RolId debe ser mayor que cero");

            if (RolFormPermissionDto.PermissionId <= 0)
                throw new ValidationException("PermissionId", "El FormPermissionId debe ser mayor que cero");
        }

        private RolFormPermissionDto MapToDTO(RolFormPermission RolFormPermissionDto)
        {
            return new RolFormPermissionDto
            {
                Id = RolFormPermissionDto.Id,
                RolId = RolFormPermissionDto.RolId,
                PermissionId = RolFormPermissionDto.PermissionId
            };
        }

        private RolFormPermission MapToEntity(RolFormPermissionDto RolFormPermissionDto)
        {
            return new RolFormPermission
            {
                Id = RolFormPermissionDto.Id,
                RolId = RolFormPermissionDto.RolId,
                PermissionId = RolFormPermissionDto.PermissionId
            };
        }

        private IEnumerable<RolFormPermissionDto> MapToList(IEnumerable<RolFormPermission> RolFormPermissions)
        {
            var RolFormPermissionDto = new List<RolFormPermissionDto>();
            foreach (var RolFormPermission in RolFormPermissions)
            {
                RolFormPermissionDto.Add(MapToDTO(RolFormPermission));
            }
            return RolFormPermissionDto;
        }
    }
}
