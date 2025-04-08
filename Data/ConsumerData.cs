using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class ConsumerData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConsumerData> _logger;

        public ConsumerData(ApplicationDbContext context, ILogger<ConsumerData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para obtener todos los consumidores del SQL
        public async Task<IEnumerable<Consumer>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Consumer WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Consumer>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los consumidores {Consumer}");
                throw;
            }
        }

        // Método para traer por id del SQL
        public async Task<Consumer?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Consumer WHERE Id = @Id AND DeleteAt IS NULL;";
                return await _context.QueryFirstOrDefaultAsync<Consumer>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el consumidor con ID {ConsumerId}", id);
                throw;
            }
        }

        // Método para crear SQL
        public async Task<Consumer> CreateAsync(Consumer consumer)
        {
            try
            {
                string query = @"
                    INSERT INTO Consumer (CreateAt, DeleteAt, Active) 
                    OUTPUT INSERTED.Id 
                    VALUES (@CreateAt, @DeleteAt, @Active);";

                consumer.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = (DateTime?)null,
                    Active = true
                });

                return consumer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el consumidor.");
                throw;
            }
        }

        // Método para actualizar Consumer
        public async Task<bool> UpdateAsync(Consumer consumer)
        {
            try
            {
                string query = @"UPDATE Consumer
                                 SET Active = @Active
                                 WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new { consumer.Active, consumer.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el consumidor: {ex.Message}");
                return false;
            }
        }

        // Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Consumer
                                 SET DeleteAt = @DeleteAt
                                 WHERE Id = @Id;";

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
                Console.WriteLine($"Error al eliminar lógicamente consumidor: {ex.Message}");
                return false;
            }
        }

        // Método para borrar persistentemente SQL usando ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Consumer WHERE Id = @Id";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar consumidor: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para borrar persistentemente usando el objeto Consumer
        public async Task<bool> DeletePersistenceAsync(Consumer consumer)
        {
            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));

            return await DeletePersistenceAsync(consumer.Id);
        }
    }
}
