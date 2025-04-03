using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }
        public async Task<IEnumerable<FormDto>> GetFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                var formsDTO = new List<FormDto>();

                foreach (var form in forms)
                {
                    formsDTO.Add(new FormDto
                    {
                        Id = form.Id,
                        Name = form.Name,
                        Description = form.Description
                    });
                }
                return formsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un form con ID inválido: {Form}", id);
                throw new Utilities.Exeptions.ValidationException("id", "El ID del form debe ser mayor que cero");
            }
            try
            {
                var form = await _formData.GetbyIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("Se intentó obtener un rol con ID inválido: {Form}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return new FormDto
                {
                    Id = form.Id,
                    Name = form.Name,
                    Description = form.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el form con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el form con ID {id}", ex);
            }
        }
        public async Task<FormDto> CreateFormAsync(FormDto FormDto)
        {
           try
           {
                ValidateForm(FormDto);
                var form = new Form
                {
                    Name = FormDto.Name,
                    Description = FormDto.Description
                };
                var formCreado = await _formData.CreateAsync(form);

                return new FormDto
                {
                    Id = formCreado.Id,
                    Description = formCreado.Description
                };
           }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear form: {FormName}", FormDto?.Name ?? "Null");
                throw new ExternalServiceException("Base de datos", "Error al crear el form", ex);
            }
        }
        private void ValidateForm(FormDto FormDto)
        {
            if (FormDto == null)
            {
                throw new Utilities.Exeptions.ValidationException("El objeto form no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(FormDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un form con Name vacío");
                throw new Utilities.Exeptions.ValidationException("Name del form es obligatorio");
            }
        }
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description
            };
        }
        private IEnumerable<FormDto> MapToList(IEnumerable<Form> forms)
        {
            var formsDTO = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDTO.Add(MapToDTO(form));
            }
            return formsDTO;
        }
    }
}
