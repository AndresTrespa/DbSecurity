using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class FavoriteData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FavoriteData> _logger;

        public FavoriteData(ApplicationDbContext context, ILogger<FavoriteData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para obtener todos los favoritos (no eliminados)
        public async Task<IEnumerable<Favorite>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Favorite WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Favorite>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los favoritos {Favorite}");
                throw;
            }
        }

        // Método para obtener un favorito por ID
        public async Task<Favorite?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Favorite WHERE Id = @Id AND DeleteAt IS NULL;";
                return await _context.QueryFirstOrDefaultAsync<Favorite>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el favorito con ID {FavoriteId}", id);
                throw;
            }
        }

        // Método para crear un nuevo favorito
        public async Task<Favorite> CreateAsync(Favorite favorite)
        {
            try
            {
                string query = @"
                    INSERT INTO Favorite (ConsumerId, ProducerId, ProductId, Date_Added, CreateAt)
                    OUTPUT INSERTED.Id
                    VALUES (@ConsumerId, @ProducerId, @ProductId, @Date_Added, @CreateAt);";

                favorite.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    favorite.ConsumerId,
                    favorite.ProducerId,
                    favorite.ProductId,
                    Date_Added = DateTime.UtcNow,
                    CreateAt = DateTime.UtcNow
                });

                return favorite;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el favorito.");
                throw;
            }
        }

        // Método para actualizar un favorito existente
        public async Task<bool> UpdateAsync(Favorite favorite)
        {
            try
            {
                string query = @"
                    UPDATE Favorite
                    SET ConsumerId = @ConsumerId,
                        ProducerId = @ProducerId,
                        ProductId = @ProductId,
                        Date_Added = @Date_Added,
                        CreateAt = @CreateAt
                    WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new
                {
                    favorite.ConsumerId,
                    favorite.ProducerId,
                    favorite.ProductId,
                    favorite.Date_Added,
                    favorite.CreateAt,
                    favorite.Id
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el favorito: {ex.Message}");
                return false;
            }
        }

        // Método para eliminación lógica
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Favorite SET DeleteAt = @DeleteAt WHERE Id = @Id AND DeleteAt IS NULL;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    DeleteAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógicamente el favorito: {ex.Message}");
                return false;
            }
        }

        // Eliminación persistente por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Favorite WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar favorito: {ex.Message}");
                return false;
            }
        }

        // Eliminación persistente por objeto Favorite
        public async Task<bool> DeletePersistenceAsync(Favorite favorite)
        {
            if (favorite == null)
                throw new ArgumentNullException(nameof(favorite));

            return await DeletePersistenceAsync(favorite.Id);
        }
    }
}
