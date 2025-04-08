using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class PersonBusiness
    {
        private readonly PersonaData _personaData;
        private readonly ILogger<PersonBusiness> _logger;
        public PersonBusiness(PersonaData personaData, ILogger<PersonBusiness> logger)
        {
            _personaData = personaData;
            _logger = logger;
        }
        public async Task<IEnumerable<PersonDTO>> GetAllPersonasAsync()
        {
            try
            {
                var personas = await _personaData.GetAllPersonasAsync();
                var PersonDTO = MapToList(personas);
                return PersonDTO;  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las persona");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de persona", ex);
            }
        }

        public async Task<PersonDTO> GetPersonaByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un persona con ID inválido: {PersonaId}", id);
                throw new Utilities.Exeptions.ValidationException("id", "El ID del persona debe ser mayor que cero");
            }
            try
            {
                var persona = await _personaData.GetPersonaIdAsync(id);
                if (persona == null)
                {
                    _logger.LogInformation("No se encontró ningún persona con ID: {PersonaId}", id);
                    throw new EntityNotFoundException("persona", id);
                }

                return new PersonDTO
                {
                    Id = persona.Id,
                    Name = persona.Name,
                    Email = persona.Email,
                    PhoneNumber = persona.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el persona con ID: {personaId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el persona con ID {id}", ex);
            }
        }

        // Método para crear un persona desde un DTO
        public async Task<PersonDTO> CreatePersonAsync(PersonDTO personDTO)
        {
            try
            {
                ValidatePersona(personDTO);

                var persona = new Persona
                {
                    Name = personDTO.Name,
                    Email = personDTO.Email,
                    PhoneNumber = personDTO.PhoneNumber
                };

                var personaCreado = await _personaData.CreateAsync(persona);

                return new PersonDTO
                {
                    Id = personaCreado.Id,
                    Name = personaCreado.Name,
                    Email = personaCreado.Email,
                    PhoneNumber = personaCreado.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Persona: {PersonaNombre}", personDTO?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Persona", ex);
            }
        }



        // Método para validar el DTO
        private void ValidatePersona(PersonDTO personaDto)
        {
            if (personaDto == null)
                throw new ValidationException("El objeto persona no puede ser nulo");

            if (string.IsNullOrWhiteSpace(personaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Persona con Name vacío");
                throw new ValidationException("Name", "El nombre de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personaDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Persona con Email vacío");
                throw new ValidationException("Email", "El email de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personaDto.PhoneNumber))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Persona con número vacío");
                throw new ValidationException("PhoneNumber", "El número de teléfono es obligatorio");
            }
        }
        // Eliminación física de un rol
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _personaData.GetPersonaIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("Rol no encontrado con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                await _personaData.DeletePersistenceAsync(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el rol con ID {id}", ex);
            }
        }

        // Eliminación lógica de un rol
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un rol con ID inválido: {RolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _personaData.GetPersonaIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("Rol no encontrado con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                await _personaData.UpdateAsync(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente el rol con ID {id}", ex);
            }
        }
        //Método para mapear de Persona a PersonaDto
        private PersonDTO MapToDTO(Persona persona)
        {
            return new PersonDTO
            {
                Id = persona.Id,
                Name = persona.Name,
                Email = persona.Email,//si existe la entidad
                PhoneNumber = persona.PhoneNumber
            };
        }
        //Método para mapear de PersonaDto a Persona
        private Persona MapToEntity(PersonDTO PersonaDto)
        {
            return new Persona     
            {
                Id = PersonaDto.Id,
                Name = PersonaDto.Name,
                Email = PersonaDto.Email,
                PhoneNumber = PersonaDto.PhoneNumber
            };
        }
        //Método para mapear una lista de Persona a una lista de PersonaDTO
        private IEnumerable<PersonDTO> MapToList(IEnumerable<Persona> personas)
        {
            var PersonaDto = new List<PersonDTO>();
            foreach (var persona in personas)
            {
                PersonaDto.Add(MapToDTO(persona));
            }
            return PersonaDto;
        }
    }
}
