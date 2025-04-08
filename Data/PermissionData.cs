using Dapper;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        ///<param name="conext">Instancia de <see cref="ApplicationDbContext"></param> para la conexion con la base de datos
        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Método Para traer todo SQL
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                string query = "SELECT * FROM Permission;";

                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryAsync<Permission>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las Permissions");
                throw;
            } 
        }

        ///<param name="module"> Insancia del Permiso a crear</param>>

        //Método para Obtener por Id Del SQL

        public async Task<Permission> GetByIdAsync(int id)
        {
            try
            {
                const string query = "SELECT * FROM Permission WHERE Id = @Id;";
                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Permission>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener permiso: {ex.Message}");
                throw;
            }
        }
        //Método para Crear SQL

        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                const string query = @"
                INSERT INTO Permission (Name, Description)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Description);";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    permission.Name,
                    permission.Description
                });

                permission.Id = id;
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear Permission: {ex.Message}");
                throw;
            }
        }
        //Método para Actualizar Permiso
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try { 
                string query = @"
                UPDATE Permission 
                SET Name = @Name, Description = @Description
                WHERE Id = @Id;";

                var parameters = new { Name = permission.Name, Description = permission.Description, Id = permission.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la permiso: {ex.Message}");
                return false;
            }
        }
        ///<param name="id">Identificador unico del Permiso a eliminar.</param>>

        //Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = "UPDATE Permission SET Deleted = 1 WHERE Id = @Id;";
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógico el permiso: {ex.Message}");
                return false;
            }
        }
        //Método para borrar persistentes SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Permission WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar permiso: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeletePersistenceAsync(Permission Permiso)
        {
            if (Permiso == null)
                throw new ArgumentNullException(nameof(Permiso));

            return await DeletePersistenceAsync(Permiso.Id);
        }
    }
}

