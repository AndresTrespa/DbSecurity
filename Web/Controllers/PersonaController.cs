using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exeptions;

namespace Web.Controllers
{
    

        [Route("api/[controller]")]
        [ApiController]
        [Produces("application/json")]
        public class PersonaController : ControllerBase
        {
            private readonly PersonBusiness _PersonBusiness;
            private readonly ILogger<PersonaController> _logger;

            public PersonaController(PersonBusiness PersonBusiness, ILogger<PersonaController> logger)
            {
                _PersonBusiness = PersonBusiness;
                _logger = logger;
            }

            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<PersonDTO>), 200)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetAllPersonasAsync()
            {
                try
                {
                    var Personas = await _PersonBusiness.GetPersonaAsync();
                    return Ok(Personas);
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permisos");
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpGet("{id}")]
            [ProducesResponseType(typeof(PersonDTO), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> GetPersonaAsync(int id)
            {
                try
                {
                    var Persona = await _PersonBusiness.GetAllPersonasAsync(id);
                    return Ok(Persona);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {PersonaId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Permiso no encontrado con ID: {PersonaId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al obtener permiso con ID: {PersonaId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            [HttpPost]
            [ProducesResponseType(typeof(PersonDTO), 200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> CreatePersona([FromBody] PersonDTO PersonaDto)
            {
                try
                {
                    var createdPersona = await _PersonBusiness.CreatePersonAsync(PersonaDto);
                    return CreatedAtAction(nameof(GetPersonaAsync), new { id = createdPersona.Id }, createdPersona);
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
