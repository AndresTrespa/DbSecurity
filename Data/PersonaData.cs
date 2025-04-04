using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Data
{
    public class PersonaData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonaData> _logger;


        ///<param name="conext">Instancia de <see cref="ApplicationDbContext"></param> para la conexion con la base de datos
        public PersonaData(ApplicationDbContext context, ILogger<PersonaData> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Método Para traer todo SQL
        public async Task<IEnumerable<Persona>> GetAllPersonasAsync()

        {
            try
            {
                string query = @"SELECT *FROM PersonSet;";
                return await _context.QueryAsync<Persona>(query);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al obtener Los Persona {Persona}");
                throw; 
            }
        }

        ///<param name="module"> Insancia del Persona a crear</param>>

        //Método para Obtener por Id Del SQL
        public async Task<Persona> GetPersonaAsync(int id)
        {
            try
            {
                String query = @"SELECT * FROM PersonSet;";
                return await _context.QueryFirstOrDefaultAsync<Persona>(query, new {Id = id});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Persona: {ex.Message}");
                throw;
            }
        }

        //Método para Crear SQL

        public async Task<Persona> CreateAsync(Persona persona)
        {
            try
            {
                string query = @"INSERT INTO PersonSet(Name, Email, PhoneNumber)
                                OUTPUT INSERTED.Id
                                VALUES(@Name, @Email, @PhoneNumber);";
                persona.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    persona.Name,
                    persona.Email,
                    persona.PhoneNumber
                });
                return persona;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Persona");
                throw;
            }
        }

        //Método para Actualizar Persona

        public async Task<bool> UpdateAsync(Persona persona)
        {
            try
            {
                string query = @"UPDATE PersonSet
                                SET Name = @Name, PhoneNumber = @PhoneNumber
                                WHERE Id = @Id;";
                var parameters = new { Name = persona.Name, PhoneNumber = persona.PhoneNumber, Id = persona.Id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Persona: {ex.Message}");
                return false;
            }
        }

        ///<param name="id">Identificador unico del Persona a eliminar.</param>>

        //Método para borrar lógico SQL Data
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE PersonaSet SET Id = 1 WHERE Id = @Id;";

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar lógico la persona: {ex.Message}");
                return false;
            }
        }
        //Método para borrar persistentes SQL
        public async Task<bool> DeletePersistenteceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM PersonSet WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar persona: {ex.Message}");
                return false;
            }
        }
    }
}
