using Entity.Context;
using Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Configuration;
namespace Data
{
    public class UserData
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;
        private readonly string ConnectionString;


        ///<param name="conext">Instancia de <see cref="ApplicationDbContext"></param> para la conexion con la base de datos
        public UserData(ApplicationDbContext context, ILogger<UserData> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public async Task<IEnumerable<User>> GetRolsAsync()

        {
            return await _context.Set<User>().ToListAsync();
        }
        public async Task<User> GetRolAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID {RoId}", id);
                throw;//Re-lanza la excepcion para qye sea manejada en capas superiores
            }
        }

        ///<param name="rol"> Insancia del rol a crear</param>>


        public async Task<User> CreateAsync(User user)
        {
            try
            {
                object value = await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear rol: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> ActualizarUser(User User)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Users SET UserName = @UserName, ProfilePhotoUrl = @ProfilePhotoUrl WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Id", User.Id);
                command.Parameters.AddWithValue("@UserName", User.UserName);
                command.Parameters.AddWithValue("@ProfilePhotoUrl", User.ProfilePhotoUrl);

                connection.Open();

                int filasAfectadas = command.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        ///<param name="id">Identificador unico del rol a eliminar.</param>>


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol: {ex.Message}");
                return false;
            }
        }
    }
}

