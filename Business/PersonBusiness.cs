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
        public async Task<IEnumerable<PersonDTO>> GetPersonaAsync()
        {
            try
            {
                var personas = await _personaData.GetAllPersonasAsync();
                var PersonDTO = new List<PersonDTO>();

                foreach (var persona in personas)
                    PersonDTO.Add(new PersonDTO
                    {
                        Id = persona.Id,
                        Name = persona.Name,
                        Email = persona.Email,
                        PhoneNumber = persona.PhoneNumber
                    });
                return PersonDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las persona");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de persona", ex);
            }
        }

        public async Task<PersonDTO> GetAllPersonasAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un persona con ID inválido: {personaId}", id);
                throw new Utilities.Exeptions.ValidationException("id", "El ID del persona debe ser mayor que cero");
            }

            try
            {
                var persona = await _personaData.GetPersonaAsync(id);
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
        public async Task<PersonDTO> CreatePersonAsync(PersonDTO PersonDTO)
        {
            try
            {
                ValidatePersona(PersonDTO);

                var persona = new Persona 
                {
                    Name = PersonDTO.Name,
                    Email = PersonDTO.Email, // Si existe en la entidad
                    PhoneNumber = PersonDTO.PhoneNumber
                };

                var personaCreado = await _personaData.CreatePersonAsync(persona);

                return new PersonDTO
                {
                    Id = personaCreado.Id,
                    Name = personaCreado.Name,
                    Email = personaCreado.Email, // Si existe en la entidad
                    PhoneNumber = personaCreado.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Persona: {PersonaNombre}", PersonDTO?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Persona", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePersona(PersonDTO PersonaDto)
        {
            if (PersonaDto == null)
            {
                throw new Utilities.Exeptions.ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PersonaDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Persona con Name vacío");
                throw new Utilities.Exeptions.ValidationException("Name", "El Name de persona es obligatorio");//trae el newspapers de "BussinesException"
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
