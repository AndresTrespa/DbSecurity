using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class CategoryData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryData> _logger;

        public CategoryData(ApplicationDbContext context, ILogger<CategoryData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtener todas las categorías activas (DeleteAt = NULL)
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Category WHERE DeleteAt IS NULL;";
                return await _context.QueryAsync<Category>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías.");
                throw;
            }
        }

        // Obtener una categoría por ID
        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Category WHERE Id = @Id AND DeleteAt IS NULL;";
                return await _context.QueryFirstOrDefaultAsync<Category>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID {CategoryId}", id);
                throw;
            }
        }

        // Crear una nueva categoría
        public async Task<Category> CreateAsync(Category category)
        {
            try
            {
                string query = @"
                    INSERT INTO Category (Name, CreateAt, DeleteAt) 
                    OUTPUT INSERTED.Id 
                    VALUES (@Name, @CreateAt, @DeleteAt);";

                category.CreateAt = DateTime.UtcNow;
                category.DeleteAt = null;

                category.Id = await _context.QueryFirstOrDefaultAsync<int>(query, category);

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría.");
                throw;
            }
        }

        // Actualizar una categoría
        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                string query = @"UPDATE Category
                                 SET Name = @Name
                                 WHERE Id = @Id AND DeleteAt IS NULL;";

                var parameters = new { category.Name, category.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la categoría: {ex.Message}");
                return false;
            }
        }

        // Borrado lógico
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Category
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
                Console.WriteLine($"Error al eliminar lógicamente la categoría: {ex.Message}");
                return false;
            }
        }

        // Borrado físico por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Category WHERE Id = @Id";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la categoría: {ex.Message}");
                return false;
            }
        }

        // Borrado físico por entidad
        public async Task<bool> DeletePersistenceAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return await DeletePersistenceAsync(category.Id);
        }
    }
}
