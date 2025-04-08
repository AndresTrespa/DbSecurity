using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las reseñas del sistema.
    /// </summary>
    public class ReviewBusiness
    {
        private readonly ReviewData _reviewData;
        private readonly ILogger<ReviewBusiness> _logger;

        public ReviewBusiness(ReviewData reviewData, ILogger<ReviewBusiness> logger)
        {
            _reviewData = reviewData;
            _logger = logger;
        }

        // Método para obtener todas las reseñas como DTOs
        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _reviewData.GetAllAsync();
                var reviewsDTO = MapToList(reviews);
                return reviewsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las reseñas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de reseñas", ex);
            }
        }

        // Método para obtener una reseña por claves compuestas (ConsumerId, ProductId)
        public async Task<ReviewDto> GetReviewByIdAsync(int consumerId, int productId)
        {
            if (consumerId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Se intentó obtener una reseña con claves inválidas: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ValidationException("consumerId/productId", "Los IDs deben ser mayores a cero");
            }

            try
            {
                var review = await _reviewData.GetByIdAsync(consumerId, productId);
                if (review == null)
                {
                    _logger.LogInformation("No se encontró ninguna reseña con claves: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                    throw new EntityNotFoundException("Review", $"ConsumerId={consumerId}, ProductId={productId}");
                }

                return MapToDTO(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la reseña: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ExternalServiceException("Base de datos", "Error al recuperar la reseña", ex);
            }
        }

        // Método para crear una reseña desde un DTO
        public async Task<ReviewDto> CreateReviewAsync(int consumerId, int productId, ReviewDto reviewDto)
        {
            try
            {
                ValidateReview(reviewDto);

                var review = new Review
                {
                    ConsumerId = consumerId,
                    ProductId = productId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    DeleteAt = false
                };

                var created = await _reviewData.CreateAsync(review);
                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reseña: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ExternalServiceException("Base de datos", "Error al crear la reseña", ex);
            }
        }

        // Método para actualizar una reseña existente
        public async Task<bool> UpdateReviewAsync(int consumerId, int productId, ReviewDto dto)
        {
            try
            {
                ValidateReview(dto);

                var existing = await _reviewData.GetByIdAsync(consumerId, productId);
                if (existing == null)
                {
                    _logger.LogInformation("No se encontró la reseña a actualizar: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                    throw new EntityNotFoundException("Review", $"ConsumerId={consumerId}, ProductId={productId}");
                }

                existing.Rating = dto.Rating;
                existing.Comment = dto.Comment;

                return await _reviewData.UpdateAsync(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la reseña: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la reseña", ex);
            }
        }

        // Eliminación lógica
        public async Task DeleteLogicAsync(int consumerId, int productId)
        {
            if (consumerId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente una reseña con claves inválidas");
                throw new ValidationException("consumerId/productId", "Los IDs deben ser mayores que cero");
            }

            try
            {
                var review = await _reviewData.GetByIdAsync(consumerId, productId);
                if (review == null)
                {
                    _logger.LogInformation("No se encontró la reseña con claves: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                    throw new EntityNotFoundException("Review", $"ConsumerId={consumerId}, ProductId={productId}");
                }

                await _reviewData.DeleteLogicAsync(consumerId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la reseña: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ExternalServiceException("Base de datos", "Error al eliminar lógicamente la reseña", ex);
            }
        }

        // Eliminación física
        public async Task DeletePersistenceAsync(int consumerId, int productId)
        {
            if (consumerId <= 0 || productId <= 0)
            {
                _logger.LogWarning("Se intentó eliminar físicamente una reseña con claves inválidas");
                throw new ValidationException("consumerId/productId", "Los IDs deben ser mayores que cero");
            }

            try
            {
                var review = await _reviewData.GetByIdAsync(consumerId, productId);
                if (review == null)
                {
                    _logger.LogInformation("No se encontró la reseña con claves: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                    throw new EntityNotFoundException("Review", $"ConsumerId={consumerId}, ProductId={productId}");
                }

                await _reviewData.DeletePersistenceAsync(consumerId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente la reseña: ConsumerId={ConsumerId}, ProductId={ProductId}", consumerId, productId);
                throw new ExternalServiceException("Base de datos", "Error al eliminar físicamente la reseña", ex);
            }
        }

        // Validación del DTO
        private void ValidateReview(ReviewDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto reseña no puede ser nulo");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                _logger.LogWarning("Se intentó registrar una reseña con calificación inválida: {Rating}", dto.Rating);
                throw new ValidationException("Rating", "La calificación debe estar entre 1 y 5");
            }

            if (string.IsNullOrWhiteSpace(dto.Comment))
            {
                _logger.LogWarning("Se intentó registrar una reseña sin comentario");
                throw new ValidationException("Comment", "El comentario es obligatorio");
            }
        }

        // Mapeo de entidad a DTO
        private ReviewDto MapToDTO(Review review)
        {
            return new ReviewDto
            {
                Date = DateTime.UtcNow, // O usar review.Date si lo tienes en el modelo
                Rating = review.Rating,
                Comment = review.Comment,
                DeleteAt = review.DeleteAt
            };
        }

        // Mapeo de DTO a entidad
        private Review MapToEntity(ReviewDto reviewDto)
        {
            return new Review
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                DeleteAt = reviewDto.DeleteAt
            };
        }

        // Mapeo de Review de entidades a lista de DTOs
        private IEnumerable<ReviewDto> MapToList(IEnumerable<Review> reviews)
        {
            var reviewDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewDto.Add(MapToDTO(review));
            }
            return reviewDto;
        }
    }
}
