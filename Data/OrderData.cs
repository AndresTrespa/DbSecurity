using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class OrderData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderData> _logger;

        public OrderData(ApplicationDbContext context, ILogger<OrderData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para traer todos los pedidos (lógico activo)
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM [Order] WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Order>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pedidos {Order}");
                throw;
            }
        }

        // Método para traer pedido por ID
        public async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM [Order] WHERE Id = @Id AND DeleteAt IS NULL;";
                return await _context.QueryFirstOrDefaultAsync<Order>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pedido con ID {OrderId}", id);
                throw;
            }
        }

        // Método para crear nuevo pedido
        public async Task<Order> CreateAsync(Order order)
        {
            try
            {
                string query = @"
                    INSERT INTO [Order] (ConsumerId, Status, Note, CreateAt, DeleteAt)
                    OUTPUT INSERTED.Id
                    VALUES (@ConsumerId, @Status, @Note, @CreateAt, @DeleteAt);";

                order.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    order.ConsumerId,
                    order.Status,
                    order.Note,
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = (DateTime?)null
                });

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el pedido.");
                throw;
            }
        }

        // Método para actualizar pedido
        public async Task<bool> UpdateAsync(Order order)
        {
            try
            {
                string query = @"UPDATE [Order]
                                 SET ConsumerId = @ConsumerId,
                                     Status = @Status,
                                     Note = @Note
                                 WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new
                {
                    order.ConsumerId,
                    order.Status,
                    order.Note,
                    order.Id
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el pedido: {ex.Message}");
                return false;
            }
        }

        // Método para borrado lógico
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE [Order]
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
                Console.WriteLine($"Error al eliminar lógicamente pedido: {ex.Message}");
                return false;
            }
        }

        // Método para borrar permanentemente por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM [Order] WHERE Id = @Id";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar pedido: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para eliminar permanentemente usando objeto Order
        public async Task<bool> DeletePersistenceAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return await DeletePersistenceAsync(order.Id);
        }
    }
}
