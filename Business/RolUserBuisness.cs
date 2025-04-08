using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los RolUser del sistema.  
    /// </summary>
    public class RolUserBusiness
    {
        private readonly RolUserData _rolUserData;
        private readonly ILogger<RolUserBusiness> _logger;

        public RolUserBusiness(RolUserData rolUserData, ILogger<RolUserBusiness> logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }
        // Método para obtener todos los RolUser como DTOs
        public async Task<IEnumerable<RolUserDto>> GetRolUserAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                var rolUsersDTO = MapToList(rolUsers);
                return rolUsersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolUser");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolUser", ex);
            }
        }
        // Método para obtener un RolUser por ID como DTO
        public async Task<RolUserDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un RolUser con ID inválido: {RolUserId}", id);
                throw new ValidationException("id", "El ID del RolUser debe ser mayor que cero");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("No se encontró ningún RolUser con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                return new RolUserDto
                {
                    Id = rolUser.Id,
                    UserId = rolUser.UserId,
                    RolId = rolUser.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolUser con ID {id}", ex);
            }
        }
        // Método para crear un RolUser desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);

                var rolUser = new RolUser
                {
                    UserId = rolUserDto.UserId,
                    RolId = rolUserDto.RolId,
                };

                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);

                return new RolUserDto
                {
                    Id = rolUserCreado.Id,
                    UserId = rolUserCreado.UserId,
                    RolId = rolUserCreado.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo RolUser: {UserId}", rolUserDto?.UserId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear el RolUser", ex);
            }
        }
        // Método para validar el DTO
        private void ValidateRolUser(RolUserDto rolUserDto)
        {
            if (rolUserDto == null)
            {
                throw new ValidationException("El objeto RolUser no puede ser nulo");
            }

            if (rolUserDto.UserId <= 0 || rolUserDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un RolUser con datos inválidos");
                throw new ValidationException("UserId/RolId", "UserId y RolId deben ser mayores que cero");
            }
        }
        // Eliminación física de un RolUser
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un RolUser con ID inválido: {RolUserId}", id);
                throw new ValidationException("id", "El ID del RolUser debe ser mayor que cero");
            }
            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("RolUser no encontrado con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                await _rolUserData.DeletePersistenceAsync(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el RolUser con ID {id}", ex);
            }
        }
        // Eliminación lógica de un RolUser
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un RolUser con ID inválido: {RolUserId}", id);
                throw new ValidationException("id", "El ID del RolUser debe ser mayor que cero");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("RolUser no encontrado con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }
                await _rolUserData.UpdateAsync(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el RolUser con ID {id}", ex);
            }
        }
        //Método para mapear de RolUser a RolUserDTO
        private RolUserDto MapToDTO(RolUser rolUser)
        {
            return new RolUserDto
            {
                Id = rolUser.Id,
                UserId = rolUser.UserId,
                RolId = rolUser.RolId
            };
        }
        //Método para mapear de RolUserDTO a RolUser
        private RolUser MapToEntity(RolUserDto rolUserDto)
        {
            return new RolUser
            {
                Id = rolUserDto.Id,
                UserId = rolUserDto.UserId,
                RolId = rolUserDto.RolId
            };
        }
        //Método para mapear una lista de RolUser a una lista de RolUserDTO
        private IEnumerable<RolUserDto> MapToList(IEnumerable<RolUser> rolUsers)
        {
            var rolUsersDTO = new List<RolUserDto>();
            foreach (var rolUser in rolUsers)
            {
                rolUsersDTO.Add(MapToDTO(rolUser));
            }
            return rolUsersDTO;
        }
    }
}