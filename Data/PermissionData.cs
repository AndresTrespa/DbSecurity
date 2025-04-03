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

            public async Task<IEnumerable<Permission>> GetPermissionAllsAsync()

            {
                return await _context.Set<Permission>().ToListAsync();
            }
            public async Task<Permission> GetbyIdAsync(int id)
            {
                try
                {
                    return await _context.Set<Permission>().FindAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener permission con ID {RoId}", id);
                    throw;//Re-lanza la excepcion para qye sea manejada en capas superiores
                }
            }

            ///<param name="module"> Insancia del permission a crear</param>>


            public async Task<Permission> CreateAsync(Permission permission)
            {
                try
                {
                    object value = await _context.Set<Permission>().AddAsync(permission);
                    await _context.SaveChangesAsync();
                    return permission;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al crear permission: {ex.Message}");
                    throw;
                }
            }


            public async Task<bool> UpdateAsync(Permission permission)
            {
                try
                {
                    _context.Set<Permission>().Update(permission);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al actualizar el permission: {ex.Message}");
                    return false;
                }
            }

            ///<param name="id">Identificador unico del permission a eliminar.</param>>


            public async Task<bool> DeleteAsync(int id)
            {
                try
                {
                    var permission = await _context.Set<Permission>().FindAsync(id);
                    if (permission == null)
                        return false;

                    _context.Set<Permission>().Remove(permission);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar el permission: {ex.Message}");
                    return false;
                }
            }
        }
    }
