using Optivio.Shared.DTOs.Product;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(ProductFilterDto filter);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetByBrandAsync(int brandId);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, CreateProductDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductDto>> GetSimilarProductsAsync(int productId);
    }
}