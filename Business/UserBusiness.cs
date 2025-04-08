using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.  
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetUserAllAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return MapToList(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var user = MapToEntity(userDto);
                var created = await _userData.CreateAsync(user);

                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {UserName}", userDto?.UserName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        public async Task<bool> UpdateUserAsync(UserDto userDto)
        {
            ValidateUser(userDto);

            try
            {
                var user = await _userData.GetByIdAsync(userDto.Id);
                if (user == null)
                {
                    _logger.LogInformation("Usuario no encontrado con ID: {UserId}", userDto.Id);
                    throw new EntityNotFoundException("User", userDto.Id);
                }

                var updatedEntity = MapToEntity(userDto);
                return await _userData.UpdateAsync(updatedEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {UserId}", userDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario con ID {userDto.Id}", ex);
            }
        }

        /// <summary>
        /// Elimina lógicamente un usuario.
        /// </summary>
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("Usuario no encontrado con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                user.Active = false;
                await _userData.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el usuario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Elimina físicamente un usuario.
        /// </summary>
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("Usuario no encontrado con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                await _userData.DeletePersistenceAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        // --- MÉTODOS PRIVADOS DE APOYO ---

        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con UserName vacío");
                throw new ValidationException("UserName", "El nombre de usuario es obligatorio");
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                Active = user.Active
            };
        }
        private User MapToEntity(UserDto userdto)
        {
            return new User
            {
                Id = userdto.Id,
                UserName = userdto.UserName,
                ProfilePhotoUrl = userdto.ProfilePhotoUrl,
                Active = userdto.Active
            };
        }
        private IEnumerable<UserDto> MapToList(IEnumerable<User> users)
        {
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                result.Add(MapToDto(user));
            }
            return result;
        }
    }
}
