using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class ProducerData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProducerData> _logger;

        public ProducerData(ApplicationDbContext context, ILogger<ProducerData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para traer todos los productores (lógico no eliminados)
        public async Task<IEnumerable<Producer>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Producer WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Producer>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productores {Producer}");
                throw;
            }
        }

        // Método para traer productor por ID
        public async Task<Producer?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Producer WHERE Id = @Id AND DeleteAt IS NULL;";
                return await _context.QueryFirstOrDefaultAsync<Producer>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el productor con ID {ProducerId}", id);
                throw;
            }
        }

        // Método para crear productor
        public async Task<Producer> CreateAsync(Producer producer)
        {
            try
            {
                string query = @"
                    INSERT INTO Producer (Address, CreateAt) 
                    OUTPUT INSERTED.Id 
                    VALUES (@Address, @CreateAt);";

                producer.CreateAt = DateTime.UtcNow;

                producer.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    producer.Address,
                    producer.CreateAt
                });

                return producer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el productor.");
                throw;
            }
        }

        // Método para actualizar productor
        public async Task<bool> UpdateAsync(Producer producer)
        {
            try
            {
                string query = @"UPDATE Producer
                                 SET Address = @Address
                                 WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new { producer.Address, producer.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el productor: {ex.Message}");
                return false;
            }
        }

        // Método para borrado lógico
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Producer
                                 SET DeleteAt = @DeleteAt
                                 WHERE Id = @Id AND DeleteAt IS NULL;";

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
                Console.WriteLine($"Error al eliminar lógicamente productor: {ex.Message}");
                return false;
            }
        }

        // Método para borrado físico por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Producer WHERE Id = @Id";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar productor: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para borrado físico usando objeto
        public async Task<bool> DeletePersistenceAsync(Producer producer)
        {
            if (producer == null)
                throw new ArgumentNullException(nameof(producer));

            return await DeletePersistenceAsync(producer.Id);
        }
    }
}
