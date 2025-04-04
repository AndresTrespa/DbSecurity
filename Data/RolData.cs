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

        public RolData (ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Metodo para traer todo SQL
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {

            try
            {
                string query = @"SELECT * FROM RolSet WHERE DeleteAt = 0;";
                return await _context.QueryAsync<Rol>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los roles {Rol}");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }


        }


        //Metodo para traer por id Del SQL
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM RolSet WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<Rol>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID {RolId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        ///<param name="rol"> Insancia del rol a crear</param>>

        //método para crear SQL
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                string query = @"
               INSERT INTO RolSet (Name, code, DeleteAt, CreateAt, Active) 
               OUTPUT INSERTED.Id 
               VALUES (@Name, @code, @DeleteAt, @CreateAt,@Active);";

                rol.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    rol.Name,
                    code = rol.code ?? "DEFAULT_CODE", // Asegurar que no sea null
                    CreateAt = DateTime.UtcNow,
                    DeleteAt = DateTime.UtcNow, // o DateTime.MinValue si lo usas como bandera
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

        //Método para actualizar Rol
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                string query = @"UPDATE RolSet
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


        ///<param name="id">Identificador unico del rol a eliminar.</param>>

        ///Método para borrar lógico SQL Data///
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE RolSet
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

        ///Método para borrar persistentes SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM RolSet WHERE Id = @Id";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync(); // Asegurar que la conexión esté abierta

                int rowsAffected = await connection.ExecuteAsync(query, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rol: {ex.Message}");
                return false;
            }
        }
    }
}