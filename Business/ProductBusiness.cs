using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exeptions;

namespace Business
{
    public class ProductBusiness
    {
        private readonly ProductData _productData;
        private readonly ILogger<ProductBusiness> _logger;

        public ProductBusiness(ProductData productData, ILogger<ProductBusiness> logger)
        {
            _productData = productData;
            _logger = logger;
        }

        // Método para obtener todos los productos como DTOs
        public async Task<IEnumerable<ProductDto>> GetProductAllAsync()
        {
            try
            {
                var products = await _productData.GetAllAsync();
                var productsDTO = MapToList(products);
                return productsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de productos", ex);
            }
        }
        // Obtener un producto por su ID
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productData.GetByIdAsync(id);
            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                FavoriteId = product.FavoriteId
            };
        }

        // Crear un nuevo producto
        public async Task<ProductDto> CreateAsync(ProductDto productDto)
        {
            // Validación básica
            if (productDto.CategoryId <= 0 || productDto.FavoriteId <= 0)
                throw new ArgumentException("Los campos CategoryId y FavoriteId deben ser mayores que cero.");

            var product = new Product
            {
                CategoryId = productDto.CategoryId,
                FavoriteId = productDto.FavoriteId
            };

            var created = await _productData.CreateAsync(product);

            return new ProductDto
            {
                Id = created.Id,
                CategoryId = created.CategoryId,
                FavoriteId = created.FavoriteId
            };
        }

        // Actualizar un producto existente
        public async Task<bool> UpdateAsync(ProductDto productDto)
        {
            var existing = await _productData.GetByIdAsync(productDto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Intento de actualizar un producto inexistente con ID {ProductId}", productDto.Id);
                return false;
            }

            existing.CategoryId = productDto.CategoryId;
            existing.FavoriteId = productDto.FavoriteId;

            return await _productData.UpdateAsync(existing);
        }

        // Borrado lógico de un producto
        public async Task<bool> DeleteLogicAsync(int id)
        {
            var existing = await _productData.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Intento de eliminar lógicamente un producto inexistente con ID {ProductId}", id);
                return false;
            }

            return await _productData.DeleteLogicAsync(id);
        }

        // Borrado físico de un producto
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            var existing = await _productData.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Intento de eliminar físicamente un producto inexistente con ID {ProductId}", id);
                return false;
            }

            return await _productData.DeletePersistenceAsync(id);
        }
            //Método para mapear de Product a ProductDto
        private ProductDto MapToDTO(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                FavoriteId = product.FavoriteId
            };
        }

        //Método para mapear de ProductDto a Product
        private Product MapToEntity(ProductDto productDto)
        {
            return new Product
            {
                Id = productDto.Id,
                CategoryId = productDto.CategoryId,
                FavoriteId = productDto.FavoriteId
            };
        }

        //Método para mapear una lista de Product a una lista de ProductDto
        private IEnumerable<ProductDto> MapToList(IEnumerable<Product> products)
        {
            var productDtos = new List<ProductDto>();
            foreach (var product in products)
            {
                productDtos.Add(MapToDTO(product));
            }
            return productDtos;
        }

    }
}
