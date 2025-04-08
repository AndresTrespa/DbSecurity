using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Entity.DTOs;

namespace Data
{
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolUserData> _logger;

        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
        {
            _context = context;
            _logger = logger;
        }
        // Método para traer todo SQL
        public async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM RolUser WHERE DeleteAt = 0;";
                return await _context.QueryAsync<RolUser>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los RolUser");
                throw;
            }
        }

        // Método para traer por id del SQL

        public async Task<RolUser> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM RolUser WHERE Id = @Id";
                return await _context.QueryFirstOrDefaultAsync<RolUser>(query, new { Id = id });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolUser con ID: {id}");
                throw ;
            }
        }
        // Método para crear SQL
        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {
               string query = @"
                    INSERT INTO RolUser (UserId, RolId)
                    OUTPUT INSERTED.Id
                    VALUES (@UserId, @RolId);";
                rolUser.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    UserId = rolUser.UserId,
                    RolId = rolUser.RolId
                });
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol.");
                throw;
            }
        }
        // Método para actualizar Rol
        public async Task<bool> UpdateAsync(RolUser rolUser)
        {
            try
            {
                string query = @"
            UPDATE RolUser
            SET RolId = @RolId, UserId = @UserId
            WHERE Id = @Id AND DeleteAt = 0;";

                var parameters = new
                {
                    rolUser.RolId,
                    rolUser.UserId,
                    rolUser.Id
                };
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolUser con ID {Id}", rolUser.Id);
                return false;
            }
        }
        // Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE RolUser 
                                DeleteAt = 1
                                WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el borrado lógico de RolUser con ID {Id}", id);
                return false;
            }
        }
        // Método para borrar persistentemente SQL usando ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                const string query = "DELETE FROM RolUser WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar físicamente el RolUser con ID: {RolId}", id);
                return false;
            }
        }
        // Sobrecarga para borrar persistentemente usando el objeto Rol

        public async Task<bool> DeletePersistenceAsync(RolUser RolUser)
        {
            if (RolUser == null)
                throw new ArgumentNullException(nameof(RolUser));

            return await DeletePersistenceAsync(RolUser.Id);
        }
    }
}
