using Dapper;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;


        ///<param name="conext">Instancia de <see cref="ApplicationDbContext"></param> para la conexion con la base de datos
        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Método Para traer todo SQL
        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            try
            {
                string query = "SELECT * FROM Form;";

                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryAsync<Form>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las Form");
                throw;
            }
        }
        //Método para Obtener por Id Del SQL

        public async Task<Form> GetByIdAsync(int id)
        {
            try
            {
                const string query = "SELECT * FROM Form WHERE Id = @Id;";
                using var connection = _context.Database.GetDbConnection();

                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Form>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener permiso: {ex.Message}");
                throw;
            }
        }

        ///<param name="form"> Insancia del rol a crear</param>>

                //Método para Crear SQL
        public async Task<Form> CreateAsync(Form Form)
        {
            try
            {
                const string query = @"
                INSERT INTO Form (Name, Description)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Description);";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                var id = await connection.QuerySingleAsync<int>(query, new
                {
                    Form.Name,
                    Form.Description
                });

                Form.Id = id;
                return Form;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear Form: {ex.Message}");
                throw;
            }
        }

        //Método para Actualizar
        public async Task<bool> UpdateAsync(Form Form)
        {
            try
            {
                string query = @"
                UPDATE Permission 
                SET Name = @Name, Description = @Description
                WHERE Id = @Id;";

                var parameters = new { Name = Form.Name, Description = Form.Description, Id = Form.Id };

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

        /////Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = "UPDATE Form SET Deleted = 1 WHERE Id = @Id;";
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
                string query = "DELETE FROM Form WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar Form: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeletePersistenceAsync(Form Form)
        {
            if (Form == null)
                throw new ArgumentNullException(nameof(Form));

            return await DeletePersistenceAsync(Form.Id);
        }
    }
}

