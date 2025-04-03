using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    public class PermisosController
    {
        [Route("api/[controller]")]
        [ApiController]
        [Produces("application/json")]
        public class PermisoController : ControllerBase
        {
            private readonly permisoBusiness _permisoBusiness;
            private readonly ILogger<PermisoController> _logger;

            public PermisoController(permisoBusiness permisoBusiness, ILogger<PermisoController> logger)
            {
                _permisoBusiness = permisoBusiness;
                _logger = logger;
            }

            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetPermissionAllsAsync()
            {
                try
                {
                    var Permission = await _permisoBusiness.GetpermisosAsync();
                    return Ok(Permission);
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permisos");
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpGet("{id}")]
            [ProducesResponseType(typeof(PermissionDto), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetPermissionById(int id)
            {
                try
                {
                    var permission = await _permisoBusiness.GetpermissionByIdAsync(id);
                    return Ok(permission);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {PermissionId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permiso con ID: {PermissionId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpPost]
            [ProducesResponseType(typeof(PermissionDto), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> createdPermission([FromBody] PermissionDto PermissionDto)
            {
                try
                {
                    var createdPermission = await _permisoBusiness.CreatePermissionAsync(PermissionDto);
                    return CreatedAtAction(nameof(GetPermissionById), new { id = createdPermission.Id }, createdPermission);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al crear permiso");
                    return BadRequest(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al crear permiso");
                    return StatusCode(500, new { message = ex.Message });
                }
            }

        }
    }
}
