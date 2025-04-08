using Dapper;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;


        ///<param name="conext">Instancia de <see cref="ApplicationDbContext"></param> para la conexion con la base de datos
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Método Para traer todo SQL

        public async Task<IEnumerable<Module>> GetAllModulesAsync()

        {
            try
            {
                string query = "SELECT * FROM Module;";

                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryAsync<Module>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las Module");
                throw;
            }
        }
        //Método para Obtener por Id Del SQL

        public async Task<Module> GetByIdAsync(int id)
        {
            try
            {
                const string query = "SELECT * FROM Module WHERE Id = @Id;";
                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Module>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener permiso: {ex.Message}");
                throw;
            }
        }
        ///<param name="module"> Insancia de Module a crear</param>>  


        public async Task<Module> CreateAsync(Module Module)
        {
            try
            {
                const string query = @"
                INSERT INTO Module (Name, Description)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Description);";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                Module.Id = await connection.QuerySingleAsync<int>(query, new
                {
                    Module.Name,
                    Module.Description
                });

                return Module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear Module: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Module Module)
        {
            try
            {
                string query = @"
                UPDATE Module 
                SET Name = @Name, Description = @Description
                WHERE Id = @Id;";

                var parameters = new { Name = Module.Name, Description = Module.Description, Id = Module.Id };

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

        ///<param name="id">Identificador unico del rol a eliminar.</param>>

        //Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = "UPDATE Module SET Deleted = 1 WHERE Id = @Id;";
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógico el Module: {ex.Message}");
                return false;
            }
        }
        //Método para borrar persistentes SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Module WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar Module: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeletePersistenceAsync(Module Module)
        {
            if (Module == null)
                throw new ArgumentNullException(nameof(Module));

            return await DeletePersistenceAsync(Module.Id);
        }
    }
}