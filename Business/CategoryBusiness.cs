using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las categorías del sistema.  
    /// </summary>
    public class CategoryBusiness
    {
        private readonly CategoryData _categoryData;
        private readonly ILogger<CategoryBusiness> _logger;

        public CategoryBusiness(CategoryData categoryData, ILogger<CategoryBusiness> logger)
        {
            _categoryData = categoryData;
            _logger = logger;
        }

        // Método para obtener todas las categorías como DTOs
        public async Task<IEnumerable<CategoryDto>> GetAllCategoryAsync()
        {
            try
            {
                var categories = await _categoryData.GetAllAsync();
                var categoriesDTO = MapToList(categories);
                return categoriesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las categorías");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de categorías", ex);
            }
        }

        // Método para obtener una categoría por ID como DTO
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una categoría con ID inválido: {CategoryId}", id);
                throw new ValidationException("id", "El ID de la categoría debe ser mayor que cero");
            }

            try
            {
                var category = await _categoryData.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogInformation("No se encontró ninguna categoría con ID: {CategoryId}", id);
                    throw new EntityNotFoundException("Category", id);
                }

                return MapToDTO(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID: {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la categoría con ID {id}", ex);
            }
        }

        // Método para crear una nueva categoría
        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                ValidateCategory(categoryDto);

                var category = MapToEntity(categoryDto);
                var created = await _categoryData.CreateAsync(category);

                return MapToDTO(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva categoría: {CategoryName}", categoryDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la categoría", ex);
            }
        }

        // Método para actualizar una categoría
        public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                ValidateCategory(categoryDto);

                var existingCategory = await _categoryData.GetByIdAsync(categoryDto.Id);
                if (existingCategory == null)
                {
                    _logger.LogInformation("Categoría no encontrada con ID: {CategoryId}", categoryDto.Id);
                    throw new EntityNotFoundException("Category", categoryDto.Id);
                }

                existingCategory.Name = categoryDto.Name;
                await _categoryData.UpdateAsync(existingCategory);

                return MapToDTO(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría con ID: {CategoryId}", categoryDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la categoría con ID {categoryDto.Id}", ex);
            }
        }

        // Eliminación lógica de una categoría
        public async Task DeleteLogicAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente una categoría con ID inválido: {CategoryId}", id);
                throw new ValidationException("id", "El ID de la categoría debe ser mayor que cero");
            }

            try
            {
                var category = await _categoryData.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogInformation("Categoría no encontrada con ID: {CategoryId}", id);
                    throw new EntityNotFoundException("Category", id);
                }

                category.DeleteAt = DateTime.UtcNow;
                await _categoryData.UpdateAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la categoría con ID: {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar lógicamente la categoría con ID {id}", ex);
            }
        }

        // Eliminación física de una categoría
        public async Task DeletePersistenceAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar una categoría con ID inválido: {CategoryId}", id);
                throw new ValidationException("id", "El ID de la categoría debe ser mayor que cero");
            }

            try
            {
                var category = await _categoryData.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogInformation("Categoría no encontrada con ID: {CategoryId}", id);
                    throw new EntityNotFoundException("Category", id);
                }

                await _categoryData.DeletePersistenceAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID: {CategoryId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la categoría con ID {id}", ex);
            }
        }

        // Validación del DTO de categoría
        private void ValidateCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new ValidationException("El objeto categoría no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una categoría con Name vacío");
                throw new ValidationException("Name", "El Name de la categoría es obligatorio");
            }
        }

        // Mapeo de entidad a DTO
        private CategoryDto MapToDTO(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        // Mapeo de DTO a entidad
        private Category MapToEntity(CategoryDto categoryDto)
        {
            return new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name
            };
        }

        // Mapeo de lista de entidades a lista de DTOs
        private IEnumerable<CategoryDto> MapToList(IEnumerable<Category> categories)
        {
            var categoriesDTO = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDTO.Add(MapToDTO(category));
            }
            return categoriesDTO;
        }
    }
}
