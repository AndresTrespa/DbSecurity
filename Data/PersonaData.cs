using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<IEnumerable<Persona>> GetAllPersonasAsync()

        {
            return await _context.Set<Persona>().ToListAsync();
        }
        public async Task<Persona> GetPersonaAsync(int id)
        {
            try
            {
                return await _context.Set<Persona>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Persona con ID {PersonaId}", id);
                throw;//Re-lanza la excepcion para qye sea manejada en capas superiores
            }
        }

        ///<param name="module"> Insancia del Persona a crear</param>>


        public async Task<Persona> CreatePersonAsync(Persona persona)
        {
            try
            {
                object value = await _context.Set<Persona>().AddAsync(persona);
                await _context.SaveChangesAsync();
                return persona;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear Persona: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> UpdateAsync(Persona persona)
        {
            try
            {
                _context.Set<Persona>().Update(persona);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Persona: {ex.Message}");
                return false;
            }
        }

        ///<param name="id">Identificador unico del Persona a eliminar.</param>>


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var persona = await _context.Set<Persona>().FindAsync(id);
                if (persona == null)
                    return false;

                _context.Set<Persona>().Remove(persona);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el persona: {ex.Message}");
                return false;
            }
        }
    }
}
