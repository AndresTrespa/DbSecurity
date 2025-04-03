using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class permisoBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger _logger;
    public permisoBusiness(PermissionData permisoData, ILogger logger)
        {
            _permissionData = permisoData;
            _logger = logger;
        }
        public async Task<IEnumerable<PermissionDto>> GetpermisosAsync()
        {
            try
            {
                var permisos = await _permissionData.GetPermissionAllsAsync();
                var permisosDTO = new List<PermissionDto>();
                foreach (var permiso in permisos)
                {
                    permisosDTO.Add(new PermissionDto
                    {
                        Id = permiso.Id,
                        Name = permiso.Name,
                        Description = permiso.Description
                    });
                }
                return permisosDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }
        public async Task<PermissionDto> GetpermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permission con ID inválido: {permiso}", id);
                throw new Utilities.Exeptions.ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }
            try
            {
                var permission = await _permissionData.GetbyIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("Se intentó obtener un permission con ID inválido: {permiso}", id);
                    throw new EntityNotFoundException("permission", id);
                }
                return new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {permiso}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el permiso", ex);
            }
        }
        public async Task<PermissionDto> CreatePermissionAsync(PermissionDto PermissionDto)
        {
            try
            {
                ValidatePermission(PermissionDto);
                var Permission = new Permission
                {
                    Name = PermissionDto.Name,
                    Description = PermissionDto.Description
                };
                var PermissionCreado = await _permissionData.CreateAsync(Permission);

                return new PermissionDto
                {
                    Id = PermissionCreado.Id,
                    Description = PermissionCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Permission: {PermissionName}", PermissionDto?.Name ?? "Null");
                throw new ExternalServiceException("Base de datos", "Error al crear el form", ex);
            }
        }
        private void ValidatePermission(PermissionDto PermissionDto)
        {
            if (PermissionDto == null)
            {
                throw new Utilities.Exeptions.ValidationException("El objeto form no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(PermissionDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Permission con Name vacío");
                throw new Utilities.Exeptions.ValidationException("Name del form es obligatorio");
            }
        }
        private PermissionDto MapToDTO(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            };
        }
        private Permission MapToEntity(PermissionDto permissionDto)
        {
            return new Permission
            {
                Id = permissionDto.Id,
                Name = permissionDto.Name,
                Description = permissionDto.Description
            };
        }
        private IEnumerable<PermissionDto> MapToList(IEnumerable<Permission> permissions)
        {
            var PermissionDTO = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                PermissionDTO.Add(MapToDTO(permission));
            }
            return PermissionDTO;
        }
    }
}
