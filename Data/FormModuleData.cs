using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para obtener todos los registros de FormModule (sin eliminar lógicamente)
        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM FormModule WHERE DeleteAt = 0;";
                return await _context.QueryAsync<FormModule>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de FormModule");
                throw;
            }
        }

        // Método para obtener un FormModule por ID
        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM FormModule WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<FormModule>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener FormModule con ID {FormModuleId}", id);
                throw;
            }
        }

        // Método para crear un FormModule
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                string query = @"
                    INSERT INTO FormModule (ModuleId, FormId, CreateAt, DeleteAt, Active)
                    OUTPUT INSERTED.Id
                    VALUES (@ModuleId, @FormId, @CreateAt, @DeleteAt, @Active);";

                formModule.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    formModule.ModuleId,
                    formModule.FormId,
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = 0,
                    Active = true
                });

                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear FormModule");
                throw;
            }
        }

        // Método para actualizar FormModule
        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            try
            {
                string query = @"
                    UPDATE FormModule
                    SET ModuleId = @ModuleId, FormId = @FormId
                    WHERE Id = @Id AND DeleteAt = 0;";

                var parameters = new { formModule.ModuleId, formModule.FormId, formModule.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar FormModule: {ex.Message}");
                return false;
            }
        }

        // Método para eliminación lógica de FormModule
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE FormModule SET DeleteAt = 1 WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógicamente FormModule: {ex.Message}");
                return false;
            }
        }

        // Método para eliminación física por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"DELETE FROM FormModule WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar físicamente FormModule: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para eliminar físicamente usando el objeto FormModule
        public async Task<bool> DeletePersistenceAsync(FormModule formModule)
        {
            if (formModule == null)
                throw new ArgumentNullException(nameof(formModule));

            return await DeletePersistenceAsync(formModule.Id);
        }
    }
}
