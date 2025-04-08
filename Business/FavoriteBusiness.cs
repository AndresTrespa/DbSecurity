using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los favoritos del sistema.
    /// </summary>
    public class FavoriteBusiness
    {
        private readonly FavoriteData _favoriteData;
        private readonly ILogger<FavoriteBusiness> _logger;

        public FavoriteBusiness(FavoriteData favoriteData, ILogger<FavoriteBusiness> logger)
        {
            _favoriteData = favoriteData;
            _logger = logger;
        }

        // Método para obtener todos los favoritos como DTOs
        public async Task<IEnumerable<FavoriteDto>> GetFavoriteAllAsync()
        {
            try
            {
                var favorites = await _favoriteData.GetAllAsync();
                var favoritesDTO = MapToList(favorites);
                return favoritesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los favoritos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de favoritos", ex);
            }
        }

        // Método para obtener un favorito por ID como DTO
        public async Task<FavoriteDto> GetFavoriteByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un favorito con ID inválido: {FavoriteId}", id);
                throw new ValidationException("id", "El ID del favorito debe ser mayor que cero");
            }

            try
            {
                var favorite = await _favoriteData.GetByIdAsync(id);
                if (favorite == null)
                {
                    _logger.LogInformation("No se encontró ningún favorito con ID: {FavoriteId}", id);
                    throw new EntityNotFoundException("Favorite", id);
                }

                return MapToDTO(favorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el favorito con ID: {FavoriteId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el favorito con ID {id}", ex);
            }
        }

        // Método para crear un favorito desde un DTO
        public async Task<FavoriteDto> CreateFavoriteAsync(FavoriteDto favoriteDto)
        {
            try
            {
                ValidateFavorite(favoriteDto);

                var favorite = MapToEntity(favoriteDto);
                var createdFavorite = await _favoriteData.CreateAsync(favorite);

                return MapToDTO(createdFavorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo favorito: {@Favorite}", favoriteDto);
                throw new ExternalServiceException("Base de datos", "Error al crear el favorito", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateFavorite(FavoriteDto favoriteDto)
        {
            if (favoriteDto == null)
            {
                throw new ValidationException("El objeto favorito no puede ser nulo");
            }

            if (favoriteDto.ConsumerId <= 0)
            {
                _logger.LogWarning("ConsumerId inválido en favorito");
                throw new ValidationException("ConsumerId", "El ID del consumidor debe ser mayor que cero");
            }

            if (favoriteDto.ProducerId <= 0)
            {
                _logger.LogWarning("ProducerId inválido en favorito");
                throw new ValidationException("ProducerId", "El ID del productor debe ser mayor que cero");
            }

            if (favoriteDto.ProductId <= 0)
            {
                _logger.LogWarning("ProductId inválido en favorito");
                throw new ValidationException("ProductId", "El ID del producto debe ser mayor que cero");
            }
        }

        // Eliminación física de un favorito
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un favorito con ID inválido: {FavoriteId}", id);
                throw new ValidationException("id", "El ID del favorito debe ser mayor que cero");
            }

            try
            {
                var favorite = await _favoriteData.GetByIdAsync(id);
                if (favorite == null)
                {
                    _logger.LogInformation("Favorito no encontrado con ID: {FavoriteId}", id);
                    throw new EntityNotFoundException("Favorite", id);
                }

                await _favoriteData.DeletePersistenceAsync(favorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el favorito con ID: {FavoriteId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el favorito con ID {id}", ex);
            }
        }

        // Eliminación lógica de un favorito
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un favorito con ID inválido: {FavoriteId}", id);
                throw new ValidationException("id", "El ID del favorito debe ser mayor que cero");
            }

            try
            {
                var favorite = await _favoriteData.GetByIdAsync(id);
                if (favorite == null)
                {
                    _logger.LogInformation("Favorito no encontrado con ID: {FavoriteId}", id);
                    throw new EntityNotFoundException("Favorite", id);
                }

                await _favoriteData.UpdateAsync(favorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el favorito con ID: {FavoriteId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el favorito con ID {id}", ex);
            }
        }

        // Método para mapear de Favorite a FavoriteDto
        private FavoriteDto MapToDTO(Favorite favorite)
        {
            return new FavoriteDto
            {
                Id = favorite.Id,
                ConsumerId = favorite.ConsumerId,
                ProducerId = favorite.ProducerId,
                ProductId = favorite.ProductId
            };
        }

        // Método para mapear de FavoriteDto a Favorite
        private Favorite MapToEntity(FavoriteDto dto)
        {
            return new Favorite
            {
                Id = dto.Id,
                ConsumerId = dto.ConsumerId,
                ProducerId = dto.ProducerId,
                ProductId = dto.ProductId
            };
        }

        // Método para mapear una lista de Favorite a una lista de FavoriteDto
        private IEnumerable<FavoriteDto> MapToList(IEnumerable<Favorite> favorites)
        {
            var favoritesDTO = new List<FavoriteDto>();
            foreach (var favorite in favorites)
            {
                favoritesDTO.Add(MapToDTO(favorite));
            }
            return favoritesDTO;
        }
    }
}
