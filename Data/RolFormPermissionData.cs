using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;


namespace Data
{
    public class RolFormPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolFormPermissionData> _logger;

        public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtener todos
        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            try
            {
                string query = "SELECT * FROM RolFormPermission;";
                return await _context.QueryAsync<RolFormPermission>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolFormPermission");
                throw;
            }
        }

        // Obtener por ID
        public async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                string query = "SELECT * FROM RolFormPermission WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<RolFormPermission>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolFormPermission con ID {Id}", id);
                throw;
            }
        }

        // Crear nuevo
        public async Task<RolFormPermission> CreateAsync(RolFormPermission RFP)
        {
            try
            {
                string query = @"
                    INSERT INTO RolFormPermission (RolId, FormId, PermissionId)
                    OUTPUT INSERTED.Id
                    VALUES (@RolId, @FormId, @PermissionId);";

                RFP.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    RFP.RolId,
                    RFP.FormId,
                    RFP.PermissionId
                });

                return RFP;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear RolFormPermission.");
                throw;
            }
        }

        // Actualizar
        public async Task<bool> UpdateAsync(RolFormPermission RFP)
        {
            try
            {
                string query = @"
                    UPDATE RolFormPermission
                    SET RolId = @RolId, FormId = @FormId, PermissionId = @PermissionId
                    WHERE Id = @Id;";

                var parameters = new
                {
                    RFP.RolId,
                    RFP.FormId,
                    RFP.PermissionId,
                    RFP.Id
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar RolFormPermission con ID {Id}", RFP.Id);
                return false;
            }
        }
        // Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE RolFormPermission
                                 SET DeleteAt = 1
                                 WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógicamente RolFormPermission: {ex.Message}");
                return false;
            }
        }
        // Eliminar físicamente por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM RolFormPermission WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar RolFormPermission con ID {Id}", id);
                return false;
            }
        }

        // Sobrecarga para eliminar por objeto
        public async Task<bool> DeleteAsync(RolFormPermission RFP)
        {
            if (RFP == null)
                throw new ArgumentNullException(nameof(RFP));

            return await DeletePersistenceAsync(RFP.Id);
        }
    }
}
