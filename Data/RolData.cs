using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;

        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para traer todo SQL
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM Rol WHERE DeleteAt = 0;";
                return await _context.QueryAsync<Rol>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles {Rol}");
                throw;
            }
        }

        // Método para traer por id del SQL
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM Rol WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<Rol>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID {RolId}", id);
                throw;
            }
        }

        // Método para crear SQL
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                string query = @"
                    INSERT INTO Rol (Name, code, DeleteAt, CreateAt, Active) 
                    OUTPUT INSERTED.Id 
                    VALUES (@Name, @code, @DeleteAt, @CreateAt, @Active);";

                rol.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    rol.Name,
                    code = rol.code ?? "DEFAULT_CODE",
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = DateTime.UtcNow,
                    Active = true
                });

                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rol.");
                throw;
            }
        }

        // Método para actualizar Rol
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                string query = @"UPDATE Rol
                                 SET Name = @Name, code = @code
                                 WHERE Id = @Id AND DeleteAt = 0;";

                var parameters = new { Name = rol.Name, code = rol.code, Id = rol.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        // Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Rol
                                 SET DeleteAt = 1
                                 WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógicamente rol: {ex.Message}");
                return false;
            }
        }

        // Método para borrar persistentemente SQL usando ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Rol WHERE Id = @Id";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rol: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para borrar persistentemente usando el objeto Rol
        public async Task<bool> DeletePersistenceAsync(Rol rol)
        {
            if (rol == null)
                throw new ArgumentNullException(nameof(rol));

            return await DeletePersistenceAsync(rol.Id);
        }
    }
}
