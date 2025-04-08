using Entity.Context;
using Microsoft.Extensions.Logging;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Data
{
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método para traer todos los usuarios
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                string query = @"SELECT * FROM [User];";
                return await _context.QueryAsync<User>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios.");
                throw;
            }
        }

        // Método para obtener un usuario por ID
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT * FROM [User] WHERE Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId}", id);
                throw;
            }
        }

        // Método para crear un nuevo usuario
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                string query = @"
                    INSERT INTO [User] (UserName, ProfilePhotoUrl, Active) 
                    OUTPUT INSERTED.Id 
                    VALUES (@UserName, @ProfilePhotoUrl, @Active);";

                user.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    user.UserName,
                    user.ProfilePhotoUrl,
                    user.Active
                });

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw;
            }
        }

        // Método para actualizar un usuario
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                string query = @"UPDATE [User]
                                 SET UserName = @UserName,
                                     ProfilePhotoUrl = @ProfilePhotoUrl,
                                     Active = @Active
                                 WHERE Id = @Id;";

                var parameters = new
                {
                    user.UserName,
                    user.ProfilePhotoUrl,
                    user.Active,
                    user.Id
                };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario: {ex.Message}");
                return false;
            }
        }

        // Método para borrar persistentemente por ID
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = "DELETE FROM [User] WHERE Id = @Id;";
                var parameters = new { Id = id };

                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                int rowsAffected = await connection.ExecuteAsync(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
                return false;
            }
        }

        // Sobrecarga para borrar usando objeto User
        public async Task<bool> DeletePersistenceAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await DeletePersistenceAsync(user.Id);
        }
    }
}
