using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class PermisoBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermisoBusiness> _logger;

        public PermisoBusiness(PermissionData permisoData, ILogger<PermisoBusiness> logger)
        {
            _permissionData = permisoData;
            _logger = logger;
        }

        public async Task<IEnumerable<PermisosDTO>> GetPermisosAsync()
        {
            try
            {
                var permisos = await _permissionData.GetAllAsync();
                var permisosDTO = MapToList(permisos);
                return permisosDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }

        public async Task<PermisosDTO> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso con ID inválido: {PermisoId}", id);
                throw new ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("Permiso no encontrado con ID: {PermisoId}", id);
                    throw new EntityNotFoundException("Permission", id);
                }

                return new PermisosDTO
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {PermisoId}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el permiso", ex);
            }
        }

        public async Task<PermisosDTO> CreatePermissionAsync(PermisosDTO permisosDTO)
        {
            try
            {
                ValidatePermission(permisosDTO);

                var permission = new Permission
                {
                    Name = permisosDTO.Name,
                    Description = permisosDTO.Description
                };

                var permissionCreado = await _permissionData.CreateAsync(permission);

                return new PermisosDTO
                {
                    Id = permissionCreado.Id,
                    Name = permissionCreado.Name,
                    Description = permissionCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso: {PermissionName}", permisosDTO?.Name ?? "Null");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        private void ValidatePermission(PermisosDTO permissionDto)
        {
            if (permissionDto == null)
            {
                throw new ValidationException("El objeto permiso no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(permissionDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un permiso con Name vacío");
                throw new ValidationException("Name del permiso es obligatorio");
            }
        }

        // Eliminación física de un permiso
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un permiso con ID inválido: {PermisoId}", id);
                throw new ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }
            try
            {
                var permiso = await _permissionData.GetByIdAsync(id);
                if (permiso == null)
                {
                    _logger.LogInformation("Permiso no encontrado con ID: {PermisoId}", id);
                    throw new EntityNotFoundException("Permiso", id);
                }

                await _permissionData.DeletePersistenceAsync(permiso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso con ID: {PermisoId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el permiso con ID {id}", ex);
            }
        }
        // Eliminación lógica de un permiso
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un permiso con ID inválido: {PermisoId}", id);
                throw new ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var permiso = await _permissionData.GetByIdAsync(id);
                if (permiso == null)
                {
                    _logger.LogInformation("Permiso no encontrado con ID: {PermisoId}", id);
                    throw new EntityNotFoundException("Permiso", id);
                }

                await _permissionData.UpdateAsync(permiso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el permiso con ID: {PermisoId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el permiso con ID {id}", ex);
            }
        }

        private PermisosDTO MapToDTO(Permission permission)
        {
            return new PermisosDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description
            };
        }

        private Permission MapToEntity(PermisosDTO permissionDto)
        {
            return new Permission
            {
                Id = permissionDto.Id,
                Name = permissionDto.Name,
                Description = permissionDto.Description
            };
        }

        private IEnumerable<PermisosDTO> MapToList(IEnumerable<Permission> permissions)
        {
            var permisosDTO = new List<PermisosDTO>();
            foreach (var permission in permissions)
            {
                permisosDTO.Add(MapToDTO(permission));
            }
            return permisosDTO;
        }
    }
}
