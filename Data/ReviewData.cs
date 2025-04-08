using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class ReviewData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewData> _logger;

        public ReviewData(ApplicationDbContext context, ILogger<ReviewData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtener todas las reseñas activas
        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Review WHERE DeleteAt = 0;";
                return await _context.QueryAsync<Review>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las reseñas {Review}");
                throw;
            }
        }

        // Obtener reseña por ConsumerId y ProductId
        public async Task<Review?> GetByIdAsync(int consumerId, int productId)
        {
            try
            {
                string query = @"SELECT * FROM Review 
                                 WHERE ConsumerId = @ConsumerId 
                                 AND ProductId = @ProductId 
                                 AND DeleteAt = 0;";

                return await _context.QueryFirstOrDefaultAsync<Review>(query, new
                {
                    ConsumerId = consumerId,
                    ProductId = productId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la reseña con ConsumerId {ConsumerId} y ProductId {ProductId}", consumerId, productId);
                throw;
            }
        }

        // Crear una reseña
        public async Task<Review> CreateAsync(Review review)
        {
            try
            {
                string query = @"
                    INSERT INTO Review (ConsumerId, ProductId, Rating, Comment, DeleteAt)
                    VALUES (@ConsumerId, @ProductId, @Rating, @Comment, 0);";

                await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    review.ConsumerId,
                    review.ProductId,
                    review.Rating,
                    review.Comment
                });

                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la reseña.");
                throw;
            }
        }

        // Actualizar reseña existente
        public async Task<bool> UpdateAsync(Review review)
        {
            try
            {
                string query = @"
                    UPDATE Review
                    SET Rating = @Rating, Comment = @Comment
                    WHERE ConsumerId = @ConsumerId AND ProductId = @ProductId AND DeleteAt = 0;";

                var parameters = new
                {
                   Rating = review.Rating,
                   Comment= review.Comment,
                   ConsumerId = review.ConsumerId,
                   ProductId = review.ProductId
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la reseña: {ex.Message}");
                return false;
            }
        }

        // Borrado lógico
        public async Task<bool> DeleteLogicAsync(int consumerId, int productId)
        {
            try
            {
                string query = @"
                    UPDATE Review
                    SET DeleteAt = 1
                    WHERE ConsumerId = @ConsumerId AND ProductId = @ProductId;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new
                {
                    ConsumerId = consumerId,
                    ProductId = productId
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógicamente la reseña: {ex.Message}");
                return false;
            }
        }

        // Borrado persistente por claves
        public async Task<bool> DeletePersistenceAsync(int consumerId, int productId)
        {
            try
            {
                string query = @"
                    DELETE FROM Review
                    WHERE ConsumerId = @ConsumerId AND ProductId = @ProductId;";

                var parameters = new { ConsumerId = consumerId, ProductId = productId };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la reseña: {ex.Message}");
                return false;
            }
        }

        // Borrado persistente usando objeto
        public async Task<bool> DeletePersistenceAsync(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            return await DeletePersistenceAsync(review.ConsumerId, review.ProductId);
        }
    }
}
