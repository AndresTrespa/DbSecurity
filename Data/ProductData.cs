using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class ProductData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductData> _logger;

        public ProductData(ApplicationDbContext context, ILogger<ProductData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para traer todos los productos (SQL)
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Product WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Product>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos {Product}");
                throw;
            }
        }

        // Método para traer un producto por ID (SQL)
        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Product WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductId}", id);
                throw;
            }
        }

        // Método para crear un producto (SQL)
        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                string query = @"
                    INSERT INTO Product (CategoryId, FavoriteId, CreateAt, DeleteAt) 
                    OUTPUT INSERTED.Id 
                    VALUES (@CategoryId, @FavoriteId, @CreateAt, @DeleteAt);";

                product.CreateAt = DateTime.UtcNow;
                product.DeleteAt = null;

                product.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    product.CategoryId,
                    product.FavoriteId,
                    product.CreateAt,
                    product.DeleteAt
                });

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto.");
                throw;
            }
        }

        // Método para actualizar un producto (SQL)
        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                string query = @"
                    UPDATE Product
                    SET CategoryId = @CategoryId, FavoriteId = @FavoriteId
                    WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new
                {
                    product.CategoryId,
                    product.FavoriteId,
                    product.Id
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el producto: {ex.Message}");
                return false;
            }
        }

        // Método para borrado lógico de un producto (SQL)
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Product
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
                Console.WriteLine($"Error al eliminar lógicamente el producto: {ex.Message}");
                return false;
            }
        }

        // Método para eliminar un producto permanentemente por ID (SQL)
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Product WHERE Id = @Id";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para eliminar permanentemente por objeto Product
        public async Task<bool> DeletePersistenceAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return await DeletePersistenceAsync(product.Id);
        }
    }
}
